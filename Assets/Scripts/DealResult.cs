using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class DealResult : MonoBehaviour
{
    public Text[] PlayerNames;
    public Text[] PlayerScores;

    Action PanelClosed;

    public void Show(Player[] players,Action OnPanelClosed)
    {
        gameObject.SetActive(true);

        for (int i = 0; i < players.Length; i++)
        {
            PlayerNames[i].text = players[i].Name;
            PlayerScores[i].text = players[i].TotalScore.ToString();
        }

        this.PanelClosed = OnPanelClosed;
    }

    public void Pressed()
    {
        PanelClosed?.Invoke();
        gameObject.SetActive(false);
    }
}
