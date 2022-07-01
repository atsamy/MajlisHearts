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
    [SerializeField]
    ChoosePopup selectTypePopup;
    //[SerializeField]
    //Button inviteButton;

    string[] ids;

    Dictionary<string, FriendListItem> friendsList;

    private void Start()
    {
        ChatManager.OnPlayerStatusUpdate += ChatManager_OnPlayerStatusUpdate;
        friendsList = new Dictionary<string, FriendListItem>();

        PlayfabManager.instance.GetFriends((friends) =>
        {
            ids = new string[friends.Count];

            for (int i = 0; i < friends.Count; i++)
            {
                FriendListItem friend = Instantiate(friendItem, content).GetComponent<FriendListItem>();
                friend.Set(friends[i].TitleDisplayName);
                ids[i] = friends[i].TitleDisplayName;

                friendsList.Add(friends[i].TitleDisplayName, friend);
            }

            ChatManager.Instance.AddFriends(ids);
        });
    }

    private void ChatManager_OnPlayerStatusUpdate(string user, int status)
    {
        if (status == 2)
        {

            friendsList[user].SetOnline();
        }
        else
        {
            friendsList[user].SetOffline();
        }
    }

    public void Show()
    {
        SFXManager.Instance.PlayClip("Select");
        friendListPanel.gameObject.SetActive(true);
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

    public void OnDisconnected()
    {
        
    }
    public void SendInvite()
    {
        bool isPlayersSelected = false;


        foreach (var item in friendsList)
        {
            if (item.Value.InviteToggle.isOn)
            {
                isPlayersSelected = true;
                break;
            }
        }

        if (!isPlayersSelected)
        {
            MenuManager.Instance.Popup.ShowWithMessage("Please Choose Friends To Invite", null);
            return;
        }

        selectTypePopup.Show((type) =>
        {
            string roomName = Random.Range(0, 10000).ToString("0000");
            string gameType = type == 0 ? "single" : "team";

            foreach (var item in friendsList)
            {
                if (item.Value.InviteToggle.isOn)
                {
                    isPlayersSelected = true;
                    ChatManager.Instance.SendPrivateMessage(item.Key, "invite:" + roomName + ":" + gameType);
                }
            }

            GameManager.Instance.IsTeam = (type == 1);

            friendListPanel.gameObject.SetActive(false);
            gameObject.SetActive(false);

            MenuManager.Instance.OpenMeeting(roomName, true);

            ChatManager.Instance.SubscribeToChannel(roomName);
            ChatManager.OnPlayerStatusUpdate -= ChatManager_OnPlayerStatusUpdate;
        });
    }

}
