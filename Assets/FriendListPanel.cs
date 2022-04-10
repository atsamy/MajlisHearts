using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Photon.Chat;
using AuthenticationValues = Photon.Realtime.AuthenticationValues;
using Photon.Chat.Demo;

public class FriendListPanel : MonoBehaviourPunCallbacks
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

    private void Start()
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

    public void Show()
    {
        friendListPanel.gameObject.SetActive(true);
        //PhotonNetwork.Friends
    }
    
    public override void OnFriendListUpdate(List<FriendInfo> friendList)
    {
        foreach (Transform item in content)
        {
            Destroy(item.gameObject);
        }

        foreach (FriendInfo item in friendList)
        {
            Text text = Instantiate(friendItem, content).GetComponent<Text>();
            text.text = item.UserId;

            text.color = item.IsOnline ? Color.green : Color.black;
        }
    }

    public void RefreshFriends()
    {
        PhotonNetwork.FindFriends(ids);
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected To Matser");
        Connected();
    }

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

    void Connected()
    {
        //PhotonNetwork.AuthValues.AuthType = CustomAuthenticationType.None;

        PlayfabManager.instance.GetFriends((friends) =>
        {
            ids = new string[friends.Count];

            for (int i = 0; i < friends.Count; i++)
            {
                ids[i] = friends[i].TitleDisplayName;
            }

            PhotonNetwork.FindFriends(ids);
        });
    }

    public void Invite()
    {
        //PhotonNetwork.PlayerList[1].se
    }
}
