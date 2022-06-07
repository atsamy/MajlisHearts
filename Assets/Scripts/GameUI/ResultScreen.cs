using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ResultPanel : DealResult
{
    int rank;
    public void ShowResult(Player[] players, Action OnPanelClosed)
    {
        int reward = 0;

        if (!GameManager.Instance.IsTeam)
        {
            Player[] orderedPlayers = players.OrderBy(a => a.TotalScore).ToArray();

            for (int i = 0; i < 4; i++)
            {
                rank = i;
                ((PlayerFinalResult)singlePlayers[i]).Set(orderedPlayers[i], GetReward());

                if (orderedPlayers[i] is MainPlayer)
                    reward = GetReward();
            }
        }
        else
        {
            int loserIndex = 0;
            int maxScore = 0;

            int winnerIndex = 0;
            float leastScore = Mathf.Infinity;

            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].TotalScore > maxScore)
                {
                    loserIndex = i;
                    maxScore = players[i].TotalScore;
                }

                if (players[i].TotalScore < leastScore)
                {
                    winnerIndex = i;
                    leastScore = players[i].TotalScore;
                }
            }

            reward = GameManager.Instance.Bet * 2;

            ((PlayerFinalResult)teamPlayers[3]).Set(players[loserIndex], 0,(loserIndex % 2));
            ((PlayerFinalResult)teamPlayers[2]).Set(players[(loserIndex + 2) % 4], 0, (loserIndex % 2));
            
            ((PlayerFinalResult)teamPlayers[0]).Set(players[winnerIndex], reward, (winnerIndex % 2));
            ((PlayerFinalResult)teamPlayers[1]).Set(players[(winnerIndex + 2) % 4], reward, (winnerIndex % 2));

            if (loserIndex == GameScript.Instance.MainPlayerIndex || 
                (loserIndex + 2) % 4 == GameScript.Instance.MainPlayerIndex)
            {
                reward = 0;
            }
        }

        GameManager.Instance.AddCurrency(reward);
        PanelClosed = OnPanelClosed;
    }

    int GetReward() => rank switch
    {
        0 => GameManager.Instance.Bet * 2,
        1 => GameManager.Instance.Bet,
        _ => 0
    };
}
