using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;


public class DealResult : MonoBehaviour
{
    [SerializeField]
    protected GameObject singlePlayersParent;
    [SerializeField]
    protected GameObject teamPlayersParent;
    [SerializeField]
    protected Transform[] singlePlayersPositions;
    [SerializeField]
    protected PlayerDealResult[] singlePlayers;
    [SerializeField]
    protected PlayerDealResult[] teamPlayers;
    [SerializeField]
    GameObject Footer;
    [SerializeField]
    Text buttonText;
    protected Action<int> PanelClosed;
    int rank = 0;

    public void ShowRound(Player[] players, bool inGame,bool gameOver, Action<int> OnPanelClosed)
    {
        if (gameOver)
        {
            buttonText.text = LanguageManager.Instance.GetString("next");
        }
        else if (inGame)
        {
            buttonText.text = LanguageManager.Instance.GetString("close");
        }
        else
        {
            buttonText.text = LanguageManager.Instance.GetString("nextround");
        }

        bool isTeam = GameManager.Instance.IsTeam;

        gameObject.SetActive(true);

        singlePlayersParent.SetActive(!isTeam);
        teamPlayersParent.SetActive(isTeam);

        PlayerDealResult[] currentPlayers = isTeam ? teamPlayers : singlePlayers;

        //float leastScore = Mathf.Infinity;
        //int highestScore = 0;
        //int winnerIndex = 0;
        //int loserIndex = 0;

        Player[] sortedPlayers = players.OrderBy(a => a.TotalScore).ToArray();

        for (int i = 0; i < sortedPlayers.Length; i++)
        {
            if (sortedPlayers[i] is MainPlayer)
                rank = i;
        }

        if (!isTeam)
        {
            for (int i = 0; i < sortedPlayers.Length; i++)
            {
                currentPlayers[i].Set(sortedPlayers[i], inGame);
                currentPlayers[i].SetWinner(false);
            }

            currentPlayers[0].SetWinner(true);
        }
        else
        {
            int loser = players.ToList().IndexOf(players.First(a => a.TotalScore == players.Max(a => a.TotalScore)));

            currentPlayers[3].Set(players[loser], inGame);
            currentPlayers[2].Set(players[(loser + 2) % 4], inGame);
            currentPlayers[1].Set(players[(loser + 1) % 4], inGame);
            currentPlayers[0].Set(players[(loser + 3) % 4], inGame);

            currentPlayers[0].SetWinner(true);
            currentPlayers[1].SetWinner(true);
            //for (int i = 0; i < players.Length; i++)
            //{
            //    currentPlayers[i].Set(players[i].Name, players[i].Avatar);
            //    currentPlayers[i].SetScore(inGame, players[i].Score, players[i].TotalScore);
            //    currentPlayers[i].SetWinner(false);

            //    if (players[i].TotalScore < leastScore)
            //    {
            //        leastScore = players[i].TotalScore;
            //        winnerIndex = i;
            //    }

            //    if (highestScore < players[i].TotalScore)
            //    {
            //        highestScore = players[i].TotalScore;
            //        loserIndex = i;
            //    }
            //}


        }

        this.PanelClosed = OnPanelClosed;
        Footer.SetActive(OnPanelClosed != null);
    }

    public void ShowInGame(Player[] players)
    {
        ShowRound(players, true,false, (rank) =>
          {
              gameObject.SetActive(false);
          });
    }

    public void ArrangeSinglePlayers()
    {

    }

    public void Pressed()
    {
        PanelClosed?.Invoke(rank);
        gameObject.SetActive(false);
        GameSFXManager.Instance.PlayClip("Click");
    }
}
