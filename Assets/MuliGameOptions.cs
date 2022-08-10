using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class MuliGameOptions : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI prize;

    [SerializeField]
    TextMeshProUGUI entry;

    [SerializeField]
    Button increaseBtn;

    [SerializeField]
    Button decreaseBtn;

    [SerializeField]
    SelectGroup typeGroup;

    public int[] EntryFees;

    Action<int,int> selectAction;

    int entryIndex;

    public void Show(Action<int,int> onSelected)
    {
        entryIndex = PlayerPrefs.GetInt("entryIndex", 0);
        SetText();

        gameObject.SetActive(true);

        selectAction = onSelected;
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
        selectAction?.Invoke(EntryFees[entryIndex], typeGroup.GroupIndex);
    }

}
