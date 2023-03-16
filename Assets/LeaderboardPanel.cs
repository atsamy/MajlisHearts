//using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardPanel : MenuScene
{
    [SerializeField]
    GameObject leaderBoardEntry;

    [SerializeField]
    Button[] tabsButtons;

    [SerializeField]
    Transform[] contents;

    private void Awake()
    {
        PlayfabManager.instance.GetHeartsLeaderboard((entries) => 
        {
            foreach (var entry in entries) 
            {
                LeaderboardEntry newEntry = Instantiate(leaderBoardEntry, contents[0]).GetComponent<LeaderboardEntry>();
                newEntry.Set(entry.Position + 1,entry.DisplayName,entry.StatValue,entry.Profile.AvatarUrl);
            }
        });

        PlayfabManager.instance.GetHeartsFriendsLeaderboard((entries) =>
        {
            foreach (var entry in entries)
            {
                LeaderboardEntry newEntry = Instantiate(leaderBoardEntry, contents[2]).GetComponent<LeaderboardEntry>();
                newEntry.Set(entry.Position + 1, entry.DisplayName, entry.StatValue, entry.Profile.AvatarUrl);
            }
        });

        PlayfabManager.instance.GetHeartsLeaderboardAroundPlayer((entries) =>
        {
            foreach (var entry in entries)
            {
                LeaderboardEntry newEntry = Instantiate(leaderBoardEntry, contents[1]).GetComponent<LeaderboardEntry>();
                newEntry.Set(entry.Position + 1, entry.DisplayName, entry.StatValue, entry.Profile.AvatarUrl);
            }
        });
    }

    public void SwitchTab(int index)
    {
        for (int i = 0; i < tabsButtons.Length; i++)
        {
            tabsButtons[i].interactable = (i != index);
            contents[i].gameObject.SetActive(i == index);
        }
    }

    public override void Close()
    {
        MenuManager.Instance.ShowMain();
        base.Close();
    }
}
