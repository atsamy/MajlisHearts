//using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class LeaderboardPanel : MenuScene
{
    [SerializeField]
    GameObject leaderBoardEntry;

    [SerializeField]
    Transform content;

    private void Awake()
    {
        PlayfabManager.instance.GetHeartsLeaderboard((entries) => 
        {
            foreach (var entry in entries) 
            {
                LeaderboardEntry newEntry = Instantiate(leaderBoardEntry, content).GetComponent<LeaderboardEntry>();
                newEntry.Set(entry.Position + 1,entry.DisplayName,entry.StatValue);
            }
        });
    }
}
