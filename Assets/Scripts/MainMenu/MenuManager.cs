using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using ArabicSupport;
using System;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;
    public CameraHover CameraHover;
    public GameObject GameModePanel;
    //public GameObject EditorPanel;
    public GameObject MainUI;
    //public AvatarPanel AvatarPanel;
    public SettingsPanel SettingsPanel;
    public StoreScene StoreScene;
    public HeaderScript Header;
    public MeetingPanel meetingPanel;
    public Popup InvitePopup;

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
            Header.SetAvatar();
        }

        TaskData[] taskDatas = new TaskData[2];

        taskDatas[0] = new TaskData()
        {
            ID = "clean room",
            ActionType = TaskAction.Clean,
            Cost = 30,
            Target = "LivingCouch"
        };

        taskDatas[1] = new TaskData()
        {
            ID = "change couch",
            ActionType = TaskAction.Clean,
            Cost = 40,
            Target = "LivingRoom"
        };

        Wrapper<TaskData> data = new Wrapper<TaskData>();
        data.array = taskDatas;

        print(JsonUtility.ToJson(data));
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
        MainUI.SetActive(false);
        SettingsPanel.Open();
    }

    public void OpenMeeting(string roomName, bool isHost)
    {
        meetingPanel.Open(roomName, isHost);
        MainUI.SetActive(false);
    }

    internal void ShowInvitePopup(string sender, string message)
    {
        bool isArabic = LanguageManager.Instance.CurrentLanguage == Language.Arabic;

        string invitationMessage = (isArabic ? "" : " <color=green>" + ArabicFixer.Fix(sender) + "</color> ") +
            LanguageManager.Instance.GetString("invitationmessage") +
            (isArabic ? " <color=green>" + ArabicFixer.Fix(sender) + "</color> " : "");

        InvitePopup.ShowWithMessage(invitationMessage, () =>
         {
             string[] inviteOptions = message.Split(':');

             GameManager.Instance.IsTeam = (inviteOptions[2] == "team");
             ChatManager.Instance.SubscribeToChannel(inviteOptions[1]);
             OpenMeeting(inviteOptions[1], false);
         });
    }

    public void OpenGameMode()
    {
        MainUI.SetActive(false);
        SFXManager.Instance.PlayClip("Open");
        GameModePanel.SetActive(true);
    }

    public void CloseGameMode()
    {
        MainUI.SetActive(true);
        SFXManager.Instance.PlayClip("Close");
        GameModePanel.SetActive(false);
    }

    public void OpenMain()
    {
        MainUI.SetActive(true);
    }

    public void CloseMain()
    {
        MainUI.SetActive(false);
    }
}
