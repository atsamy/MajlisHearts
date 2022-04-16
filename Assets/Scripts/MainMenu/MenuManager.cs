using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    public Text Currency;
    public Text Level;
    public Text UserName;

    public GameObject EditorPanel;
    public GameObject MainUI;

    public MeetingPanel meetingPanel;

    public Popup InvitePopup;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Currency.text = GameManager.Instance.Currency.ToString();
        Level.text = GameManager.Instance.MyPlayer.Level.ToString();
        UserName.text = GameManager.Instance.MyPlayer.Name;

        GameManager.Instance.OnCurrencyChanged += Instance_OnCurrencyChanged;
    }

    public void StartSingleGame()
    {
        GameManager.Instance.IsMultiGame = false;
        SceneManager.LoadScene(2);
    }

    public void ShowEditorPanel()
    {
        EditorPanel.SetActive(true);
        MainUI.SetActive(false);
    }

    public void BackToMainUI()
    {
        EditorPanel.GetComponent<EditorUI>().CategoryPanel_OnCancel();

        EditorPanel.SetActive(false);
        MainUI.SetActive(true);
    }

    private void Instance_OnCurrencyChanged(int value)
    {
        Currency.text = value.ToString();
    }

    public void OpenMeeting(string roomName,bool isHost)
    {
        meetingPanel.Open(roomName,isHost);
    }

    internal void ShowInvitePopup(string sender, string message)
    {
        InvitePopup.ShowWithMessage(sender + " Invited You To Join in His Majlis",()=>
        {
            string roomName = message.Split(':')[1];

            ChatManager.Instance.SubscribeToChannel(roomName);
            OpenMeeting(roomName,false);
        });
    }
}
