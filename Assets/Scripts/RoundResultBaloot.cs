using ExitGames.Client.Photon.StructWrapping;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoundResultBaloot : RoundResult
{
    [SerializeField]
    Image[] Avatars;
    [SerializeField]
    TextMeshProUGUI[] Names;
    [SerializeField]
    TextMeshProUGUI[] roundPoints;
    [SerializeField]
    TextMeshProUGUI[] FloorPoints;
    [SerializeField]
    TextMeshProUGUI[] ProjectPoints;
    [SerializeField]
    TextMeshProUGUI[] totalPoints;
    [SerializeField]
    TextMeshProUGUI[] teamTotalPoints;

    [SerializeField]
    GameObject[] teamCrown;


    public override void ShowRound(GameScriptBase game, bool inGame, bool gameOver, Action<int> OnPanelClosed)
    {
        base.ShowRound(game, inGame, gameOver, OnPanelClosed);

        GameScriptBaloot balootGame = game as GameScriptBaloot;

        for (int i = 0; i < 2; i++)
        {
            roundPoints[i].text = (game.Players[i + 0].Score + game.Players[i + 2].Score).ToString();
            teamTotalPoints[i].text = balootGame.TeamsTotalScore[i].ToString();
            totalPoints[i].text = balootGame.TeamsScore[i].ToString();
            ProjectPoints[i].text = (((PlayerBaloot)game.Players[i + 0]).ProjectScore +
                ((PlayerBaloot)game.Players[i + 2]).ProjectScore).ToString();
            FloorPoints[i].text = balootGame.balootRoundScript.FloorPoints == i ? "10" : "0";
            teamCrown[i].SetActive(balootGame.WinningTeam == i);
        }


        for (int i = 0; i < game.Players.Length; i++)
        {
            Avatars[i].sprite = AvatarManager.Instance.GetAvatarSprite(game.Players[i].Name);
            Names[i].text = ArabicSupport.ArabicFixer.Fix(game.Players[i].Name, false, false);
        }

    }
}
