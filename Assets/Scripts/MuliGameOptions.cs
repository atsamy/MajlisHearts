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

    public int[] EntryFees;

    Action<int,int> selectAction;

    int entryIndex;
    int gameType;

    public void OpenMultiGame()
    {
        entryIndex = PlayerPrefs.GetInt("entryIndex", 0);
        SetText();
        gameType = 0;
        startGameText.text = LanguageManager.Instance.GetString("startgame");
        base.Open();
    }

    public void OpenFriendGame(Action<int, int> selectAction)
    {
        entryIndex = PlayerPrefs.GetInt("entryIndex", 0);
        SetText();
        this.gameType = 1;
        startGameText.text = LanguageManager.Instance.GetString("continue");
        this.selectAction = selectAction;
        base.Open();
    }

    private void SetText()
    {
        entry.text = EntryFees[entryIndex].ToString();
        prize.text = (EntryFees[entryIndex] * 2).ToString();

        decreaseBtn.interactable = (entryIndex > 0);
        increaseBtn.interactable = (entryIndex < EntryFees.Length - 1);
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

        if (EntryFees[entryIndex] > GameManager.Instance.Coins)
        {
            MenuManager.Instance.Popup.ShowWithCode("nocoins", () =>
            {
                MenuManager.Instance.OpenStore(0);
            });
            return;
        }

        if (gameType == 0)
        {
            multiPanel.Open(EntryFees[entryIndex], typeGroup.GroupIndex);
        }
        else
        {
            selectAction?.Invoke(EntryFees[entryIndex], typeGroup.GroupIndex);
        }
    }

}
