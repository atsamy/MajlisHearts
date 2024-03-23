using UnityEngine;
using UnityEngine.UI;

public class LeaderboardPanel : MenuScene
{
    [SerializeField]
    GameObject leaderBoardEntry;

    [SerializeField]
    Button[] tabsButtons;

    [SerializeField]
    RectTransform[] Heartcontents;

    [SerializeField]
    RectTransform[] Balootcontents;

    RectTransform[] currentcontents;

    [SerializeField]
    ScrollRect scrollRect;

    [SerializeField]
    GameObject balootParent;

    [SerializeField]
    GameObject heartParent;

    [SerializeField]
    Button heartBtn;

    [SerializeField]
    Button balootBtn;

    private void Awake()
    {
        GetGameData("HeartsPoints",Heartcontents);
        GetGameData("BalootPoints", Balootcontents);

        currentcontents = Heartcontents;
    }

    public void GetGameData(string gameName,Transform[] content)
    {
        PlayfabManager.instance.GetLeaderboard(gameName,(entries) =>
        {
            foreach (var entry in entries)
            {
                LeaderboardEntry newEntry = Instantiate(leaderBoardEntry, content[0]).GetComponent<LeaderboardEntry>();
                newEntry.Set(entry.Position + 1, entry.Profile.DisplayName, entry.StatValue, entry.Profile.AvatarUrl);
            }
        });

        PlayfabManager.instance.GetFriendsLeaderboard(gameName,(entries) =>
        {
            foreach (var entry in entries)
            {
                LeaderboardEntry newEntry = Instantiate(leaderBoardEntry, content[2]).GetComponent<LeaderboardEntry>();
                newEntry.Set(entry.Position + 1, entry.DisplayName, entry.StatValue, entry.Profile.AvatarUrl);
            }
        });

        PlayfabManager.instance.GetLeaderboardAroundPlayer(gameName,(entries) =>
        {
            foreach (var entry in entries)
            {
                LeaderboardEntry newEntry = Instantiate(leaderBoardEntry, content[1]).GetComponent<LeaderboardEntry>();
                newEntry.Set(entry.Position + 1, entry.DisplayName, entry.StatValue, entry.Profile.AvatarUrl);
            }
        });
    }

    public void SwitchTab(int index)
    {
        for (int i = 0; i < tabsButtons.Length; i++)
        {
            tabsButtons[i].interactable = (i != index);
            Balootcontents[i].gameObject.SetActive(i == index);
            Heartcontents[i].gameObject.SetActive(i == index);
        }

        scrollRect.content = currentcontents[index];
    }

    public void OpenBaloot()
    {
        currentcontents = Balootcontents;
        balootParent.SetActive(true);
        heartParent.SetActive(false);

        heartBtn.interactable = true;
        balootBtn.interactable = false;
    }

    public void OpenHearts()
    {
        currentcontents = Heartcontents;
        balootParent.SetActive(false);
        heartParent.SetActive(true);

        heartBtn.interactable = false;
        balootBtn.interactable = true;
    }

    public override void Close()
    {
        MenuManager.Instance.ShowMain();
        base.Close();
    }
}
