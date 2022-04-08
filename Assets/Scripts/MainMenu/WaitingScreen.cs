using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;
//using photon;

public class WaitingScreen : MenuScene, IInRoomCallbacks, IMatchmakingCallbacks, IConnectionCallbacks, IOnEventCallback
{
    //public Transform Circle;
    //public Transform OppositeCircle;

    public Text gameInfoTop;

    public GameObject StartGameButton;
    //public GameObject UserInput;
    //public Text RoomCode;
    bool joinRoom = false;
    //public Text gameInfoBottom;

    enum GameMode { Random, CreateRoom, JoinRoom }

    GameMode mode = GameMode.Random;

    bool IsconnectedToMaster;
    bool gameReady;

    string roomName;
    //int RaceIndex = 0;
    bool createRoomWhenReady;

    //public GameObject Searching;
    //public GameObject GreenDot;

    public GameObject BackButton;
    //public GameObject RoomNumberPanel;

    public Text InfoText;

    float timer = 0;

    string PrivateRoomName;
    const int beginGame = 25;
    //public AudioSource Audio;

    bool roomCreated;
    // Start is called before the first frame update
    void Start()
    {
        //Invoke("ResetText", 2);
        timer = 0;
    }

    // Update is called once per frame
    //void Update()
    //{
    //    //if (GameManager.Instance.IsRankingGame)
    //    //{
    //    //    Circle.Rotate(0, 0, -Time.deltaTime * 25);
    //    //    OppositeCircle.Rotate(0, 0, Time.deltaTime * 25);

    //    //    Color color = gameInfoTop.color;
    //    //    color.a = Mathf.Abs(Mathf.Cos(Time.time * 2));
    //    //    gameInfoTop.color = color;

    //    //    timer += Time.deltaTime;
    //    //}

    //    //print(roomCreated);
    //}

    //IEnumerable startAIGame()
    //{
    //    yield return new WaitForSeconds(12);

    //    GameManager.Instance.StartAIGame();
    //}

    public override void Close()
    {
        PhotonNetwork.RemoveCallbackTarget(this);

        if (PhotonNetwork.IsConnected)
            PhotonNetwork.Disconnect();

        IsconnectedToMaster = false;
        //UserInput.SetActive(false);
        joinRoom = false;

        //gameInfoTop.text = "looking for other player";

        //Circle.GetComponent<Image>().color = Color.white;

        //RoomNumberPanel.SetActive(false);

        mode = GameMode.Random;
        BackButton.SetActive(false);

        //GreenDot.SetActive(false);
        //gameInfoTop.text = "";
        //Audio.mute = false;

        //SFXManager.Instance.FadeInMusic();

        roomCreated = false;

        //BackButton.SetActive(true);

        base.Close();
    }

    public void OpenAndCreatePrivateRoom()
    {
        //Searching.SetActive(false);

        mode = GameMode.CreateRoom;

        //Audio.mute = true;

        gameInfoTop.text = "Waiting for the other player";
        //RoomNumberPanel.SetActive(true);


        //Circle.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.16f);
        //InfoText.text = "Creating Room";

        Open();

        StartCoroutine(Shuffle());
    }

    IEnumerator Shuffle()
    {
        while (!roomCreated)
        {
            InfoText.text = Random.Range(0, 10000).ToString("0000");
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void OpenAndJoinPrivateRoom()
    {
        //Searching.SetActive(false);

        mode = GameMode.JoinRoom;
        //BackButton.SetActive(true);
        //UserInput.SetActive(true);

        //Circle.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.16f);

        //Audio.mute = true;

        gameInfoTop.text = "";

        Open();
    }
    public override void Open()
    {
        base.Open();
        PhotonNetwork.AddCallbackTarget(this);
        //GameManager.Instance.SetEquippedItems();

        //SFXManager.Instance.FadeMusic();
        BackButton.SetActive(true);

        if (!PhotonNetwork.IsConnectedAndReady)
        {
            print("not connected");

            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            print("is connected");
            Connected();
        }
    }

    public void OpenRoom()
    {

    }

    public void CreateRoom()
    {

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;
        //roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "rank", GameManager.Instance.Me.RankIndex } };
        roomOptions.IsVisible = mode == GameMode.Random;

        string RoomName = Random.Range(0, 10000).ToString("0000");

        PhotonNetwork.CreateRoom(RoomName);
    }

    //internal void HostReady(string roomName, int raceIndex)
    //{
    //    this.roomName = roomName;
    //    this.RaceIndex = raceIndex;

    //    if (IsconnectedToMaster)
    //    {
    //        print("connected");
    //        print(roomName);
    //        PhotonNetwork.JoinRoom(roomName);
    //    }
    //    else
    //    {
    //        print("not connected");
    //    }

    //    StartCoroutine(TimeOut());
    //}

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

            //print

            if (PhotonNetwork.PlayerList.Length > 1)
            {
                Debug.Log("Start game with " + PhotonNetwork.PlayerList.Length + " players");

                RaiseEventOptions eventOptionsCards = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                PhotonNetwork.RaiseEvent(beginGame, null, eventOptionsCards, SendOptions.SendReliable);
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

                GameManager.Instance.IsMultiGame = false;
                SceneManager.LoadScene(2);
            }

            //GetComponent<AudioSource>().Stop();
            //if (PhotonNetwork.CurrentRoom != null)
            //    PhotonNetwork.LeaveRoom();
            //Audio.mute = true;





            //if (PhotonNetwork.CountOfRooms > 1)
            //{
            //    RaiseEventOptions eventOptionsCards = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            //    PhotonNetwork.RaiseEvent(beginGame, null, eventOptionsCards, SendOptions.SendReliable);
            //}
            //else
            //{
            //    GameManager.Instance.IsMultiGame = false;
            //    SceneManager.LoadScene(1);
            //}
        }
    }

