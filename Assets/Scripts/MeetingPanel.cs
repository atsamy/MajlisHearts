using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MeetingPanel : MenuScene, IConnectionCallbacks, IInRoomCallbacks, IMatchmakingCallbacks, IOnEventCallback
{
    string roomName;
    bool isHost;

    [SerializeField]
    GameObject startGameButton;
    [SerializeField]
    GameObject shuffleSeatsButton;
    [SerializeField]
    SpriteRenderer[] avatars;

    const int startGameCode = 1;
    const int shuffleCode = 2;

    string[] playersOrder;

    Dictionary<string, Sprite> lookUpAvatars;

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    void Start()
    {
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            print("not connected");
            AuthenticationValues auth = new AuthenticationValues(GameManager.Instance.MyPlayer.Name);

            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable()
            {
                    { "avatar", GameManager.Instance.MyPlayer.Avatar }
            });

            PhotonNetwork.LocalPlayer.NickName = GameManager.Instance.MyPlayer.Name;
            PhotonNetwork.AuthValues = auth;
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            print("is connected");
            Connected();
        }
    }

    public void Open(string roomName, bool isHost)
    {
        gameObject.SetActive(true);

        this.roomName = roomName;
        this.isHost = isHost;

        playersOrder = new string[4];

        lookUpAvatars = new Dictionary<string, Sprite>();

        if (isHost)
        {
            playersOrder[0] = GameManager.Instance.MyPlayer.Name;

            for (int i = 1; i < playersOrder.Length; i++)
            {
                playersOrder[i] = "empty";
            }

            return;
        }
    }

    void Connected()
    {
        if (isHost)
        {
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 4;
            roomOptions.IsVisible = false;
            roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();

            Wrapper<InventoryItem> wrappedCustomization = new Wrapper<InventoryItem>();

            wrappedCustomization.array = GameManager.Instance.Customization.ToArray();
            roomOptions.CustomRoomProperties.Add("Customization", JsonUtility.ToJson(wrappedCustomization));
            roomOptions.CustomRoomProperties.Add("players", playersOrder);

            PhotonNetwork.CreateRoom(roomName, roomOptions);

            CreateAvatar(PhotonNetwork.LocalPlayer);
        }
        else
        {
            PhotonNetwork.JoinRoom(roomName);
        }
    }

    public void StartGame()
    {
        RaiseEventOptions eventOptionsCards = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(startGameCode, null, eventOptionsCards, SendOptions.SendReliable);
    }

    public void OnConnected()
    {

    }

    public void OnConnectedToMaster()
    {
        Connected();
    }

    public void OnCustomAuthenticationFailed(string debugMessage)
    {

    }

    public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
    {

    }

    public void OnDisconnected(DisconnectCause cause)
    {

    }

    public void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {

    }

    public void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        if (isHost && PhotonNetwork.PlayerList.Length > 1)
        {
            startGameButton.SetActive(true);
            shuffleSeatsButton.SetActive(true);

            playersOrder[newPlayer.ActorNumber - 1] = newPlayer.NickName;
            PhotonNetwork.CurrentRoom.CustomProperties["players"] = playersOrder;
        }

        CreateAvatar(newPlayer);
    }

    public void Shuffle()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;

        string temp = playersOrder[1];
        playersOrder[1] = playersOrder[3];
        playersOrder[3] = playersOrder[2];
        playersOrder[2] = temp;

        PhotonNetwork.CurrentRoom.CustomProperties["players"] = playersOrder;

        RaiseEventOptions eventOptionsCards = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(shuffleCode, playersOrder, eventOptionsCards, SendOptions.SendReliable);
    }

    void RearragePlayers(string[] players)
    {
        //string[] players = (string[])PhotonNetwork.CurrentRoom.CustomProperties["players"];

        for (int i = 1; i < players.Length; i++)
        {
            if (players[i] != "empty")
            {
                avatars[i].sprite = lookUpAvatars[players[i]];
                avatars[i].gameObject.SetActive(true);
            }
            else
            {
                avatars[i].gameObject.SetActive(false);
            }
        }
    }

    public void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {

    }

    public void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {

    }

    public void OnRegionListReceived(RegionHandler regionHandler)
    {

    }

    public void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {

    }

    public void OnFriendListUpdate(List<FriendInfo> friendList)
    {

    }

    public void OnCreatedRoom()
    {

    }

    public void OnCreateRoomFailed(short returnCode, string message)
    {

    }

    public void OnJoinedRoom()
    {
        Debug.Log("Joined private room successfully");

        List<InventoryItem> customization = JsonUtility.FromJson<Wrapper<InventoryItem>>(PhotonNetwork.CurrentRoom.CustomProperties["Customization"].ToString()).array.ToList();
        MajlisScript.Instance.SetItems(customization);

        foreach (var item in PhotonNetwork.PlayerList)
        {
            CreateAvatar(item);
        }
    }

    public void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed joining private room");
        PhotonNetwork.JoinRoom(roomName);
    }

    public void OnJoinRandomFailed(short returnCode, string message)
    {

    }

    public void OnLeftRoom()
    {

    }

    public void OnEvent(EventData photonEvent)
    {
        switch (photonEvent.Code)
        {
            case startGameCode:
                StartCoroutine(StartGameRoutine());
                break;
            case shuffleCode:
                RearragePlayers((string[])photonEvent.CustomData);
                break;
        }
    }
    IEnumerator StartGameRoutine()
    {
        AddBots();

        yield return new WaitForSeconds(3);

        GameManager.Instance.GameType = GameType.Online;
        SceneManager.LoadScene(2);
    }

    private void CreateAvatar(Photon.Realtime.Player newPlayer)
    {
        string path = "Avatar/Body/" + newPlayer.CustomProperties["avatar"];
        Sprite avatar = Resources.Load<Sprite>(path);

        avatars[newPlayer.ActorNumber - 1].gameObject.SetActive(true);
        avatars[newPlayer.ActorNumber - 1].sprite = avatar;

        lookUpAvatars.Add(newPlayer.NickName, avatar);
    }


    private void AddBots()
    {
        for (int i = PhotonNetwork.PlayerList.Length - 1; i < 4; i++)
        {
            avatars[i].gameObject.SetActive(true);
        }
    }
}
