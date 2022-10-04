using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialScript : MonoBehaviour
{
    [SerializeField]
    GameObject tutorial;

    [SerializeField]
    GameObject characterPanel;
    [SerializeField]
    GameObject bgBlock;
    [SerializeField]
    TypeText text;

    [SerializeField]
    Button playButton;
    [SerializeField]
    Button todoButton;
    [SerializeField]
    Button taskButton;
    [SerializeField]
    GameObject[] closeToDo;

    [SerializeField]
    Button settingsBtn;
    [SerializeField]
    Button rulesBtn;
    [SerializeField]
    Button closeRulesBtn;
    //[SerializeField]
    //Transform taskPosition;
    [SerializeField]
    Button[] modeButtons;

    //[SerializeField]
    //GameObject gameModePanel;

    [SerializeField]
    Transform parent;

    [SerializeField]
    Transform pointer;
    [SerializeField]
    Transform hand;
    [SerializeField]
    GameObject gameRules;

    [SerializeField]
    GameObject majlisNamePanel;
    [SerializeField]
    InputField majlisNameText;
    [SerializeField]
    GameObject nameError;
    [SerializeField]
    GameObject GameModes;

    //[SerializeField]
    //GameObject[] closeGamePanelBtns;

    int index = 0;
    public static bool IsTutorialDone
    {
        get => PlayerPrefs.GetInt("tutorial", 0) == 1;
        set => PlayerPrefs.SetInt("tutorial", value ? 1 : 0);
    }
    Transform originalParent;

    bool block;

    //int tutorialIndex;
    // Start is called before the first frame update
    void Start()
    {
        //tutorialIndex = PlayerPrefs.GetInt("tutorial", 0);

        if (!IsTutorialDone)
        {
            tutorial.SetActive(true);
            text.Play(LanguageManager.Instance.GetString("tutorial_" + index)
                .Replace("{p}",GameManager.Instance.MyPlayer.Name));
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (block)
                return;

            index++;

            switch (index)
            {
                case 5:
                    majlisNamePanel.SetActive(true);
                    characterPanel.SetActive(false);
                    block = true;
                    return;
                case 8:
                    hand.gameObject.SetActive(true);
                    hand.position = todoButton.transform.position + Vector3.down * 145;
                    characterPanel.SetActive(false);
                    block = true;
                    originalParent = todoButton.transform.parent;

                    todoButton.transform.parent = parent;
                    todoButton.onClick.AddListener(() =>
                    {
                        todoButton.transform.parent = originalParent;

//                        GameModes.SetActive(true);
                        //fix this later
                        todoButton.onClick.RemoveAllListeners();
                        characterPanel.SetActive(true);
                        hand.gameObject.SetActive(false);

                        foreach (var item in closeToDo)
                        {
                            item.SetActive(false);
                        }

                        block = false;
                    });
                    
                    return;
                    //break;
                //case 11:
                //    break;
                case 10:
                    bgBlock.SetActive(false);
                    hand.gameObject.SetActive(true);
                    hand.position = taskButton.transform.position + Vector3.down * 130;

                    characterPanel.SetActive(false);
                    block = true;

                    taskButton.onClick.AddListener(() =>
                    {

                        hand.gameObject.SetActive(false);
                        MajlisScript.Instance.TaskFinished += () =>
                        {
                            block = false;

                            bgBlock.SetActive(true);
                            characterPanel.SetActive(true);
                            text.Play(LanguageManager.Instance.GetString("tutorial_" + index));
                        };
                    });
                    return;
                case 11:
                    foreach (var item in closeToDo)
                    {
                        item.SetActive(true);
                    }
                    //gameObject.SetActive(false);
                    //PlayerPrefs.SetInt("tutorial", 2);
                    //finish tutorial
                    break;
                case 13:
                    hand.gameObject.SetActive(true);
                    hand.position = playButton.transform.position + Vector3.down * 155;
                    characterPanel.SetActive(false);
                    block = true;
                    originalParent = playButton.transform.parent;
                    playButton.transform.parent = parent;
                    playButton.onClick.AddListener(() =>
                    {
                        GameModes.SetActive(true);

                        playButton.transform.parent = originalParent;
                        playButton.onClick.RemoveAllListeners();

                        characterPanel.SetActive(true);

                        hand.gameObject.SetActive(false);
                        block = false;
                    });
                    return;

                case 14:
                    originalParent = modeButtons[0].transform.parent;
                    modeButtons[0].transform.parent = parent;

                    pointer.gameObject.SetActive(true);
                    pointer.position = modeButtons[0].transform.position;
                    break;
                case 15:
                    modeButtons[0].transform.parent = originalParent;
                    originalParent = modeButtons[1].transform.parent;
                    modeButtons[1].transform.parent = parent;

                    pointer.position = modeButtons[1].transform.position;
                    break;
                case 16:
                    modeButtons[1].transform.parent = originalParent;
                    originalParent = modeButtons[2].transform.parent;
                    modeButtons[2].transform.parent = parent;

                    pointer.position = modeButtons[2].transform.position;
                    break;
                case 17:
                    modeButtons[2].transform.parent = originalParent;
                    MenuManager.Instance.ShowMain();
                    GameModes.SetActive(false);
                    pointer.gameObject.SetActive(false);
                    break;
                case 18:
                    originalParent = settingsBtn.transform.parent;
                    settingsBtn.transform.parent = parent;

                    characterPanel.SetActive(false);

                    hand.gameObject.SetActive(true);
                    hand.position = settingsBtn.transform.position + Vector3.down * 150;
                    block = true;

                    settingsBtn.onClick.AddListener(SettingsClicked);
                    return;
                case 20:
                    gameObject.SetActive(false);
                    IsTutorialDone = true;
                    return;
            }
            text.Play(LanguageManager.Instance.GetString("tutorial_" + index)
                .Replace("{m}", GameManager.Instance.MyPlayer.MajlisName));
        }
    }

    public void RulesDone()
    {
        block = false;

        characterPanel.SetActive(true);
        closeRulesBtn.onClick.RemoveListener(RulesDone);
    }

    public void SettingsClicked()
    {
        settingsBtn.transform.parent = originalParent;

        originalParent = rulesBtn.transform.parent;
        rulesBtn.transform.parent = parent;

        //characterPanel.SetActive(false);
        settingsBtn.onClick.RemoveListener(SettingsClicked);

        hand.position = rulesBtn.transform.position + Vector3.down * 150;
        rulesBtn.onClick.AddListener(RulesClicked);
    }

    public void RulesClicked()
    {
        //originalParent = rulesBtn.transform.parent;
        rulesBtn.transform.parent = originalParent;

        rulesBtn.onClick.RemoveListener(RulesClicked);
        closeRulesBtn.onClick.AddListener(RulesDone);
        hand.gameObject.SetActive(false);
    }

    public void SubmitMajlisName()
    {
        if (string.IsNullOrEmpty(majlisNameText.text) || majlisNameText.text.Length > 20)
        {
            nameError.SetActive(true);
            return;
        }
        characterPanel.SetActive(true);
        GameManager.Instance.SetMajlisName(majlisNameText.text);
        majlisNamePanel.SetActive(false);
        block = false;
    }
}
