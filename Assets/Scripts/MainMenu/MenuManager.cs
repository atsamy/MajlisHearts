using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using ArabicSupport;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    public ChangeNumber Currency;
    public Text Level;
    public Text UserName;

    public GameObject EditorPanel;
    public GameObject MainUI;
    public AvatarPanel AvatarPanel;
    public SettingsPanel SettingsPanel;

    public MeetingPanel meetingPanel;
    public Popup InvitePopup;
    public Image Avatar;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Currency.setNumber(GameManager.Instance.Currency);
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
        GameManager.Instance.GameType = GameType.Single;
        SceneManager.LoadScene(2);
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
        Currency.Change(value);
    }

    public void OpenSettings()
    {
        SettingsPanel.Open();
    }

    public void OpenMeeting(string roomName, bool isHost)
    {
        meetingPanel.Open(roomName, isHost);
        MainUI.SetActive(false);
    }

    internal void ShowInvitePopup(string sender, string message)
    {
        InvitePopup.ShowWithMessage(sender + " Invited You To Join in His Majlis", () =>
         {
             string[] inviteOptions = message.Split(':');

             GameManager.Instance.IsTeam = (inviteOptions[2] == "team");
             ChatManager.Instance.SubscribeToChannel(inviteOptions[1]);
             OpenMeeting(inviteOptions[1], false);
         });
    }
}
