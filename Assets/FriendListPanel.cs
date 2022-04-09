using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

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

    private void Start()
    {
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

    public void Show()
    {
        friendListPanel.gameObject.SetActive(true);

        //PhotonNetwork.Friends
    }
    
    public override void OnFriendListUpdate(List<FriendInfo> friendList)
    {
        foreach (Transform item in content)
        {
            Text friend = item.GetComponent<Text>();

            if (friendList.Find(a => a.UserId == friend.text).IsOnline)
            {
                friend.color = Color.green;
            }
        }
    }
    private void Update()
    {
        //PhotonNetwork.Friends
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
        PhotonNetwork.NickName = GameManager.Instance.MyPlayer.Name;
        PhotonNetwork.AuthValues.UserId = GameManager.Instance.MyPlayer.Name;

        PlayfabManager.instance.GetFriends((friends) =>
        {
            string[] ids = new string[friends.Count];

            for (int i = 0; i < friends.Count; i++)
            {
                Text text = Instantiate(friendItem, content).GetComponent<Text>();
                text.text = friends[i].TitleDisplayName;

                ids[i] = friends[i].TitleDisplayName;
            }

            PhotonNetwork.FindFriends(ids);
        });
    }
}
