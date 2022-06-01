using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class DealResult : MonoBehaviour
{
    [SerializeField]
    GameObject singlePlayersParent;
    [SerializeField]
    GameObject teamPlayersParent;
    [SerializeField]
    PlayerDealResult[] singlePlayers;
    [SerializeField]
    PlayerDealResult[] teamPlayers;
    [SerializeField]
    GameObject StartButton;

    protected int rank;
    Action PanelClosed;

    public void Show(Player[] players, Action OnPanelClosed)
    {
        bool isTeam = GameManager.Instance.IsTeam;

        gameObject.SetActive(true);

        singlePlayersParent.SetActive(!isTeam);
        teamPlayersParent.SetActive(isTeam);

        PlayerDealResult[] currentPlayers = isTeam ? teamPlayers : singlePlayers;

        float leastScore = Mathf.Infinity;
        int winnerIndex = 0;

        for (int i = 0; i < players.Length; i++)
        {
            currentPlayers[i].Set(players[i].Name,"");
            currentPlayers[i].SetScore(players[i].TotalScore);

            if (players[i].TotalScore < leastScore)
                winnerIndex = i;

            currentPlayers[winnerIndex].SetWinner(false);
        }

        currentPlayers[winnerIndex].SetWinner(true);

        this.PanelClosed = OnPanelClosed;
        StartButton.SetActive(OnPanelClosed != null);
    }

    public void Pressed()
    {
        PanelClosed?.Invoke();
        gameObject.SetActive(false);
    }
}
