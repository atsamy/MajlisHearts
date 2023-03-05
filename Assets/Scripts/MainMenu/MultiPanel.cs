using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;
using System.Linq;
//using photon;

public class MultiPanel : MenuScene, IInRoomCallbacks, IMatchmakingCallbacks, IConnectionCallbacks, IOnEventCallback
{
    public Text gameInfoTop;
    int gameType = 0;
    [SerializeField]
    Multiplayer[] playerEntries;
    [SerializeField]
    GameObject footer;

    bool IsconnectedToMaster;
    public GameObject BackButton;
    public Text InfoText;

    const int beginGame = 25;
    const int AiAdded = 26;
    const int sendAllPlayers = 27;

    bool readyToJoin;
    Coroutine AIcoroutine;
    public int GameEntryFee = 50;
    bool skipCreateAI;
    int currentIndex;
    List<PlayerInfo> playerInfos;
    string[] aiNames;
    public TextAsset Names;

    bool rejoinRoom;

    public override void Close()
    {
        SFXManager.Instance.PlayClip("Close");
        PhotonNetwork.RemoveCallbackTarget(this);

        PhotonNetwork.LeaveRoom();

        foreach (var item in playerEntries)
        {
            item.Reset();
        }
        readyToJoin = false;

        if (AIcoroutine != null)
        {
            StopCoroutine(AIcoroutine);
        }

        base.Close();
    }

    public void Reconnect()
    {
        PhotonNetwork.LeaveRoom();
        foreach (var item in playerEntries)
        {
            item.Reset();
        }
        //readyToJoin = false;
        currentIndex = 0;
        rejoinRoom = true;
        //Open(GameManager.Instance.Bet, gameType);
    }

    public void Open(int cost, int type)
    {
        base.Open();
        currentIndex = 0;
        //SFXManager.Instance.PlayClip("Select");
        string[] playersOrder = new string[4];
        playerInfos = new List<PlayerInfo>();

#if UNITY_IOS
        aiNames = Names.text.Split("\n");
#elif UNITY_ANDROID
        aiNames = Names.text.Split("\r\n");
#endif

        GameManager.Instance.Bet = cost;
        gameType = type;
        GameManager.Instance.IsTeam = (gameType == 1);

        if (IsconnectedToMaster)
        {
            JoinOrCreateRoom();
        }
        else
        {
            readyToJoin = true;
        }

        PhotonNetwork.AddCallbackTarget(this);
        BackButton.SetActive(true);

        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable()
            {
                    { "avatar", GameManager.Instance.MyPlayer.Avatar }
            });

