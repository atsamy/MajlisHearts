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
    TextMeshProUGUI projectsTeam1;
    [SerializeField]
    TextMeshProUGUI projectsTeam2;
    [SerializeField]
    TextMeshProUGUI totalPointsTeam1;
    [SerializeField]
    TextMeshProUGUI totalPointsTeam2;

    [SerializeField]
    GameObject teamOneCrown;
    [SerializeField]
    GameObject teamTwoCrown;

    public override void ShowRound(GameScriptBase game, bool inGame, bool gameOver, Action<int> OnPanelClosed)
    {
        base.ShowRound(game, inGame, gameOver, OnPanelClosed);

        GameScriptBaloot balootGame = game as GameScriptBaloot;

        roundTeam1.text = (game.Players[0].Score + game.Players[2].Score).ToString();
        roundTeam2.text = (game.Players[1].Score + game.Players[3].Score).ToString();

        totalPointsTeam1.text = balootGame.TeamsTotalScore[0].ToString();
        totalPointsTeam2.text = balootGame.TeamsTotalScore[1].ToString();

        pointsTeam1.text = balootGame.TeamsScore[0].ToString();
        pointsTeam2.text = balootGame.TeamsScore[1].ToString();

        projectsTeam1.text = (((PlayerBaloot)game.Players[0]).ProjectScore + 
            ((PlayerBaloot)game.Players[2]).ProjectScore).ToString();

        projectsTeam2.text = (((PlayerBaloot)game.Players[1]).ProjectScore +
            ((PlayerBaloot)game.Players[3]).ProjectScore).ToString();

        teamOneCrown.SetActive(balootGame.WinningTeam == 0);
        teamTwoCrown.SetActive(balootGame.WinningTeam == 1);
    }
}
