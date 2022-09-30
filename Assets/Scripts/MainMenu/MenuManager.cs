using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using ArabicSupport;
using System;
using TMPro;
using NiobiumStudios;
using System.Collections;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;
    public CameraHover CameraHover;
    public GameObject GameModePanel;
    public SettingsPanel SettingsPanel;
    public StoreScene StoreScene;
    public MainPanelScript MainPanel;
    public MeetingPanel meetingPanel;
    public InvitePopup InvitePopup;

    public Image timerFill;

    internal void OpenStore(int index)
    {
        StoreScene.Open(index);
    }

    public Popup Popup;

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
    }

    private void ChatManager_OnGotPrivateMessage(string sender, string message)
    {
        if (message.ToString() == "decline")
        {
            meetingPanel.FriendDeclined(sender);
        }
        else if (message.ToString() == "timeout")
        {
            meetingPanel.FriendTimedOut(sender);
        }
        else
        {
            if (sender != GameManager.Instance.MyPlayer.Name)
                ShowInvitePopup(sender, message.ToString());
        }
    }

    void ClaimDailyReward(int day)
    {
        GameManager.Instance.AddCoins(DailyRewards.instance.GetReward(day).reward);
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
        meetingPanel.Open(roomName,entryfee, players);
        MainPanel.HideHeader(true, true);
    }

    public void OpenMeeting(string roomName, int entryfee)
    {
        meetingPanel.Open(roomName, entryfee);
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
}
