using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoundResultBaloot : RoundResult
{
    [SerializeField]
    TextMeshProUGUI roundTeam1;
    [SerializeField]
    TextMeshProUGUI roundTeam2;
    [SerializeField]
    TextMeshProUGUI pointsTeam1;
    [SerializeField]
    TextMeshProUGUI pointsTeam2;
    [SerializeField]
    TextMeshProUGUI totalPointsTeam1;
    [SerializeField]
    TextMeshProUGUI totalPointsTeam2;

    public override void ShowRound(GameScriptBase game, bool inGame, bool gameOver, Action<int> OnPanelClosed)
    {
        base.ShowRound(game, inGame, gameOver, OnPanelClosed);

        roundTeam1.text = (game.Players[0].Score + game.Players[2].Score).ToString();
        roundTeam2.text = (game.Players[1].Score + game.Players[3].Score).ToString();

        totalPointsTeam1.text = ((GameScriptBaloot)game).TeamsTotalScore[0].ToString();
        totalPointsTeam2.text = ((GameScriptBaloot)game).TeamsTotalScore[1].ToString();

        pointsTeam1.text = ((GameScriptBaloot)game).TeamsScore[0].ToString();
        pointsTeam2.text = ((GameScriptBaloot)game).TeamsScore[1].ToString();
    }
}
