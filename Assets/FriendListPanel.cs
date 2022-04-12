using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class FriendListPanel : MonoBehaviour
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

    Dictionary<string, Toggle> friendsList;

    private void Start()
    {
        ChatManager.OnPlayerStatusUpdate += ChatManager_OnPlayerStatusUpdate;

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

            ChatManager.Instance.AddFriends(ids);
        });
    }

    private void ChatManager_OnPlayerStatusUpdate(string user, int status)
    {
        friendsList[user].interactable = status == 2;
        friendsList[user].GetComponentInChildren<Text>().color = status == 2 ? Color.green : Color.black;
    }

    public void Show()
    {
        friendListPanel.gameObject.SetActive(true);
        //PhotonNetwork.Friends
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

    public void Invite()
    {

    }

    public void OnDisconnected()
    {
        
    }
    public void SendInvite()
    {
        string roomName = Random.Range(0, 10000).ToString("0000");

        foreach (var item in friendsList)
        {
            if (item.Value.isOn)
            {
                ChatManager.Instance.SendPrivateMessage(item.Key, "RoomNumber:" + roomName);
            }
        }

        friendListPanel.gameObject.SetActive(false);
        ChatManager.OnPlayerStatusUpdate -= ChatManager_OnPlayerStatusUpdate;
    }

}
