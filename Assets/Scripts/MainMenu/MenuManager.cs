using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using ArabicSupport;
using System;
using TMPro;
using NiobiumStudios;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;
    public CameraHover CameraHover;
    public GameObject GameModePanel;
    public SettingsPanel SettingsPanel;
    public StoreScene StoreScene;
    public MainPanelScript MainPanel;
    public MeetingPanel meetingPanel;
    public FriendListPanel friendPanel;
    public InvitePopup InvitePopup;

    public Image timerFill;

    public FriendRequestPopup FriendRequestPopup;


    internal void OpenStore(int index)
    {
        StoreScene.Open(index);
    }

    [SerializeField]
    Popup popup;

    public MenuScene CurrentScene { get; internal set; }

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (!string.IsNullOrEmpty(GameManager.Instance.MyPlayer.Avatar))
        {
            MainPanel.SetAvatar();
        }
        DailyRewards.instance.onClaimPrize += ClaimDailyReward;

        ChatManager.OnGotPrivateMessage += ChatManager_OnGotPrivateMessage;

        //if (PlayerPrefs.GetInt("gamefinished", 0) == 1)
        //{
        //    SpinWheel.SetActive(true);
        //    PlayerPrefs.SetInt("gamefinished", 0);
        //}
    }

    private void ChatManager_OnGotPrivateMessage(string sender, string message)
    {
        if (message == "decline")
        {
            meetingPanel.FriendDeclined(sender);
        }
        else if (message == "timeout")
        {
            meetingPanel.FriendTimedOut(sender);
        }
        else if (message.Contains("friendRequest"))
        {
            FriendRequestPopup.Show(sender, () =>
             {
                 PlayfabManager.instance.AcceptFriendRequest(message.Split(":")[1],sender,(success,friendName) =>
                 {
                     if (success)
                     {
                         friendPanel.AcceptFriend(sender);
                     }
                 });

             }, () =>
             {
                 PlayfabManager.instance.DenyFriendRequest(message.Split(":")[1]);
             });
        }
        else if (message.Contains("accept"))
        {
            friendPanel.ConfirmFriend(sender);
        }
        else
        {
            if (sender != GameManager.Instance.MyPlayer.Name)
                ShowInvitePopup(sender, message);
        }
    }

    void ClaimDailyReward(int day,int multiplier)
    {
        GameManager.Instance.AddCoins(DailyRewards.instance.GetReward(day).reward * multiplier);
    }

    internal void ShowFriendRequest(Dictionary<string, string> friendRequests)
    {
        if (friendRequests.Count > 0)
        {
            FriendRequestPopup.Show(friendRequests.ElementAt(0).Value, () =>
            {
                PlayfabManager.instance.AcceptFriendRequest(friendRequests.ElementAt(0).Key, friendRequests.ElementAt(0).Value,
                    (success,friendName) =>
                {
                    if (success)
                    {
                        ChatManager.Instance.SendPrivateMessage(friendName, "accept");
                        friendPanel.AcceptFriend(friendName);
                    }
                });
                friendRequests.Remove(friendRequests.ElementAt(0).Key);
                ShowFriendRequest(friendRequests);
            }, () =>
            {
                PlayfabManager.instance.DenyFriendRequest(friendRequests.ElementAt(0).Key);
                friendRequests.Remove(friendRequests.ElementAt(0).Key);
                ShowFriendRequest(friendRequests);
            });

        }
    }

    public void StartSingleGame()
    {
        SFXManager.Instance.PlayClip("Select");

        FadeScreen.Instance.FadeIn(2, () => 
        {
            GameManager.Instance.GameType = GameType.Single;
            SceneManager.LoadScene(2);
        });
    }

    public void OpenSettings()
    {
        MainPanel.HideHeader(true,true);
        SettingsPanel.Open();
        HideMain(true, true);
    }

    public void OpenMeeting(string roomName,int entryfee, System.Collections.Generic.List<PlayerInfo> players)
    {
        meetingPanel.OpenAsHost(roomName,entryfee, players);
        MainPanel.HideHeader(true, true);
    }

    public void OpenMeeting(string roomName, int entryfee)
    {
        meetingPanel.OpenAsClient(roomName, entryfee);
        MainPanel.HideHeader(true, true);
    }

    internal void ShowInvitePopup(string sender, string message)
    {
        bool isArabic = LanguageManager.Instance.CurrentLanguage == Language.Arabic;

        string invitationMessage = (isArabic ? "" : " <color=green>" + ArabicFixer.Fix(sender) + "</color> ") +
            LanguageManager.Instance.GetString("invitationmessage") +
            (isArabic ? " <color=green>" + ArabicFixer.Fix(sender) + "</color> " : "");
        string[] inviteOptions = message.Split(':');
        int cost = int.Parse(inviteOptions[3]);

        Coroutine waitforResponse = StartCoroutine(SendTimedOut(sender)); 

        InvitePopup.Show(invitationMessage, cost, () =>
         {
             CloseCurrentScene();
             StopCoroutine(waitforResponse);
             GameManager.Instance.IsTeam = (inviteOptions[2] == "team");
             ChatManager.Instance.SubscribeToChannel(inviteOptions[1]);
             OpenMeeting(inviteOptions[1],cost);
         },()=>
         {
             StopCoroutine(waitforResponse);
             ChatManager.Instance.SendPrivateMessage(sender, "decline");
         });
    }

    IEnumerator SendTimedOut(string sender)
    {
        yield return new WaitForSeconds(10);
        ChatManager.Instance.SendPrivateMessage(sender, "timeout");
        InvitePopup.gameObject.SetActive(false);
    }

    public void OpenGameMode()
    {
        if (!GameManager.Instance.IsTutorialDone)
        {
            MainPanel.HideHeader(true, true);
            return;
        }
        MainPanel.HideHeader(true,false);
        SFXManager.Instance.PlayClip("Open");
        GameModePanel.SetActive(true);
    }

    public void CloseGameMode()
    {
        MainPanel.ShowHeader();
        SFXManager.Instance.PlayClip("Close");
        GameModePanel.SetActive(false);
    }

    public void ShowMain()
    {
        MainPanel.ShowHeader();
    }

    public void HideMain(bool hideGems,bool hideCoins)
    {
        MainPanel.HideHeader(hideGems,hideCoins);
    }

    public void CloseCurrentScene()
    {
        if (CurrentScene != null)
        {
            CurrentScene.gameObject.SetActive(false);
            CurrentScene = null;
        }
    }

    public void OpenPopup(string code, bool hideCoins = true, bool hideGems = true, Action OnOkPressed = null,Action OnCancelPressed = null)
    {
        HideMain(hideGems, hideCoins);
        popup.ShowWithCode(code, ()=>
        {
            OnOkPressed?.Invoke();
            SFXManager.Instance.PlayClip("OK");
        }, ()=>
        {
            OnCancelPressed?.Invoke();
            SFXManager.Instance.PlayClip("Close");
        });
        SFXManager.Instance.PlayClip("Popup");
    }

    //public void OpenPopup(string code,bool hideCoins,bool hideGems, Action OnOkPressed = null, Action OnCancelPressed = null)
    //{
    //    HideMain(hideGems, hideCoins);
    //    OpenPopup(code,OnOkPressed,OnCancelPressed);
    //}
}
