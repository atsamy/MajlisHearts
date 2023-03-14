using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeaderboardEntry : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI Rank;

    [SerializeField]
    TextMeshProUGUI Name;

    [SerializeField]
    TextMeshProUGUI Points;

    public void Set(int rank,string name,int points)
    {
        Rank.text = rank.ToString();
        Name.text = name;
        Points.text = points.ToString();
    }
}
