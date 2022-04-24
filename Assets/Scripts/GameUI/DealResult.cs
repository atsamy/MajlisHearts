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
    public GameObject StartButton;


    protected int rank;
    Action PanelClosed;

    public void Show(Player[] players, Action OnPanelClosed)
    {
        gameObject.SetActive(true);

        for (int i = 0; i < players.Length; i++)
        {
            PlayerNames[i].text = players[i].Name;
            PlayerScores[i].text = players[i].TotalScore.ToString();

            if (players[i].Name == GameManager.Instance.MyPlayer.Name)
                rank = i;
        }


        this.PanelClosed = OnPanelClosed;

        StartButton.SetActive(OnPanelClosed != null);
    }

    public void Pressed()
    {
        PanelClosed?.Invoke();
        gameObject.SetActive(false);
    }
}
