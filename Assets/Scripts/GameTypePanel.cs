using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameTypePanel : MonoBehaviour
{
    public event Action<BalootGameType> OnGameTypeSelected;

    [SerializeField]
    TextMeshProUGUI hokumText;

    [SerializeField]
    TextMeshProUGUI passText;

    [SerializeField]
    GameObject hokumButton;

    public void Show(int round,int hokumIndex,int playerIndex)
    {
        gameObject.SetActive(true);

        if (hokumIndex != -1)
        {
            if (hokumIndex == playerIndex)
            {
                hokumText.text = "Confirm Hokum";
                passText.text = "Nothing";
            }
            else
            {
                hokumButton.SetActive(false);
            }
        }
        else if (round == 1)
        {
            passText.text = "Nothing";
        }

    }

    private void ResetButtons()
    {
        hokumButton.SetActive(true);
        passText.text = "Pass";
        hokumText.text = "Hokum";
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
    Hukom = 1,
    Ashkal = 2,
    Pass = 3
}
