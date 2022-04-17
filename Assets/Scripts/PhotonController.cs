//using Photon.Pun;
//using Photon.Realtime;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class PhotonController : MonoBehaviour, IConnectionCallbacks
//{
//    private void OnEnable()
//    {
//        PhotonNetwork.AddCallbackTarget(this);
//    }

//    private void OnDisable()
//    {
//        PhotonNetwork.RemoveCallbackTarget(this);
//    }
//    // Start is called before the first frame update
//    void Start()
//    {
//        if (!PhotonNetwork.IsConnectedAndReady)
//        {
//            print("not connected");

//            PhotonNetwork.ConnectUsingSettings();
//        }
//        else
//        {
//            print("is connected");
//            Connected();
//        }
//    }

//    void Connected()
//    {
        
//    }

//    public void OnConnected()
//    {
//        PhotonNetwork.NickName = GameManager.Instance.MyPlayer.Name;
//        PhotonNetwork.AuthValues.UserId = GameManager.Instance.MyPlayer.Name;

//        PlayfabManager.instance.GetFriends((friends) => 
//        {
//            PhotonNetwork.FindFriends();
//        });

        
//    }

//    public void OnConnectedToMaster()
//    {
//        Connected();
//    }

//    public void OnCustomAuthenticationFailed(string debugMessage)
//    {
        
//    }

//    public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
//    {
        
//    }

//    public void OnDisconnected(DisconnectCause cause)
//    {
        
//    }

//    public void OnRegionListReceived(RegionHandler regionHandler)
//    {
        
//    }
//}
