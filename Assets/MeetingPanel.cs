using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MeetingPanel : MenuScene, IConnectionCallbacks, IInRoomCallbacks, IMatchmakingCallbacks, IOnEventCallback
{
    string roomName;
    bool isHost;

    public GameObject StartGameButton;

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

            roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();
            roomOptions.CustomRoomProperties.Add("Customization", GameManager.Instance.Customization);

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
        PhotonNetwork.RaiseEvent(1, null, eventOptionsCards, SendOptions.SendReliable);
    }

    public void OnConnected()
    {
        
    }

    public void OnConnectedToMaster()
    {
        PhotonNetwork.AuthValues = new AuthenticationValues() { UserId = GameManager.Instance.MyPlayer.Name };
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "player", 1 } });
        PhotonNetwork.LocalPlayer.NickName = GameManager.Instance.MyPlayer.Name;

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
        List<InventoryItem> customization = (List<InventoryItem>)PhotonNetwork.CurrentRoom.CustomProperties["Customization"];
        MajlisScript.Instance.SetItems(customization);
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

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == 1)
        {
            GameManager.Instance.IsMultiGame = true;
            SceneManager.LoadScene(2);
        }
    }
}
