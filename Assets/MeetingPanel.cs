using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeetingPanel : MenuScene, IConnectionCallbacks, IInRoomCallbacks, IMatchmakingCallbacks
{
    string roomName;
    bool isHost;

    void Start()
    {
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            print("not connected");

            PhotonNetwork.NickName = GameManager.Instance.MyPlayer.Name;

            AuthenticationValues auth = new AuthenticationValues(GameManager.Instance.MyPlayer.Name);
            //auth.UserId = GameManager.Instance.MyPlayer.Name;
            PhotonNetwork.AuthValues = auth;
            PhotonNetwork.ConnectUsingSettings();
            //chatClient.ConnectUsingSettings(PhotonNetwork.PhotonServerSettings.AppSettings.GetChatSettings());
        }
        else
        {
            print("is connected");
            Connected();
        }
    }

    public void Open(string roomName,bool isHost)
    {
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

            PhotonNetwork.CreateRoom(roomName, roomOptions);
        }
        else
        {
            PhotonNetwork.JoinRoom(roomName);
        }
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
        
    }

    public void OnJoinRoomFailed(short returnCode, string message)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public void OnJoinRandomFailed(short returnCode, string message)
    {
        
    }

    public void OnLeftRoom()
    {
        
    }
}
