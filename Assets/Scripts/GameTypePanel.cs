using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameTypePanel : MonoBehaviour
{
    public event Action<BalootGameType> OnGameTypeSelected;
    public event Action OnOtherHokumSelected;

    [SerializeField]
    TextMeshProUGUI hokumText;

    [SerializeField]
    TextMeshProUGUI passText;

    [SerializeField]
    Button ashkalBtn;

    [SerializeField]
    GameObject hokumButton;
    [SerializeField]
    Button otherHokumButton;

    private void Awake()
    {
        otherHokumButton.onClick.AddListener(() => 
        {
            gameObject.SetActive(false);
            OnOtherHokumSelected?.Invoke(); 
        });
    }
    public void Show(int round,int hokumIndex,int playerIndex, int startIndex)
    {
        gameObject.SetActive(true);
        if (hokumIndex != -1)
        {
            if (hokumIndex == playerIndex)
            {
                hokumText.text = LanguageManager.Instance.GetString("confirmhokum");
                passText.text = LanguageManager.Instance.GetString("nothing");

                hokumButton.SetActive(true);
                otherHokumButton.gameObject.SetActive(false);
            }
            else
            {
                hokumButton.SetActive(false);
                if (round == 1)
                {
                    passText.text = LanguageManager.Instance.GetString("nothing");
                }
            }
        }
        else if (round == 1)
        {
            passText.text = LanguageManager.Instance.GetString("nothing");
            hokumText.text = LanguageManager.Instance.GetString("otherHokum");
            hokumButton.SetActive(false);
            otherHokumButton.gameObject.SetActive(true);
        }

        int leftPlayer = startIndex - 1;
        leftPlayer = leftPlayer < 0 ? leftPlayer + 4 : leftPlayer;

        ashkalBtn.interactable = (startIndex == playerIndex || leftPlayer == playerIndex);
    }

    private void ResetButtons()
    {
        hokumButton.gameObject.SetActive(true);
        passText.text = LanguageManager.Instance.GetString("pass");
        hokumText.text = LanguageManager.Instance.GetString("Hokum");

        hokumButton.SetActive(true);
        otherHokumButton.gameObject.SetActive(false);
    }

    public void ChooseType(int type)
    {
        OnGameTypeSelected?.Invoke((BalootGameType)type);
        gameObject.SetActive(false);

        ResetButtons();
    }
}

public enum BalootGameType
{
    Sun = 0,
    Hokum = 1,
    Ashkal = 2,
    Pass = 3
}
