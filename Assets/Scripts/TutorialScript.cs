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
            text.Play(LanguageManager.Instance.GetString("gametutorial_" + index));
        }
        else if (tutorialIndex == 1)
        {
            tutorial.SetActive(true);
            text.Play(LanguageManager.Instance.GetString("majlistutorial_" + index));
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

            if (tutorialIndex == 0)
            {
                switch (index)
                {
                    case 2:
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

                    case 3:
                        pointer.gameObject.SetActive(true);
                        pointer.position = modeButtons[0].transform.position;
                        break;
                    case 4:
                        pointer.position = modeButtons[1].transform.position;
                        break;
                    case 5:
                        pointer.position = modeButtons[2].transform.position;
                        break;
                    case 6:
                        gameModePanel.SetActive(false);
                        pointer.gameObject.SetActive(false);
                        break;
                    case 7:
                        //gameModePanel.SetActive(true);
                        block = true;
                        gameRules.SetActive(true);
                        characterPanel.SetActive(false);
                        return;
                    case 8:
                        //characterPanel.SetActive(true);
                        break;
                    case 9:
                        characterPanel.SetActive(true);
                        break;
                    case 10:
                        hand.gameObject.SetActive(true);
                        hand.position = modeButtons[2].transform.position + Vector3.left * 50;

                        gameModePanel.SetActive(true);

                        characterPanel.SetActive(false);
                        modeButtons[2].enabled = true;

                        modeButtons[2].onClick.AddListener(() =>
                        {
                            PlayerPrefs.SetInt("tutorial", 1);
                        });

                        return;
                }
                text.Play(LanguageManager.Instance.GetString("gametutorial_" + index));
            }
            else if (tutorialIndex == 1)
            {
                switch (index)
                {
                    case 2:
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

                    case 3:
                        break;
                    case 4:
                        bgBlock.SetActive(false);
                        hand.gameObject.SetActive(true);
                        hand.position = taskButton.transform.position + Vector3.left * 100;

                        characterPanel.SetActive(false);
                        block = true;

                        taskButton.onClick.AddListener(()=>
                        {
                            hand.gameObject.SetActive(false);
                            majlisScript.TaskFinished += () =>
                            {
                                block = false;

                                bgBlock.SetActive(true);
                                characterPanel.SetActive(true);
                                text.Play(LanguageManager.Instance.GetString("majlistutorial_" + index));
                            };
                        });
                        return;
                    case 5:
                        foreach (var item in closeToDo)
                        {
                            item.SetActive(true);
                        }
                        gameObject.SetActive(false);
                        PlayerPrefs.SetInt("tutorial", 2);
                        //finish tutorial
                        return;
                }
                text.Play(LanguageManager.Instance.GetString("majlistutorial_" + index));
            }
        }
    }

    public void RulesDone()
    {
        block = false;

        characterPanel.SetActive(true);
        gameRules.SetActive(false);
    }
}
