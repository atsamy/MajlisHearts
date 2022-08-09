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

    string[] ids;

    Dictionary<string, FriendListItem> friendsList;

    private void Start()
    {
        ChatManager.OnPlayerStatusUpdate += ChatManager_OnPlayerStatusUpdate;
        friendsList = new Dictionary<string, FriendListItem>();

        AddAllFriends();
    }

    private void AddAllFriends()
    {
        PlayfabManager.instance.GetFriends((friends) =>
        {
            ids = new string[friends.Count];

            for (int i = 0; i < friends.Count; i++)
            {
                print(friends[i].TitleDisplayName + " " + friends[i].Profile.AvatarUrl);
                AvatarManager.Instance.SetPlayerAvatar(friends[i].TitleDisplayName, friends[i].Profile.AvatarUrl);
                FriendListItem friend = Instantiate(friendItem, content).GetComponent<FriendListItem>();
                friend.Set(friends[i].TitleDisplayName);
                ids[i] = friends[i].TitleDisplayName;

                friendsList.Add(friends[i].TitleDisplayName, friend);
            }

            ChatManager.Instance.AddFriends(ids);
        });
    }

    public void RemoveAllFriends()
    {
        friendsList.Clear();
        for (int i = 0; i < content.childCount; i++)
        {
            Destroy(content.GetChild(i).gameObject);
        }
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
                friendName.text = "";
                CloseAddPanel();
                //add friend
                RemoveAllFriends();
                AddAllFriends();
            }
            else
            {
                error.SetActive(true);
            }
        });
    }

    public void Close()
    {
        MenuManager.Instance.OpenMain();
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
            //fix
            MenuManager.Instance.Popup.ShowWithCode("choosefriends", null);
            return;
        }

        selectTypePopup.Show(LanguageManager.Instance.GetString("choosemode"),(type) =>
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
