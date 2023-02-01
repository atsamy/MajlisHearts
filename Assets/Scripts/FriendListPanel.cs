using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

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
    [SerializeField]
    Button inviteBtn;
    //[SerializeField]
    //Text friendName;
    [SerializeField]
    MuliGameOptions gameOptions;

    List<string> acceptedFriends;

    string[] ids;

    Dictionary<string, FriendListItem> friendsList;
    Dictionary<string, string> friendsInfo = new Dictionary<string, string>();
    Dictionary<string, string> sendRequestMessage = new Dictionary<string, string>();

    private void Start()
    {
        acceptedFriends = new List<string>();

        ChatManager.OnPlayerStatusUpdate += ChatManager_OnPlayerStatusUpdate;
        friendsList = new Dictionary<string, FriendListItem>();

        AddAllFriends();
    }

    private void OnDestroy()
    {
        ChatManager.OnPlayerStatusUpdate -= ChatManager_OnPlayerStatusUpdate;
    }

    private void AddAllFriends(Action<bool> success = null)
    {
        PlayfabManager.instance.GetFriends((friends) =>
        {
            ids = new string[friends.Count];
            //friendsInfo = friends;
            Dictionary<string, string> friendRequests = new Dictionary<string, string>();
            for (int i = 0; i < friends.Count; i++)
            {
                if (friends[i].Tags != null)
                {
                    if (friends[i].Tags[0] == "confirmed" || acceptedFriends.Contains(friends[i].TitleDisplayName))
                    {
                        print(friends[i].TitleDisplayName + " " + friends[i].Profile.AvatarUrl);
                        AvatarManager.Instance.SetPlayerAvatar(friends[i].TitleDisplayName, friends[i].Profile.AvatarUrl);
                        FriendListItem friend = Instantiate(friendItem, content).GetComponent<FriendListItem>();
                        friend.Set(friends[i].TitleDisplayName,true, friends[i].Profile.PlayerId,FriendValueChanged);
                        friendsList.Add(friends[i].TitleDisplayName, friend);
                    }
                    else if (friends[i].Tags[0] == "requester")
                    {
                        friendRequests.Add(friends[i].FriendPlayFabId, friends[i].TitleDisplayName);
                    }
                    else if (friends[i].Tags[0] == "requestee")
                    {
                        AvatarManager.Instance.SetPlayerAvatar(friends[i].TitleDisplayName, friends[i].Profile.AvatarUrl);
                        FriendListItem friend = Instantiate(friendItem, content).GetComponent<FriendListItem>();
                        friend.Set(friends[i].TitleDisplayName, false, friends[i].Profile.PlayerId,FriendValueChanged);
                        friendsList.Add(friends[i].TitleDisplayName, friend);
                    }

                    friendsInfo.Add(friends[i].Profile.DisplayName, friends[i].Profile.AvatarUrl);
                    ids[i] = friends[i].TitleDisplayName;
                }

            }
            if (friendRequests.Count > 0)
            {
                MenuManager.Instance.ShowFriendRequest(friendRequests);
            }
            if(ids.Length > 0)
                ChatManager.Instance.AddFriends(ids);

            //friendRequests.Clear();

            success?.Invoke(true);
        });
    }

    void FriendValueChanged(bool value)
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

        inviteBtn.interactable = isPlayersSelected;
    }

    internal bool IsConfirmedFriend(string sender)
    {
        if (!friendsList.ContainsKey(sender))
            return false;

        return friendsList[sender].IsConfirmed;
    }

    public void RemoveAllFriends()
    {
        friendsList.Clear();
        for (int i = 0; i < content.childCount; i++)
        {
            Destroy(content.GetChild(i).gameObject);
        }

        friendsInfo.Clear();
    }

    private void ChatManager_OnPlayerStatusUpdate(string user, int status)
    {
        print(user+","+status);

        if (!friendsList.ContainsKey(user))
            return;

        if (status == 2)
        {
            friendsList[user].SetOnline();

            if (sendRequestMessage.ContainsKey(user))
            {
                print("send to:" + user);
                ChatManager.Instance.SendPrivateMessage(user, sendRequestMessage[user]);
                sendRequestMessage.Remove(user);
            }
        }
        else
        {
            friendsList[user].SetOffline();
        }
    }

    internal void AcceptFriend(string friendName)
    {
        acceptedFriends.Add(friendName);
        RemoveAllFriends();
        AddAllFriends((result) =>
        {
            if (result)
            {
                ChatManager.Instance.SendPrivateMessage(friendName, "accept");
            }
        });
    }

    public void Refresh()
    {
        RemoveAllFriends();
        AddAllFriends();
        ChatManager.Instance.Refresh();
    }

    public void Show()
    {
        //SFXManager.Instance.PlayClip("Select");
        friendListPanel.Open();
        //MenuManager.Instance.CurrentScene = friendListPanel.gameObject;
    }

    public void ConfirmFriend(string friendName)
    {
        ChatManager.Instance.Refresh();
        if (!friendsList.ContainsKey(friendName))
        {
            Debug.LogError("user " + friendName + " doesnt exits!");
            return;
        }
        friendsList[friendName].Confirm();
    }
    //public void OpenAddPanel()
    //{
    //    addFriendPanel.SetActive(true);
    //}

    public void SendFriendRequest()
    {
        if (string.IsNullOrEmpty(FriendNameEntry.text))
        {
            TouchScreenKeyboard.Open("");
            return;
        }
        string friendName = FriendNameEntry.text;
        PlayfabManager.instance.AddFriend(friendName, (success,myPlayfabID) =>
        {
            if (success)
            {
                FriendNameEntry.text = "";
                RemoveAllFriends();
                AddAllFriends((success) =>
                {
                    if (success)
                    {
                        sendRequestMessage.Add(friendName, "friendRequest:" + myPlayfabID);
                        //ChatManager.Instance.SendPrivateMessage(friendName, "friendRequest:" + accoundInfo.PlayFabId);
                    }
                });
            }
            else
            {
                MenuManager.Instance.OpenPopup("nomatchname");
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
            MenuManager.Instance.OpenPopup("choosefriends");
            return;
        }

        friendListPanel.gameObject.SetActive(false);

        gameOptions.OpenFriendGame((cost, type) =>
        {
            string roomName = UnityEngine.Random.Range(0, 10000).ToString("0000");
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
