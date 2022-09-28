using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MeetingPanel : MenuScene, IConnectionCallbacks, IInRoomCallbacks, IMatchmakingCallbacks, IOnEventCallback
{
    string roomName;
    bool isHost;

    [SerializeField]
    GameObject startGameButton;
    [SerializeField]
    GameObject shuffleSeatsButton;
    [SerializeField]
    GameObject chatPanel;
    [SerializeField]
    GameObject friendsStatusParent;
    [SerializeField]
    FriendStatus[] friendsStatus;
    [SerializeField]
    Image[] avatars;
    [SerializeField]
    Button toggleStatusBtn;

    List<playerStatus> playersStatuse;

    const int startGameCode = 1;
    const int shuffleCode = 2;
    const int playersStatusMessage = 3;

    string[] playersOrder;

    int entryFee;

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

    internal void FriendDeclined(string sender)
    {
        friendsStatus.First(a => a.PlayerName == sender).ChangeStatus("declined");
        playersStatuse.Find(a => a.playerName == sender).Status = "declined";
        SendPlayerStatusChange();

        CheckForPlayers();
    }

    internal void FriendTimedOut(string sender)
    {
        friendsStatus.First(a => a.PlayerName == sender).ChangeStatus("timedOut");
        playersStatuse.Find(a => a.playerName == sender).Status = "timedOut";
        SendPlayerStatusChange();

        CheckForPlayers();
    }

    public void CheckForPlayers()
    {
        bool stillWaiting = false;
        bool playerReady = false;

        for (int i = 1; i < playersStatuse.Count; i++)
        {
            if (playersStatuse[i].Status == "waiting")
                stillWaiting = true;
            else if (playersStatuse[i].Status == "ready")
                playerReady = true;
        }

        if (!stillWaiting && playerReady)
        {
            startGameButton.SetActive(true);

            if (GameManager.Instance.IsTeam)
            {
                shuffleSeatsButton.SetActive(true);
            }
        }
    }

    public void Open(string roomName, int entryfee)
    {
        gameObject.SetActive(true);
        this.roomName = roomName;
        isHost = false;
        this.entryFee = entryfee;
        playersOrder = new string[4];
    }

    public void Open(string roomName, int entryfee, List<PlayerInfo> players)
    {
        Open(roomName, entryfee);
        isHost = true;

        playersOrder[0] = GameManager.Instance.MyPlayer.Name;

        for (int i = 1; i < playersOrder.Length; i++)
        {
            playersOrder[i] = "empty";
        }

        playersStatuse = new List<playerStatus>();

        for (int i = 0; i < 4; i++)
        {
            if (i < players.Count)
            {
                friendsStatus[i].Set(players[i].Name, players[i].Avatar, 0, i == 0 ? "ready" : "waiting");
                playersStatuse.Add(new playerStatus()
                {
                    playerName = players[i].Name,
                    Avatar = players[i].Avatar,
                    Status = i == 0 ? "ready" : "waiting"
                });
            }
            else
            {
                friendsStatus[i].SetLanguage();
            }
        }


        TogglePlayersStatus();
        toggleStatusBtn.interactable = true;

    }

    public void ToggleChatPanel()
    {
        chatPanel.SetActive(!chatPanel.activeSelf);
    }
    public void TogglePlayersStatus()
    {
        friendsStatusParent.SetActive(!friendsStatusParent.activeSelf);
    }

    void Connected()
    {
        if (isHost)
        {
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 4;
            roomOptions.IsVisible = false;
            roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();

            Wrapper<FinishedTask> wrappedCustomization = new Wrapper<FinishedTask>();

            wrappedCustomization.array = TasksManager.Instance.FinishedTasks.ToArray();
            roomOptions.CustomRoomProperties.Add("Customization", JsonUtility.ToJson(wrappedCustomization));
            roomOptions.CustomRoomProperties.Add("players", playersOrder);

            roomOptions.CustomRoomProperties["TableTop"] = GameManager.Instance.EquippedItem["TableTop"];
            roomOptions.CustomRoomProperties["CardBack"] = GameManager.Instance.EquippedItem["CardBack"];

            PhotonNetwork.CreateRoom(roomName, roomOptions);
        }
        else
        {
            PhotonNetwork.JoinRoom(roomName);
        }
    }

    public void StartGame()
    {
        RaiseEventOptions eventOptionsCards = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(startGameCode, playersOrder, eventOptionsCards, SendOptions.SendReliable);


        startGameButton.SetActive(false);
        shuffleSeatsButton.SetActive(false);
        //fades
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
            //startGameButton.SetActive(true);

            //if (GameManager.Instance.IsTeam)
            //    shuffleSeatsButton.SetActive(true);

            playersOrder[newPlayer.ActorNumber - 1] = newPlayer.NickName;
            PhotonNetwork.CurrentRoom.CustomProperties["players"] = playersOrder;

            friendsStatus.First(a => a.PlayerName == newPlayer.NickName).ChangeStatus("ready");
            playersStatuse.Find(a => a.playerName == newPlayer.NickName).Status = "ready";

            SendPlayerStatusChange();
            CheckForPlayers();
        }

        CreateAvatar(newPlayer);
    }

    private void SendPlayerStatusChange()
    {
        Wrapper<playerStatus> wrapper = new Wrapper<playerStatus>();
        wrapper.array = playersStatuse.ToArray();

        string data = JsonUtility.ToJson(wrapper);

        RaiseEventOptions eventOptionsCards = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent(playersStatusMessage, data, eventOptionsCards, SendOptions.SendReliable);
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
        //PhotonNetwork.CurrentRoom.CustomProperties["players"] = players;

        for (int i = 1; i < players.Length; i++)
        {
            if (players[i] != "empty")
            {
                avatars[i].sprite = AvatarManager.Instance.GetPlayerAvatar(players[i]);
                avatars[i].transform.parent.gameObject.SetActive(true);
            }
            else
            {
                avatars[i].transform.parent.gameObject.SetActive(false);
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
        CreateAvatar(PhotonNetwork.LocalPlayer);
    }

    public void OnCreateRoomFailed(short returnCode, string message)
    {

    }

    public void OnJoinedRoom()
    {
        Debug.Log("Joined private room successfully");

        //apply new customization
        List<FinishedTask> customization = JsonUtility.FromJson<Wrapper<FinishedTask>>(PhotonNetwork.CurrentRoom.CustomProperties["Customization"].ToString()).array.ToList();
        MajlisScript.Instance.ResetTask();
        MajlisScript.Instance.AdjustMajlis(customization);

        foreach (var item in PhotonNetwork.PlayerList)
        {
            CreateAvatar(item);
        }
    }

    public void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed joining private room: " + message);
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

                if (!PhotonNetwork.IsMasterClient)
                    PhotonNetwork.CurrentRoom.CustomProperties["players"] = photonEvent.CustomData;

                StartCoroutine(StartGameRoutine());
                break;
            case shuffleCode:
                RearragePlayers((string[])photonEvent.CustomData);
                break;
            case playersStatusMessage:
                playerStatus[] data = JsonUtility.FromJson<Wrapper<playerStatus>>(photonEvent.CustomData.ToString()).array;
                AdjustPlayerStatus(data);
                break;
        }
    }

    private void AdjustPlayerStatus(playerStatus[] customData)
    {
        for (int i = 0; i < 4; i++)
        {
            if (i < customData.Length)
                friendsStatus[i].Set(customData[i]);
            else
                friendsStatus[i].SetLanguage();
        }

        TogglePlayersStatus();
        toggleStatusBtn.interactable = true;
    }

    IEnumerator StartGameRoutine()
    {
        AddBots();

        yield return new WaitForSeconds(3);

        GameManager.Instance.DeductCoins(entryFee);
        GameManager.Instance.Bet = entryFee;
        GameManager.Instance.GameType = GameType.Friends;

        FadeScreen.Instance.FadeIn(2, () =>
        {
            SceneManager.LoadScene(2);
        });
    }

    private void CreateAvatar(Photon.Realtime.Player newPlayer)
    {
        if (newPlayer.ActorNumber < 1)
            return;

        Sprite playerSprite = AvatarManager.Instance.GetPlayerAvatar(newPlayer.NickName);

        if (playerSprite == null)
        {
            AvatarManager.Instance.SetPlayerAvatar(newPlayer.NickName, newPlayer.CustomProperties["avatar"].ToString());
        }

        avatars[newPlayer.ActorNumber - 1].transform.parent.gameObject.SetActive(true);
        avatars[newPlayer.ActorNumber - 1].sprite = AvatarManager.Instance.GetPlayerAvatar(newPlayer.NickName);
    }


    private void AddBots()
    {
        for (int i = PhotonNetwork.PlayerList.Length - 1; i < 4; i++)
        {
            if (!avatars[i].transform.parent.gameObject.activeSelf)
            {
                avatars[i].transform.parent.gameObject.SetActive(true);
                avatars[i].sprite = AvatarManager.Instance.RobotAvatar;
            }
        }
    }

    public override void Close()
    {
        base.Close();
        PhotonNetwork.LeaveRoom();
    }
}

[Serializable]
public class playerStatus
{
    public string playerName;
    public string Avatar;
    public string Status;
}