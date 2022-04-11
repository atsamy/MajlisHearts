using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Photon.Chat;
using AuthenticationValues = Photon.Chat.AuthenticationValues;
using Photon.Chat.Demo;
using ExitGames.Client.Photon;

public class FriendListPanel : MonoBehaviour, IChatClientListener
{
    [SerializeField]
    GameObject friendListPanel;

    [SerializeField]
    GameObject friendItem;

    [SerializeField]
    Transform content;

    [SerializeField]
    GameObject addFriendPanel;

    [SerializeField]
    GameObject error;

    [SerializeField]
    Text friendName;

    string[] ids;

    ChatClient chatClient;

    Dictionary<string, Toggle> friendsList;

    private void Start()
    {
        AuthenticationValues auth = new AuthenticationValues(GameManager.Instance.MyPlayer.Name);
        //auth.UserId = GameManager.Instance.MyPlayer.Name;


        chatClient = new ChatClient(this);

        chatClient.AuthValues = auth;

        ChatAppSettings chatSettings = PhotonNetwork.PhotonServerSettings.AppSettings.GetChatSettings();

        chatClient.ConnectUsingSettings(chatSettings);
        //if (!PhotonNetwork.IsConnectedAndReady)
        //{
        //    print("not connected");

        //    PhotonNetwork.NickName = GameManager.Instance.MyPlayer.Name;

        //    AuthenticationValues auth = new AuthenticationValues(GameManager.Instance.MyPlayer.Name);
        //    //auth.UserId = GameManager.Instance.MyPlayer.Name;
        //    PhotonNetwork.AuthValues = auth;
        //    PhotonNetwork.ConnectUsingSettings();
        //    //chatClient.ConnectUsingSettings(PhotonNetwork.PhotonServerSettings.AppSettings.GetChatSettings());
        //}
        //else
        //{
        //    print("is connected");
        //    Connected();
        //}
    }

    private void Update()
    {
        if (this.chatClient != null)
        {
            this.chatClient.Service(); // make sure to call this regularly! it limits effort internally, so calling often is ok!
        }
    }
    public void Show()
    {
        friendListPanel.gameObject.SetActive(true);
        //PhotonNetwork.Friends
    }

    //public override void OnFriendListUpdate(List<FriendInfo> friendList)
    //{
    //    foreach (Transform item in content)
    //    {
    //        Destroy(item.gameObject);
    //    }

    //    foreach (FriendInfo item in friendList)
    //    {
    //        Text text = Instantiate(friendItem, content).GetComponent<Text>();
    //        text.text = item.UserId;

    //        text.color = item.IsOnline ? Color.green : Color.black;
    //    }
    //}

    public void RefreshFriends()
    {
        PhotonNetwork.FindFriends(ids);
    }

    //public override void OnConnectedToMaster()
    //{
    //    Debug.Log("Connected To Matser");
    //    Connected();
    //}

    public void OpenAddPanel()
    {
        addFriendPanel.SetActive(true);
    }

    public void CloseAddPanel()
    {
        addFriendPanel.SetActive(false);
    }

    public void AddFriend()
    {
        PlayfabManager.instance.AddFriend(friendName.text, (success) =>
        {
            if (success)
            {
                CloseAddPanel();
            }
            else
            {
                error.SetActive(true);
            }
        });
    }

    public void Close()
    {
        friendListPanel.gameObject.SetActive(false);
    }

    //void Connected()
    //{
    //    //PhotonNetwork.AuthValues.AuthType = CustomAuthenticationType.None;
    //    Debug.Log("Connected to chat client");

    //    chatClient.SetOnlineStatus(ChatUserStatus.Online);

    //    PlayfabManager.instance.GetFriends((friends) =>
    //    {
    //        ids = new string[friends.Count];

    //        for (int i = 0; i < friends.Count; i++)
    //        {
    //            ids[i] = friends[i].TitleDisplayName;
    //        }

    //        PhotonNetwork.FindFriends(ids);
    //    });


    //}

    public void Invite()
    {
        //PhotonNetwork.PlayerList[1].se
    }

    public void DebugReturn(DebugLevel level, string message)
    {
        //throw new System.NotImplementedException();
    }

    public void OnDisconnected()
    {
        throw new System.NotImplementedException();
    }

    public void OnConnected()
    {
        Debug.Log("Connected to photon chat");
        //throw new System.NotImplementedException();
        chatClient.SetOnlineStatus(ChatUserStatus.Online);
        friendsList = new Dictionary<string, Toggle>();

        PlayfabManager.instance.GetFriends((friends) =>
        {
            ids = new string[friends.Count];

            for (int i = 0; i < friends.Count; i++)
            {
                Toggle friend = Instantiate(friendItem, content).GetComponent<Toggle>();
                friend.GetComponentInChildren<Text>().text = friends[i].TitleDisplayName;
                ids[i] = friends[i].TitleDisplayName;

                friendsList.Add(friends[i].TitleDisplayName, friend);
            }

            chatClient.AddFriends(ids);
        });
    }

    public void OnChatStateChange(ChatState state)
    {
        //throw new System.NotImplementedException();
    }

    public void SendInvite()
    {
        string roomName = Random.Range(0, 10000).ToString("0000");

        foreach (var item in friendsList)
        {
            if (item.Value.isOn)
            {
                chatClient.SendPrivateMessage(item.Key, "RoomNumber:" + roomName);
            }
        }

        //MenuManager.Instantiate.ShowMajlis();
        //
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        //throw new System.NotImplementedException();
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        //throw new System.NotImplementedException();
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        //throw new System.NotImplementedException();
    }

    public void OnUnsubscribed(string[] channels)
    {
        //throw new System.NotImplementedException();
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        //throw new System.NotImplementedException();
        friendsList[user].interactable = status == 2;
        friendsList[user].GetComponentInChildren<Text>().color = status == 2 ? Color.green : Color.black;
        //Debug.Log("Status change for: " + user + " to: " + status);
    }

    public void OnUserSubscribed(string channel, string user)
    {
        //throw new System.NotImplementedException();
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        //throw new System.NotImplementedException();
    }
}
