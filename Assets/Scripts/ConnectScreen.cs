using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
//using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using ExitGames.Client.Photon;

public class ConnectScreen : MenuScene, IInRoomCallbacks, IMatchmakingCallbacks, IConnectionCallbacks
{
    public GameObject frinedPrefab;
    public GridLayoutGroup Grid;
    //public RequestInfo requestInfo;

    //public RoomPlayer player1;
    //public RoomPlayer player2;

    public GameObject RoomScreen;
    //bool connectToRoom;
    public Text gameInfo;

    //private IEnumerator waitAndSeeCo;

    public Toggle toggle;
    //public CustomizeData otherCar;
    public GameObject Facebook;

    public bool IsconnectedToMaster;
    public bool gameReady;

    string roomName;
    int RaceIndex = 0;
    bool createRoomWhenReady;

    public override void Close()
    {
        RoomScreen.SetActive(false);

        PhotonNetwork.RemoveCallbackTarget(this);

        if (PhotonNetwork.IsConnected)
            PhotonNetwork.Disconnect();
    }


    public override void Open()
    {
        base.Open();

        PhotonNetwork.AddCallbackTarget(this);

        if (!PhotonNetwork.IsConnected)
            PhotonNetwork.ConnectUsingSettings();
    }

    public void OpenRoom()
    {

    }

    public void CreateRoom()
    {
        RoomOptions options = new RoomOptions() { IsVisible = true, MaxPlayers = 2 };

        int roomIndex = Random.Range(1, 7);

        string RoomName = Random.Range(0, 10000).ToString("0000");

        PhotonNetwork.CreateRoom(RoomName, options, null);

        gameInfo.text = "Created Room";
    }

    internal void HostReady(string roomName, int raceIndex)
    {
        this.roomName = roomName;
        this.RaceIndex = raceIndex;

        if (IsconnectedToMaster)
        {
            print("connected");
            print(roomName);
            PhotonNetwork.JoinRoom(roomName);
        }
        else
        {
            print("not connected");
            //connectToRoom = true;
        }

        StartCoroutine(TimeOut());
    }

    IEnumerator TimeOut()
    {
        yield return new WaitForSeconds(10);

        if (!gameReady)
        {
            if (PhotonNetwork.CurrentRoom != null)
                PhotonNetwork.LeaveRoom();
        }
    }

    public void OnJoinedRoom()
    {
        Debug.Log("joined room");

        if (PhotonNetwork.PlayerList.Length == 2)
        {
            StartCoroutine(StartGameIn(3));
        }
        else
        {
            //ExitGames.Client.Photon.Hashtable prop = new ExitGames.Client.Photon.Hashtable();

            //GameManager.Instance.RouteIndex = Random.Range(1, 7);
            //prop.Add("Index", GameManager.Instance.RouteIndex);

            
            //PhotonNetwork.CurrentRoom.SetCustomProperties(prop);
        }
        gameInfo.text = "Joined Room";
    }

    public void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster() was called by PUN");

        gameInfo.text = "Connected to Master";

        //RoomOptions options = new RoomOptions() { IsVisible = true, MaxPlayers = 2 };

        //string RoomName = UnityEngine.Random.Range(0, 10000).ToString("0000");

        //PhotonNetwork.JoinOrCreateRoom(RoomName, options, null);

        PhotonNetwork.JoinRandomRoom();
    }

    //void OnPhotonRandomJoinFailed()
    //{
    //    PhotonNetwork.CreateRoom(null);
    //}

    IEnumerator StartGameIn(int time)
    {
        gameReady = true;

        while (time > 0)
        {
            //gameInfo.text = LanguageManager.Instance.getString("startafter") + " " + time;
            yield return new WaitForSeconds(1);
            time -= 1;
        }

        SceneManager.LoadScene(2);
    }

    public void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        if (PhotonNetwork.PlayerList.Length == 2)
            StartCoroutine(StartGameIn(3));
        //throw new NotImplementedException();
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
        gameInfo.text = "Created Room";
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
    }

    /// <summary>
    /// connection call backs
    /// </summary>

    public void OnConnected()
    {
        //throw new NotImplementedException();
    }

    public void OnDisconnected(DisconnectCause cause)
    {
        //throw new NotImplementedException();
    }

    public void OnRegionListReceived(RegionHandler regionHandler)
    {
        //throw new NotImplementedException();
    }

    public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
    {
        //throw new NotImplementedException();
    }

    public void OnCustomAuthenticationFailed(string debugMessage)
    {
        //throw new NotImplementedException();
    }

}