        if (!PhotonNetwork.IsConnectedAndReady)
        {
            print("not connected");

            AuthenticationValues auth = new AuthenticationValues(GameManager.Instance.MyPlayer.Name);



            PhotonNetwork.LocalPlayer.NickName = GameManager.Instance.MyPlayer.Name;
            PhotonNetwork.AuthValues = auth;

            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            print("is connected");
            IsconnectedToMaster = true;

            if (readyToJoin)
            {
                JoinOrCreateRoom();
            }
        }
    }

    public void OnJoinedRoom()
    {
        Debug.Log("joined room");

        if (PhotonNetwork.IsMasterClient)
        {
            PlayerInfo newPlayer = new PlayerInfo()
            {
                Name = GameManager.Instance.MyPlayer.Name,
                Avatar = GameManager.Instance.MyPlayer.Avatar,
                Points = GameManager.Instance.MyPlayer.Points
            };


            playerInfos.Add(newPlayer);
            CreateNewPlayer(newPlayer, true, true);

            AIcoroutine = StartCoroutine(AddAiPlayers());
        }
    }

    private void CreateNewPlayer(PlayerInfo player, bool IsLocal, bool IsHost)
    {
        if (!IsLocal)
            AvatarManager.Instance.SetPlayerAvatar(player.Name, player.Avatar);

        playerEntries[currentIndex].Set(player.Name, IsLocal, IsHost);

        currentIndex++;
    }

    private void CreateAllPlayer(PlayerInfo[] players)
    {
        currentIndex = 0;
        for (int i = 0; i < playerEntries.Length; i++)
        {
            if (players.Length > i)
            {
                //playersOrder[i] = players[i].Name;
                CreateNewPlayer(players[i], players[i].Name == GameManager.Instance.MyPlayer.Name,
                players[i].Name == PhotonNetwork.MasterClient.NickName);
            }
            else
            {
                playerEntries[i].Reset();
            }
        }
    }

    public void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster() was called by PUN");

        if (!IsconnectedToMaster || rejoinRoom)
        {
            IsconnectedToMaster = true;
            rejoinRoom = false;
            if (readyToJoin)
            {
                JoinOrCreateRoom();
            }
            //JoinRoomButton.interactable = true;
        }
    }

    public void JoinOrCreateRoom()
    {
        //SFXManager.Instance.PlayClip("Select");

        PhotonNetwork.NickName = GameManager.Instance.MyPlayer.Name;
        PhotonNetwork.AuthValues.UserId = GameManager.Instance.MyPlayer.Name;

        ExitGames.Client.Photon.Hashtable roomProperties = new ExitGames.Client.Photon.Hashtable()
        {
            { "bet", GameManager.Instance.Bet },
            { "type", gameType}
        };
        RoomOptions roomOptions = new RoomOptions()
        {
            IsOpen = true,
            IsVisible = true,
            CustomRoomProperties = roomProperties,
            MaxPlayers = 4,
        };
        roomOptions.CustomRoomPropertiesForLobby = new string[]
        {
            "bet",
            "type"
        };

        PhotonNetwork.JoinRandomOrCreateRoom(roomProperties, 4, MatchmakingMode.FillRoom,
            null, null, null, roomOptions, null);
    }

    IEnumerator StartGameIn(int time)
    {
        BackButton.SetActive(false);
        while (time > 0)
        {
            gameInfoTop.text = time.ToString();//LanguageManager.Instance.getString("startafter") + " " + time;
            yield return new WaitForSeconds(1);
            time -= 1;
        }

        StartGame();
    }

    public void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        //InfoText.text = PhotonNetwork.PlayerList.Length + " Players";

        if (PhotonNetwork.IsMasterClient)
        {
            skipCreateAI = true;

            PlayerInfo newPlayerInfo = new PlayerInfo()
            {
                Avatar = newPlayer.CustomProperties["avatar"].ToString(),
                Name = newPlayer.NickName
            };
            playerInfos.Add(newPlayerInfo);

            SendAllPlayersToOthers();

            CreateNewPlayer(newPlayerInfo, false, false);

            if (playerInfos.Count == 4)
                SendGameStartEvent();
        }

    }

    private void SendAllPlayersToOthers()
    {
        Wrapper<PlayerInfo> wrapper = new Wrapper<PlayerInfo>();
        wrapper.array = playerInfos.ToArray();
        string data = JsonUtility.ToJson(wrapper);

        RaiseEventOptions eventOptionsCards = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent(sendAllPlayers, data, eventOptionsCards, SendOptions.SendReliable);
    }

    private void StartGame()
    {
        FadeScreen.Instance.FadeIn(2, () =>
        {
            GameManager.Instance.DeductCoins(GameManager.Instance.Bet);

            PhotonNetwork.RemoveCallbackTarget(this);
            GameManager.Instance.GameType = GameType.Online;
            SceneManager.LoadScene(2);
        });
    }

    public void SendGameStartEvent()
    {
        string[] playersOrder = new string[4];

        for (int i = 0; i < 4; i++)
        {
            playersOrder[i] = playerInfos[i].Name;
        }

        RaiseEventOptions eventOptionsCards = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(beginGame, playersOrder, eventOptionsCards, SendOptions.SendReliable);
    }

    public void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        if (otherPlayer.ActorNumber == 1)
        {
            Reconnect();
            //MenuManager.Instance.OpenPopup("hostleftmulti",false,false,()=>
            //{
            //    Reconnect();
            //},()=>
            //{
            //    Close();
            //});
        }
        else if (PhotonNetwork.IsMasterClient)
        {
            playerInfos.Remove(playerInfos.Find(a => a.Name == otherPlayer.NickName));

            CreateAllPlayer(playerInfos.ToArray());
            //SetAllPlayers();
            SendAllPlayersToOthers();
        }
    }

    //private void SetAllPlayers()
    //{
    //    for (int i = 0; i < playerEntries.Length; i++)
    //    {
    //        if (playerInfos.Count > i)
    //        {
    //            playerEntries[i].Set(playerInfos[i].Name, playerInfos[i].Name == GameManager.Instance.MyPlayer.Name,
    //                playerInfos[i].Name == PhotonNetwork.MasterClient.NickName);
    //        }
    //        else
    //        {
    //            playerEntries[i].Reset();
    //        }
    //    }
    //}

    public void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {

    }

    public void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {

    }

    public void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        print("master client switched");
    }

    /// <summary>
    /// Match Making 
    /// </summary>
    /// <param name="friendList"></param>
    public void OnFriendListUpdate(List<FriendInfo> friendList)
    {

    }

    public void OnCreatedRoom()
    {
        Debug.Log("room created");
    }

    IEnumerator AddAiPlayers()
    {
        List<int> addedAI = new List<int>();

        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(Random.Range(8f - i, 10f - i));
            //await System.Threading.Tasks.Task.Delay(Random.Range(8000 - i, 10000 - i));

            if (playerInfos.Count < 4 && !skipCreateAI)
            {
                PlayerInfo newPlayer = new PlayerInfo();
                int newIndex = 0;
                do
                {
                    newIndex = Random.Range(0, aiNames.Length);
                }
                while (addedAI.Contains(newIndex));

                addedAI.Add(newIndex);
                string[] aiData = aiNames[newIndex].Split("-");

                newPlayer.Name = aiData[0];
                newPlayer.Avatar = "Avatar" + aiData[1];
                newPlayer.Points = GameManager.Instance.MyPlayer.Points;

                playerInfos.Add(newPlayer);
                CreateNewPlayer(newPlayer, false, false);
                string data = JsonUtility.ToJson(newPlayer);
                RaiseEventOptions eventOptionsCards = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
                PhotonNetwork.RaiseEvent(AiAdded, data, eventOptionsCards, SendOptions.SendReliable);

                if (playerInfos.Count == 4)
                {
                    SendGameStartEvent();
                    break;
                }
            }

            skipCreateAI = false;
        }

    }

    public void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Create Room Failed " + message);
    }

    public void OnJoinRoomFailed(short returnCode, string message)
    {
        BackButton.SetActive(true);
        gameInfoTop.text = message;

        Debug.Log("Join Room Failed " + message);
    }

    public void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Join Random Failed " + message);
        //CreateRoom();
    }

    public void OnLeftRoom()
    {

    }

    public void OnConnected()
    {

    }

    public void OnDisconnected(DisconnectCause cause)
    {
        IsconnectedToMaster = false;
    }

    public void OnRegionListReceived(RegionHandler regionHandler)
    { }

    public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
    { }

    public void OnCustomAuthenticationFailed(string debugMessage)
    { }

    public void OnEvent(EventData photonEvent)
    {
        switch (photonEvent.Code)
        {
            case beginGame:
                {
                    string[] data = (string[])photonEvent.CustomData;
                    PhotonNetwork.CurrentRoom.CustomProperties["players"] = data;

                    StartCoroutine(StartGameIn(3));
                    break;
                }
            case AiAdded:
                {
                    PlayerInfo data = JsonUtility.FromJson<PlayerInfo>(photonEvent.CustomData.ToString());
                    //playersOrder[currentIndex] = data.Name;
                    CreateNewPlayer(data, false, false);
                    break;
                }
            case sendAllPlayers:
                {
                    Wrapper<PlayerInfo> data = JsonUtility.FromJson<Wrapper<PlayerInfo>>(photonEvent.CustomData.ToString());
                    CreateAllPlayer(data.array);
                    break;
                }
        }
    }
}
