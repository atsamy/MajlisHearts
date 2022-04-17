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

    public GameObject StartGameButton;
    public SpriteRenderer[] Avatars;

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
        PhotonNetwork.RaiseEvent(1, null, eventOptionsCards, SendOptions.SendReliable);
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
            StartGameButton.SetActive(true);

        CreateAvatar(newPlayer);
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
        if (photonEvent.Code == 1)
        {
            StartCoroutine(StartGameRoutine());
        }
    }

    IEnumerator StartGameRoutine()
    {
        AddBots();

        yield return new WaitForSeconds(3);

        GameManager.Instance.IsMultiGame = true;
        SceneManager.LoadScene(2);
    }

    private void CreateAvatar(Photon.Realtime.Player newPlayer)
    {
        Avatars[newPlayer.ActorNumber - 1].gameObject.SetActive(true);
        string path = "Avatar/Body/" + newPlayer.CustomProperties["avatar"];
        Avatars[newPlayer.ActorNumber - 1].sprite = Resources.Load<Sprite>(path);
    }

    private void AddBots()
    {
        for (int i = PhotonNetwork.PlayerList.Length - 1; i < 4; i++)
        {
            Avatars[i].gameObject.SetActive(true);
        }
    }
}
