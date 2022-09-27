using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using ArabicSupport;
using System;
using TMPro;
using NiobiumStudios;

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

    internal void OpenStore(int index)
    {
        StoreScene.Open(index);
    }

    public Popup Popup;

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

    public void OpenMeeting(string roomName,int entryfee, bool isHost)
    {
        meetingPanel.Open(roomName,entryfee, isHost);
        MainPanel.HideHeader(false, false);
    }

    internal void ShowInvitePopup(string sender, string message)
    {
        bool isArabic = LanguageManager.Instance.CurrentLanguage == Language.Arabic;

        string invitationMessage = (isArabic ? "" : " <color=green>" + ArabicFixer.Fix(sender) + "</color> ") +
            LanguageManager.Instance.GetString("invitationmessage") +
            (isArabic ? " <color=green>" + ArabicFixer.Fix(sender) + "</color> " : "");
        string[] inviteOptions = message.Split(':');
        int cost = int.Parse(inviteOptions[3]);

        InvitePopup.Show(invitationMessage, cost, () =>
         {
             GameManager.Instance.IsTeam = (inviteOptions[2] == "team");
             ChatManager.Instance.SubscribeToChannel(inviteOptions[1]);
             OpenMeeting(inviteOptions[1],cost, false);
         });
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
}
