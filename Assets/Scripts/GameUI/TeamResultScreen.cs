using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamResultScreen : DealResult
{
    public Text Reward;
    void Start()
    {
        int reawrd = GetReward();
        Reward.text = reawrd.ToString();
        GameManager.Instance.AddCurrency(reawrd);
    }

    int GetReward() => rank switch
    {
        0 => GameManager.Instance.Bet * 2,
        1 => GameManager.Instance.Bet * 2,
        _ => 0
    };
}
