using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;
//using photon;

public class MultiPanel : MenuScene, IInRoomCallbacks, IMatchmakingCallbacks, IConnectionCallbacks, IOnEventCallback
{
    public Text gameInfoTop;

    public GameObject StartGameButton;

    public Button JoinRoomButton;

    [SerializeField]
    SelectGroup betSelection;
    [SerializeField]
    SelectGroup typeSelection;
    [SerializeField]
    GameObject LoginPanel;
    //enum GameMode { Random, CreateRoom, JoinRoom }

    //GameMode mode = GameMode.Random;

    bool IsconnectedToMaster;
    bool gameReady;

    bool createRoomWhenReady;
    public GameObject BackButton;
    public Text InfoText;

    string PrivateRoomName;
    const int beginGame = 25;
    bool roomCreated;

    public override void Close()
    {
        PhotonNetwork.RemoveCallbackTarget(this);

        if (PhotonNetwork.IsConnected)
            PhotonNetwork.Disconnect();

        IsconnectedToMaster = false;
        BackButton.SetActive(false);

        roomCreated = false;
        base.Close();
    }

    IEnumerator Shuffle()
    {
        while (!roomCreated)
        {
            InfoText.text = Random.Range(0, 10000).ToString("0000");
            yield return new WaitForSeconds(0.1f);
        }
    }

    public override void Open()
    {
        base.Open();
        PhotonNetwork.AddCallbackTarget(this);
        BackButton.SetActive(true);

        if (!PhotonNetwork.IsConnectedAndReady)
        {
            print("not connected");

            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            print("is connected");
            IsconnectedToMaster = true;
            JoinRoomButton.interactable = true;
        }
    }

    //public void CreateRoom()
    //{

    //    RoomOptions roomOptions = new RoomOptions();
    //    roomOptions.MaxPlayers = 4;
    //    roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "bet", BetSelection.GroupIndex }, { "type", typeSelection.GroupIndex } };
    //    roomOptions.IsVisible = true;

    //    string RoomName = Random.Range(0, 10000).ToString("0000");

    //    PhotonNetwork.CreateRoom(RoomName);
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
        }
    }

    public void OnJoinedRoom()
    {
        Debug.Log("joined room");
    }

    public void OnConnectedToMaster()
    {
        if (!IsconnectedToMaster)
        {
            Debug.Log("OnConnectedToMaster() was called by PUN");
            print("Connected to Master");

            IsconnectedToMaster = true;
            JoinRoomButton.interactable = true;
            //Connected();
        }
    }

    public void JoinOrCreateRoom()
    {
        PhotonNetwork.NickName = GameManager.Instance.MyPlayer.Name;
        PhotonNetwork.AuthValues.UserId = GameManager.Instance.MyPlayer.Name;

        ExitGames.Client.Photon.Hashtable roomOptions = new ExitGames.Client.Photon.Hashtable();
        roomOptions = new ExitGames.Client.Photon.Hashtable() { { "bet", betSelection.GroupIndex }, { "type", typeSelection.GroupIndex } };

        PhotonNetwork.JoinRandomOrCreateRoom(roomOptions,4);
        LoginPanel.SetActive(false);
    }

    IEnumerator StartGameIn(int time)
    {
        BackButton.SetActive(false);

        gameReady = true;
        PhotonNetwork.RemoveCallbackTarget(this);

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
            if (PhotonNetwork.IsMasterClient)
            {
                RaiseEventOptions eventOptionsCards = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                PhotonNetwork.RaiseEvent(beginGame, null, eventOptionsCards, SendOptions.SendReliable);
            }
        }

        InfoText.text = PhotonNetwork.PlayerList.Length + " Players";
    }

    public void StartGame()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
        GameManager.Instance.IsMultiGame = true;
        SceneManager.LoadScene(2);
    }

    public void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {

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
        //InfoText.text = ("Created Room");

        //if (mode == GameMode.CreateRoom)
        //{
        //    InfoText.text = PhotonNetwork.CurrentRoom.Name;
        //    roomCreated = true;
        //}

        //StartCoroutine(TimeOut());
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
        //Debug.Log("event: " + photonEvent.Code);

        if (photonEvent.Code == beginGame)
        {
            StartCoroutine(StartGameIn(3));
        }
    }
}
