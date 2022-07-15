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
    [SerializeField]
    protected PlayerDealResult[] singlePlayers;
    [SerializeField]
    protected PlayerDealResult[] teamPlayers;
    [SerializeField]
    GameObject Footer;
    [SerializeField]
    TextMeshProUGUI buttonText;
    protected Action PanelClosed;

    public void ShowRound(Player[] players, bool inGame, Action OnPanelClosed)
    {
        buttonText.text = inGame ? LanguageManager.Instance.GetString("close") : LanguageManager.Instance.GetString("nextround");

        bool isTeam = GameManager.Instance.IsTeam;

        gameObject.SetActive(true);

        singlePlayersParent.SetActive(!isTeam);
        teamPlayersParent.SetActive(isTeam);

        PlayerDealResult[] currentPlayers = isTeam ? teamPlayers : singlePlayers;

        float leastScore = Mathf.Infinity;
        int highestScore = 0;
        int winnerIndex = 0;
        int loserIndex = 0;

        for (int i = 0; i < players.Length; i++)
        {
            currentPlayers[i].Set(players[i].Name, players[i].Avatar);
            currentPlayers[i].SetScore(inGame ? players[i].Score : players[i].TotalScore);
            currentPlayers[i].SetWinner(false);

            if (players[i].TotalScore < leastScore)
            {
                leastScore = players[i].TotalScore;
                winnerIndex = i;
            }

            if (highestScore < players[i].TotalScore)
            {
                highestScore = players[i].TotalScore;
                loserIndex = i;
            }
        }

        if (!isTeam)
        {
            currentPlayers[winnerIndex].SetWinner(true);
        }
        else
        {
            currentPlayers[(loserIndex + 1) % 4].SetWinner(true);
            currentPlayers[(loserIndex + 3) % 4].SetWinner(true);
        }

        this.PanelClosed = OnPanelClosed;
        Footer.SetActive(OnPanelClosed != null);
    }

    public void ShowInGame(Player[] players)
    {
        ShowRound(players, true, () =>
          {
              gameObject.SetActive(false);
          });
    }

    public void Pressed()
    {
        PanelClosed?.Invoke();
        gameObject.SetActive(false);
        GameSFXManager.Instance.PlayClip("Click");
    }
}
