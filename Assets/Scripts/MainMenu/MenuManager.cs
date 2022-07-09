using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using ArabicSupport;
using TMPro;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    public TextMeshProUGUI Currency;
    public Text Level;
    public TextMeshProUGUI UserName;

    public GameObject GameModePanel;
    public GameObject EditorPanel;
    public GameObject MainUI;
    public AvatarPanel AvatarPanel;
    public SettingsPanel SettingsPanel;

    public MeetingPanel meetingPanel;
    public Popup InvitePopup;
    public Image Avatar;

    public Popup Popup;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Currency.text = (GameManager.Instance.Currency.ToString());
        Level.text = GameManager.Instance.MyPlayer.Level.ToString();
        UserName.text = ArabicFixer.Fix(GameManager.Instance.MyPlayer.Name);

        GameManager.Instance.OnCurrencyChanged += Instance_OnCurrencyChanged;

        if (string.IsNullOrEmpty(GameManager.Instance.MyPlayer.Avatar))
        {
            OpenAvatarPanel();
        }
        else
        {
            Avatar.sprite = Resources.Load<Sprite>("Avatar/Face/" + GameManager.Instance.MyPlayer.Avatar);
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

    public void ShowEditorPanel()
    {
        EditorPanel.SetActive(true);
        MainUI.SetActive(false);
    }

    public void OpenAvatarPanel()
    {
        AvatarPanel.Open((index) =>
        {
            GameManager.Instance.MyPlayer.Avatar = "Avatar" + index;
            Avatar.sprite = Resources.Load<Sprite>("Avatar/Face/Avatar" + index);
        });
    }

    public void BackToMainUI()
    {
        EditorPanel.GetComponent<EditorUI>().CategoryPanel_OnCancel();

        EditorPanel.SetActive(false);
        MainUI.SetActive(true);
    }

    private void Instance_OnCurrencyChanged(int value)
    {
        Currency.text = (value.ToString());
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
        InvitePopup.ShowWithMessage( "<color=green>" + sender + "</color> Invited You To Join in His Majlis", () =>
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
