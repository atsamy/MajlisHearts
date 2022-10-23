using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using TMPro;

public class DealResult : MonoBehaviour
{
    [SerializeField]
    protected GameObject singlePlayersParent;
    [SerializeField]
    protected GameObject teamPlayersParent;
    //[SerializeField]
    //protected Transform[] singlePlayersPositions;
    [SerializeField]
    protected PlayerDealResult[] singlePlayers;
    [SerializeField]
    protected PlayerDealResult[] teamPlayers;
    [SerializeField]
    TextMeshProUGUI[] ranks;
    [SerializeField]
    GameObject Footer;
    [SerializeField]
    TextMeshProUGUI buttonText;
    protected Action<int> PanelClosed;
    int rank = 0;
    [SerializeField]
    GameObject timerButton;
    [SerializeField]
    Image timerFill;
    [SerializeField]
    TextMeshProUGUI timerText;

    //private void Start()
    //{
    //    StartCoroutine(CountNextRound(5));
    //}

    public void ShowRound(Player[] players, bool inGame,bool gameOver, Action<int> OnPanelClosed)
    {
        gameObject.SetActive(true);

        if (gameOver)
        {
            Footer.SetActive(true);
            buttonText.text = LanguageManager.Instance.GetString("next");
            timerButton.SetActive(false);
        }
        else if (inGame)
        {
            //Footer.SetActive(true);
            //buttonText.text = LanguageManager.Instance.GetString("close");
            timerButton.SetActive(false);
        }
        else
        {
            //Footer.SetActive(false);
            //buttonText.text = LanguageManager.Instance.GetString("nextround");
            timerButton.SetActive(true);
            StartCoroutine(CountNextRound(5));
        }

        bool isTeam = GameManager.Instance.IsTeam;

        singlePlayersParent.SetActive(!isTeam);
        teamPlayersParent.SetActive(isTeam);

        PlayerDealResult[] currentPlayers = isTeam ? teamPlayers : singlePlayers;
        Player[] sortedPlayers = players.OrderBy(a => a.TotalScore).ToArray();

        int rankIndex = 0;
        int score = sortedPlayers[0].TotalScore;

        string[] ranksText = new string[] {"1st","2nd","3rd","4th" };

        if (!isTeam)
        {
            for (int i = 0; i < sortedPlayers.Length; i++)
            {
                currentPlayers[i].Set(sortedPlayers[i], inGame);

                if (sortedPlayers[i].TotalScore > score)
                {
                    rankIndex++;
                    score = sortedPlayers[i].TotalScore;

                }

                if (!inGame)
                {
                    currentPlayers[i].PlayAnimation();
                }

                ranks[i].text = ranksText[rankIndex];

                if (sortedPlayers[i] is MainPlayer)
                {
                    rank = rankIndex;
                }
            }
        }
        else
        {
            int loser = players.ToList().IndexOf(players.First(a => a.TotalScore == players.Max(a => a.TotalScore)));

            currentPlayers[3].Set(players[loser], inGame);
            currentPlayers[2].Set(players[(loser + 2) % 4], inGame);
            currentPlayers[1].Set(players[(loser + 1) % 4], inGame);
            currentPlayers[0].Set(players[(loser + 3) % 4], inGame);
        }

        this.PanelClosed = OnPanelClosed;
    }

    private IEnumerator CountNextRound(int v)
    {
        float timer = v;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            int time = Mathf.CeilToInt(timer);
            timerText.text = time.ToString();

            if (time % 2 == 0)
            {
                timerFill.fillClockwise = false;
                timerFill.fillAmount = (1 - (time - timer));
            }
            else
            {
                timerFill.fillClockwise = true;
                timerFill.fillAmount = time - timer;
            }
            yield return null;
        }

        if (PanelClosed != null)
        {
            gameObject.SetActive(false);
            PanelClosed?.Invoke(rank);
        }
    }

    public void ShowInGame(Player[] players)
    {
        ShowRound(players, true,false, (rank) =>
          {
              gameObject.SetActive(false);
          });
    }

    public void ArrangeSinglePlayers()
    {

    }

    public void Pressed()
    {
        PanelClosed?.Invoke(rank);
        gameObject.SetActive(false);
        GameSFXManager.Instance.PlayClip("Click");
    }
}
