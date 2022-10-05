using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class FriendListPanel : MonoBehaviour
{
    [SerializeField]
    MenuScene friendListPanel;
    [SerializeField]
    GameObject friendItem;
    [SerializeField]
    Transform content;
    //[SerializeField]
    //GameObject addFriendPanel;
    [SerializeField]
    InputField FriendNameEntry;
    [SerializeField]
    GameObject error;
    //[SerializeField]
    //Text friendName;
    [SerializeField]
    MuliGameOptions gameOptions;

    string[] ids;

    Dictionary<string, FriendListItem> friendsList;
    Dictionary<string, string> friendsInfo = new Dictionary<string, string>();

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
            //friendsInfo = friends;
            for (int i = 0; i < friends.Count; i++)
            {
                friendsInfo.Add(friends[i].Profile.DisplayName,friends[i].Profile.AvatarUrl);
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
        //SFXManager.Instance.PlayClip("Select");
        friendListPanel.Open();
        //MenuManager.Instance.CurrentScene = friendListPanel.gameObject;
    }

    //public void OpenAddPanel()
    //{
    //    addFriendPanel.SetActive(true);
    //}

    public void AddFriend()
    {
        if (string.IsNullOrEmpty(FriendNameEntry.text))
            return;

        PlayfabManager.instance.AddFriend(FriendNameEntry.text, (success) =>
        {
            if (success)
            {
                FriendNameEntry.text = "";
                //CloseAddPanel();
                //add friend
                RemoveAllFriends();
                AddAllFriends();
            }
            else
            {
                MenuManager.Instance.Popup.ShowWithCode("nomatchname");
                //error.SetActive(true);
            }
        });
    }

    public void Close()
    {
        friendListPanel.Close();
    }

    public void SendInvite()
    {
        bool isPlayersSelected = false;
        friendListPanel.gameObject.SetActive(false);

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

        gameOptions.OpenFriendGame((cost, type) =>
        {
            string roomName = Random.Range(0, 10000).ToString("0000");
            string gameType = type == 0 ? "single" : "team";

            List<PlayerInfo> PlayersInvited = new List<PlayerInfo>();
            PlayersInvited.Add(GameManager.Instance.MyPlayer);

            foreach (var item in friendsList)
            {
                if (item.Value.InviteToggle.isOn)
                {
                    isPlayersSelected = true;
                    ChatManager.Instance.SendPrivateMessage(item.Key, "invite:" + roomName + ":" + gameType + ":" + cost);

                    PlayersInvited.Add(new PlayerInfo()
                    {
                        Name = item.Key,
                        Avatar = friendsInfo[item.Key]
                    });
                }
            }

            GameManager.Instance.IsTeam = (type == 1);
            MenuManager.Instance.OpenMeeting(roomName, cost, PlayersInvited);

            ChatManager.Instance.SubscribeToChannel(roomName);
            ChatManager.OnPlayerStatusUpdate -= ChatManager_OnPlayerStatusUpdate;
        });
    }

}