    public void OnJoinedRoom()
    {
        Debug.Log("joined room");

        //if (PhotonNetwork.PlayerList.Length == 4)
        //{
        //    StartCoroutine(StartGameIn(3));
        //}
        //else
        //{
        //    if (mode == GameMode.Random)
        //        StartCoroutine(TimeOut());
        //}

        //GreenDot.SetActive(true);

        //                 print("Joined Room");
        //Debug.Log(PhotonNetwork.room.Name);
    }

    public void OnConnectedToMaster()
    {
        if (!IsconnectedToMaster)
        {
            Debug.Log("OnConnectedToMaster() was called by PUN");
            print("Connected to Master");

            Connected();
        }
    }

    private void Connected()
    {
        PhotonNetwork.NickName = PlayerPrefs.GetString("userName");

        if (mode == GameMode.Random)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else if (mode == GameMode.CreateRoom)
        {
            CreateRoom();
        }
        else
        {
            if (joinRoom)
            {
                JoinPrivateRoom();
            }
        }

        IsconnectedToMaster = true;
    }

    public void JoinRoom()
    {
        if (IsconnectedToMaster)
        {
            JoinPrivateRoom();
        }
        else
        {
            joinRoom = true;
        }

        BackButton.SetActive(false);
        //UserInput.SetActive(false);
    }

    void JoinPrivateRoom()
    {
        //PhotonNetwork.JoinRoom(RoomCode.text);
    }

    //void OnPhotonRandomJoinFailed()
    //{
    //    PhotonNetwork.CreateRoom(null);
    //}

    IEnumerator StartGameIn(int time)
    {
        BackButton.SetActive(false);

        //Audio.mute = true;
        gameReady = true;
        PhotonNetwork.RemoveCallbackTarget(this);
        //GetComponent<AudioSource>().Stop();
        while (time > 0)
        {
            gameInfoTop.text = time.ToString();//LanguageManager.Instance.getString("startafter") + " " + time;
            yield return new WaitForSeconds(1);
            time -= 1;
        }
        GameManager.Instance.IsMultiGame = true;
        SceneManager.LoadScene(2);
    }

    public void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        if (PhotonNetwork.PlayerList.Length == 4)
        {
            //if (PhotonNetwork.IsMasterClient)
            //{
            //    StartGameButton.SetActive(true);
            //}

            if (PhotonNetwork.IsMasterClient)
            {
                RaiseEventOptions eventOptionsCards = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                PhotonNetwork.RaiseEvent(beginGame, null, eventOptionsCards, SendOptions.SendReliable);
            }
            //GetOtherPlayerData();

            //if (mode == GameMode.CreateRoom)
            //    InfoText.text = "Starting The Game";
        }

        InfoText.text = PhotonNetwork.PlayerList.Length + " Players";
        //throw new NotImplementedException();
    }

    public void StartGame()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
        GameManager.Instance.IsMultiGame = true;
        SceneManager.LoadScene(2);
    }

    public void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        //throw new NotImplementedException();
    }

    public void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        //throw new NotImplementedException();
    }

    public void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        //throw new NotImplementedException();
    }

    public void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        //throw new NotImplementedException();
    }

    /// <summary>
    /// Match Making 
    /// </summary>
    /// <param name="friendList"></param>
    public void OnFriendListUpdate(List<FriendInfo> friendList)
    {
        //throw new NotImplementedException();
    }

    public void OnCreatedRoom()
    {
        InfoText.text = ("Created Room");

        if (mode == GameMode.CreateRoom)
        {
            InfoText.text = PhotonNetwork.CurrentRoom.Name;
            roomCreated = true;
        }

        StartCoroutine(TimeOut());
        //throw new NotImplementedException();
    }

    public void OnCreateRoomFailed(short returnCode, string message)
    {
        //throw new NotImplementedException();
        Debug.Log("Create Room Failed " + message);
    }

    public void OnJoinRoomFailed(short returnCode, string message)
    {
        //throw new NotImplementedException();
        BackButton.SetActive(true);
        gameInfoTop.text = message;

        Debug.Log("Join Room Failed " + message);
    }

    public void OnJoinRandomFailed(short returnCode, string message)
    {
        //throw new NotImplementedException();
        Debug.Log("Join Random Failed " + message);

        CreateRoom();
    }

    public void OnLeftRoom()
    {
        //throw new NotImplementedException();
        //PhotonNetwork.LeaveLobby();
        Connected();

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
        //Debug.Log("event: " + photonEvent.Code);

        if (photonEvent.Code == beginGame)
        {
            StartCoroutine(StartGameIn(3));
        }
    }
}
