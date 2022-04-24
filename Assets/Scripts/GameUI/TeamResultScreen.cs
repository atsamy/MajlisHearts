using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamResultScreen : DealResult
{
    public Text Reward;
    void Start()
    {
        Reward.text = GetReward().ToString();
    }

    int GetReward() => rank switch
    {
        0 => GameManager.Instance.Bet * 2,
        1 => GameManager.Instance.Bet,
        _ => 0
    };
}
