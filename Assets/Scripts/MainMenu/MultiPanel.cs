using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;
//using photon;

public class MultiPanel : MonoBehaviour, IInRoomCallbacks, IMatchmakingCallbacks, IConnectionCallbacks, IOnEventCallback
{
    public Text gameInfoTop;
    //public GameObject StartGameButton;
    //public Button JoinRoomButton;

    [SerializeField]
    SelectGroup betSelection;
    [SerializeField]
    SelectGroup typeSelection;
    [SerializeField]
    GameObject LoginPanel;
    [SerializeField]
    GameObject WaitPanel;
    [SerializeField]
    Transform playersContent;
    [SerializeField]
    GameObject playerEntry;
    [SerializeField]
    GameObject footer;

    bool IsconnectedToMaster;
    bool gameReady;

    bool createRoomWhenReady;
    public GameObject BackButton;
    public Text InfoText;

    string PrivateRoomName;
    const int beginGame = 25;
    bool roomCreated;

    public void Close()
    {
        SFXManager.Instance.PlayClip("Close");
        PhotonNetwork.RemoveCallbackTarget(this);

        if (PhotonNetwork.IsConnected)
            PhotonNetwork.Disconnect();

        IsconnectedToMaster = false;

        WaitPanel.SetActive(false);
        LoginPanel.SetActive(false);

        MenuManager.Instance.OpenMain();

        roomCreated = false;
        readyToJoin = false;
    }

    IEnumerator Shuffle()
    {
        while (!roomCreated)
        {
            InfoText.text = Random.Range(0, 10000).ToString("0000");
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void Open()
    {
        MenuManager.Instance.CloseMain();
        SFXManager.Instance.PlayClip("Select");
        LoginPanel.SetActive(true);

        PhotonNetwork.AddCallbackTarget(this);
        BackButton.SetActive(true);

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
            IsconnectedToMaster = true;

            if (readyToJoin)
            {
                JoinOrCreateRoom();
            }
            //JoinRoomButton.interactable = true;
        }
    }

    IEnumerator TimeOut()
    {
        for (int i = 0; i < 15; i++)
        {
            Debug.Log(i);
            yield return new WaitForSeconds(1);
        }

        if (!gameReady)
        {
            gameReady = true;
            BackButton.SetActive(false);

            if (PhotonNetwork.PlayerList.Length > 1)
            {
                Debug.Log("Start game with " + PhotonNetwork.PlayerList.Length + " players");

                string data = GameManager.Instance.EquippedItem["TableTop"] + ":" + GameManager.Instance.EquippedItem["CardBack"];

                RaiseEventOptions eventOptionsCards = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                PhotonNetwork.RaiseEvent(beginGame, data, eventOptionsCards, SendOptions.SendReliable);
            }
            else
            {
                PhotonNetwork.Disconnect();
                PhotonNetwork.RemoveCallbackTarget(this);

                int time = 3;
                while (time > 0)
                {
                    gameInfoTop.text = time.ToString();
                    yield return new WaitForSeconds(1);
                    time -= 1;
                }

                GameManager.Instance.GameType = GameType.Fake;
                SceneManager.LoadScene(2);
            }
        }
    }

    public void OnJoinedRoom()
    {
        Debug.Log("joined room");


        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            Multiplayer entry = Instantiate(playerEntry, playersContent).GetComponent<Multiplayer>();
            entry.Set(PhotonNetwork.PlayerList[i].NickName, PhotonNetwork.PlayerList[i].NickName, PhotonNetwork.PlayerList[i].IsLocal, PhotonNetwork.PlayerList[i].IsMasterClient);
        }
    }

    public void OnConnectedToMaster()
    {
        if (!IsconnectedToMaster)
        {
            Debug.Log("OnConnectedToMaster() was called by PUN");
            print("Connected to Master");

            IsconnectedToMaster = true;

            if (readyToJoin)
            {
                JoinOrCreateRoom();
            }
            //JoinRoomButton.interactable = true;
        }
    }

    bool readyToJoin;

    public void ReadyToJoin()
    {
        if (IsconnectedToMaster)
        {
            JoinOrCreateRoom();
            return;
        }

        LoginPanel.SetActive(false);
        WaitPanel.SetActive(true);

        readyToJoin = true;
    }

    public void JoinOrCreateRoom()
    {
        SFXManager.Instance.PlayClip("Select");

        PhotonNetwork.NickName = GameManager.Instance.MyPlayer.Name;
        PhotonNetwork.AuthValues.UserId = GameManager.Instance.MyPlayer.Name;

        ExitGames.Client.Photon.Hashtable roomProperties = new ExitGames.Client.Photon.Hashtable()
        {
            { "bet", betSelection.GroupIndex },
            { "type", typeSelection.GroupIndex }
        };

        print(betSelection.GroupIndex + " " + typeSelection.GroupIndex);

        RoomOptions roomOptions = new RoomOptions()
        {
            IsOpen = true,
            IsVisible = true,
            CustomRoomProperties = roomProperties,
            MaxPlayers = 4,
        };

        roomOptions.CustomRoomPropertiesForLobby = new string[]
        {
             "bet","type"
        };

        PhotonNetwork.JoinRandomOrCreateRoom(roomProperties, 4, MatchmakingMode.FillRoom,
            null, null, null, roomOptions, null);
    }

    IEnumerator StartGameIn(int time)
    {
        BackButton.SetActive(false);

        gameReady = true;


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
        if (PhotonNetwork.IsMasterClient)
        {
            footer.SetActive(true);
        }

        InfoText.text = PhotonNetwork.PlayerList.Length + " Players";

        Multiplayer entry = Instantiate(playerEntry, playersContent).GetComponent<Multiplayer>();
        entry.Set("",newPlayer.NickName,false,false);
    }

    private void StartGame()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
        GameManager.Instance.GameType = GameType.Online;
        SceneManager.LoadScene(2);
    }

    public void SendGameStartEvent()
    {
        string data = GameManager.Instance.EquippedItem["TableTop"] + ":" + GameManager.Instance.EquippedItem["CardBack"];

        RaiseEventOptions eventOptionsCards = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(beginGame, data, eventOptionsCards, SendOptions.SendReliable);
    }

    public void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        //remove player
    }

    public void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {

    }

    public void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {

    }

    public void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {

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

        PhotonNetwork.CurrentRoom.CustomProperties["TableTop"] =
            GameManager.Instance.EquippedItem["TableTop"];

        PhotonNetwork.CurrentRoom.CustomProperties["CardBack"] =
            GameManager.Instance.EquippedItem["CardBack"];

        //Multiplayer entry = Instantiate(playerEntry, playersContent).GetComponent<Multiplayer>();
        //entry.Set(GameManager.Instance.MyPlayer.Avatar, GameManager.Instance.MyPlayer.Name, true, true);
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
        //Connected();
    }

    void ResetText()
    {
        //showText = true;
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
        if (photonEvent.Code == beginGame)
        {
            string[] data = photonEvent.CustomData.ToString().Split(':');

            PhotonNetwork.CurrentRoom.CustomProperties["TableTop"] = data[0];
            PhotonNetwork.CurrentRoom.CustomProperties["CardBack"] = data[1];

            StartCoroutine(StartGameIn(3));
        }
    }
}
