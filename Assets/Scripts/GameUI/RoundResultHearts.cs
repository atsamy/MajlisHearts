using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class RoundResultHearts : RoundResult
{
    [SerializeField]
    PlayerDealResult[] singlePlayers;
    [SerializeField]
    TextMeshProUGUI[] ranks;
    public override void ShowRound(GameScriptBase game, bool inGame, bool gameOver, Action<int> OnPanelClosed)
    {
        base.ShowRound(game,inGame,gameOver, OnPanelClosed);

        PlayerDealResult[] currentPlayers = singlePlayers;
        PlayerBase[] sortedPlayers = game.Players.OrderBy(a => a.TotalScore).ToArray();

        int rankIndex = 0;
        int score = sortedPlayers[0].TotalScore;

        string[] ranksText = new string[] { "1st", "2nd", "3rd", "4th" };


        for (int i = 0; i < sortedPlayers.Length; i++)
        {
            currentPlayers[i].Set(sortedPlayers[i], inGame);

            if (sortedPlayers[i].TotalScore > score)
            {
                rankIndex++;
                score = sortedPlayers[i].TotalScore;

            }

            if (!inGame)
            {
                currentPlayers[i].PlayAnimation();
            }

            ranks[i].text = ranksText[rankIndex];

            if (sortedPlayers[i] is MainPlayer)
            {
                rank = rankIndex;
            }
        }
    }
}
