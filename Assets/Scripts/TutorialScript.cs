using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    //[SerializeField]
    //Transform taskPosition;
    [SerializeField]
    Button[] modeButtons;

    [SerializeField]
    GameObject gameModePanel;

    [SerializeField]
    Transform parent;

    [SerializeField]
    Transform pointer;
    [SerializeField]
    Transform hand;
    [SerializeField]
    GameObject gameRules;

    [SerializeField]
    GameObject[] closeGamePanelBtns;

    [SerializeField]
    MajlisScript majlisScript;

    int index = 0;

    Transform originalParent;

    bool block;

    int tutorialIndex;
    // Start is called before the first frame update
    void Start()
    {
        tutorialIndex = PlayerPrefs.GetInt("tutorial", 0);

        if (tutorialIndex == 0)
        {
            tutorial.SetActive(true);
            text.Play(LanguageManager.Instance.GetString("tutorial_" + index));
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
                case 4:
                    hand.gameObject.SetActive(true);
                    hand.position = todoButton.transform.position + Vector3.left * 60;
                    characterPanel.SetActive(false);
                    block = true;
                    originalParent = todoButton.transform.parent;

                    todoButton.transform.parent = parent;
                    todoButton.onClick.AddListener(() =>
                    {
                        todoButton.transform.parent = originalParent;

                            //fix this later
                            todoButton.onClick.RemoveAllListeners();
                        characterPanel.SetActive(true);
                        hand.gameObject.SetActive(false);

                        foreach (var item in closeToDo)
                        {
                            item.SetActive(false);
                        }
                            //index++;
                            //print(index);

                            block = false;
                            //MenuManager.Instance.CloseMain();
                            //text.Play(LanguageManager.Instance.GetString("tutorial_" + index));
                        });
                    return;

                case 5:
                    break;
                case 6:
                    bgBlock.SetActive(false);
                    hand.gameObject.SetActive(true);
                    hand.position = taskButton.transform.position + Vector3.left * 100;

                    characterPanel.SetActive(false);
                    block = true;

                    taskButton.onClick.AddListener(() =>
                    {
                        hand.gameObject.SetActive(false);
                        majlisScript.TaskFinished += () =>
                        {
                            block = false;

                            bgBlock.SetActive(true);
                            characterPanel.SetActive(true);
                            text.Play(LanguageManager.Instance.GetString("tutorial_" + index));
                        };
                    });
                    return;
                case 7:
                    foreach (var item in closeToDo)
                    {
                        item.SetActive(true);
                    }
                    //gameObject.SetActive(false);
                    //PlayerPrefs.SetInt("tutorial", 2);
                    //finish tutorial
                    break;
                case 8:
                    hand.gameObject.SetActive(true);
                    hand.position = playButton.transform.position + Vector3.left * 60;
                    characterPanel.SetActive(false);
                    block = true;
                    originalParent = playButton.transform.parent;
                    playButton.transform.parent = parent;
                    playButton.onClick.AddListener(() =>
                    {
                        playButton.transform.parent = originalParent;
                        gameModePanel.transform.parent = parent;
                        playButton.onClick.RemoveAllListeners();

                        characterPanel.SetActive(true);

                        hand.gameObject.SetActive(false);

                        foreach (var item in modeButtons)
                        {
                            item.enabled = false;
                        }

                        foreach (var item in closeGamePanelBtns)
                        {
                            item.SetActive(false);
                        }
                            //index++;
                            //print(index);

                            block = false;

                            //text.Play(LanguageManager.Instance.GetString("tutorial_" + index));
                        });
                    return;

                case 10:
                    pointer.gameObject.SetActive(true);
                    pointer.position = modeButtons[0].transform.position;
                    break;
                case 11:
                    pointer.position = modeButtons[1].transform.position;
                    break;
                case 12:
                    pointer.position = modeButtons[2].transform.position;
                    break;
                case 13:
                    gameModePanel.SetActive(false);
                    pointer.gameObject.SetActive(false);
                    break;
                case 14:
                    //gameModePanel.SetActive(true);
                    block = true;
                    gameRules.SetActive(true);
                    characterPanel.SetActive(false);
                    return;
                case 15:
                    characterPanel.SetActive(true);
                    // show pointer to settings
                    break;
                case 16:
                    gameObject.SetActive(false);
                    //hand.gameObject.SetActive(true);
                    //hand.position = modeButtons[2].transform.position + Vector3.left * 50;

                    //gameModePanel.SetActive(true);

                    //characterPanel.SetActive(false);
                    //modeButtons[2].enabled = true;

                    //modeButtons[2].onClick.AddListener(() =>
                    //{
                    //    //PlayerPrefs.SetInt("tutorial", 1);
                    //});

                    return;
            }
            text.Play(LanguageManager.Instance.GetString("tutorial_" + index));
        }
    }

    public void RulesDone()
    {
        block = false;

        characterPanel.SetActive(true);
        gameRules.SetActive(false);
    }
}
