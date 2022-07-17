using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerFinalResult : PlayerDealResult
{
    [SerializeField]
    TextMeshProUGUI Reward;

    public void Set(Player player,int reward)
    {
        Set(name, player.Avatar);
        SetScore(player.TotalScore);
        Reward.text = reward.ToString();
    }

    public void Set(Player player, int reward, int team)
    {
        Set(player,reward);
        ShowTeamBadge(team);
    }
}
