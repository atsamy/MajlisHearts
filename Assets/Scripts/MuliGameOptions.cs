using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class MuliGameOptions : MenuScene
{
    [SerializeField]
    TextMeshProUGUI prize;
    [SerializeField]
    TextMeshProUGUI entry;
    [SerializeField]
    TextMeshProUGUI startGameText;
    [SerializeField]
    Button increaseBtn;
    [SerializeField]
    Button decreaseBtn;
    [SerializeField]
    SelectGroup typeGroup;
    [SerializeField]
    MultiPanel multiPanel;

    [SerializeField]
    TextMeshProUGUI[] entryFeesText;

    [SerializeField]
    Button[] difficultyButtons;

    Action<int,int> selectAction;

    int entryIndex;
    int gameType;

    int[] entryFees;

    private void Awake()
    {
        entryFees = GameManager.Instance.GameData.EntryFee;

        for (int i = 0; i < entryFees.Length; i++)
        {
            entryFeesText[i].text = entryFees[i].ToString();
        }

        entryIndex = PlayerPrefs.GetInt("gameSelection", 0);

        for (int i = 0; i < difficultyButtons.Length; i++)
        {
            difficultyButtons[i].interactable = (i != entryIndex);
        }
    }

    public void SelectGame(int index)
    {
        entryIndex = index;
        PlayerPrefs.SetInt("gameSelection", entryIndex);

        for (int i = 0; i < difficultyButtons.Length; i++)
        {
            difficultyButtons[i].interactable = (i != entryIndex);
        }

        SFXManager.Instance.PlayClip("Toggle");
    }

    public void OpenMultiGame()
    {
        //entryIndex = PlayerPrefs.GetInt("entryIndex", 0);
        //SetText();
        gameType = 0;
        startGameText.text = LanguageManager.Instance.GetString("startgame");
        base.Open();
    }

    public void OpenFriendGame(Action<int, int> selectAction)
    {
        //entryIndex = PlayerPrefs.GetInt("entryIndex", 0);
        //SetText();
        this.gameType = 1;
        startGameText.text = LanguageManager.Instance.GetString("continue");
        this.selectAction = selectAction;
        base.Open();
    }

    private void SetText()
    {
        entry.text = entryFees[entryIndex].ToString();
        prize.text = (entryFees[entryIndex] * 2).ToString();

        decreaseBtn.interactable = (entryIndex > 0);
        increaseBtn.interactable = (entryIndex < entryFees.Length - 1);
    }

    public void IncreaeEntry()
    {
        entryIndex++;
        PlayerPrefs.SetInt("entryIndex", entryIndex);

        SetText();
    }

    public void DecreaseEntry()
    {
        entryIndex--;
        PlayerPrefs.SetInt("entryIndex", entryIndex);

        SetText();
    }

    public void StartGame()
    {
        gameObject.SetActive(false);
        MenuManager.Instance.HideMain(true, true);

        if (entryFees[entryIndex] > GameManager.Instance.Coins)
        {
            MenuManager.Instance.OpenPopup("nocoins",false,true, () =>
            {
                MenuManager.Instance.OpenStore(0);
            }, () =>
            {
                MenuManager.Instance.ShowMain();
            });
            return;
        }

        if (gameType == 0)
        {
            multiPanel.Open(entryFees[entryIndex], typeGroup.GroupIndex);
        }
        else
        {
            selectAction?.Invoke(entryFees[entryIndex], typeGroup.GroupIndex);
        }
    }

}
