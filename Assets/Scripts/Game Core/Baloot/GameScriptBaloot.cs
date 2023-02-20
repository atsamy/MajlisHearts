using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScriptBaloot : GameScriptBase
{
    public static GameScriptBaloot Instance;
    BalootMainPlayer myPlayer => (BalootMainPlayer)Players[0];

    public delegate void StartCardsReady(Card balootCard);
    public event StartCardsReady OnStartCardsReady;

    public delegate void PlayerSelectedType(int index, BalootGameType type);
    public event PlayerSelectedType OnPlayerSelectedType;

    public delegate void RestartDeal();
    public event RestartDeal OnRestartDeal;

    public RoundScriptBaloot balootRoundScript => (RoundScriptBaloot)RoundScript;

    [HideInInspector]
    public int[] TeamsScore;

    [HideInInspector]
    public int[] TeamsTotalScore;

    private void Awake()
    {
        Instance = this;
        RoundScript = new RoundScriptBaloot();
        TeamsScore = new int[2];
        TeamsTotalScore= new int[2];
    }

    void Start()
    {
        balootRoundScript.OnEvent += Deal_OnEvent;

        Players = new BalootPlayer[4];

        for (int i = 0; i < 4; i++)
        {
            if (i == 0)
            {
                Players[i] = new BalootMainPlayer(i);
                //Players[i].Avatar = AvatarManager.Instance.playerAvatar;
                Players[i].Name = GameManager.Instance.MyPlayer.Name;
            }
            else
            {
                Players[i] = new BalootAIPlayer(i);
                //Players[i].Avatar = AvatarManager.Instance.RobotAvatar;
                Players[i].Name = "Player " + i;
            }

            ((BalootPlayer)Players[i]).OnTypeSelected += Players_SelectedType;
            Players[i].OnCardReady += GameScript_OnCardReady;

        }


        RoundScript.SetPlayers(Players);

        //SetEnvironment(GameManager.Instance.EquippedItem["TableTop"],
        //     GameManager.Instance.EquippedItem["CardBack"]);

        StartGame();
    }

    public override bool SetFinalScore()
    {
        for (int i = 0; i < Players.Length; i++)
        {
            Players[i].Score = 0;
        }

        switch (balootRoundScript.RoundType)
        {
            case BalootGameType.Sun:
                SetScoreSuns();
                break;
            case BalootGameType.Hukom:
                int[] total = new int[2];

                total[0] = Players[0].Score + Players[2].Score;
                total[0] = CalculatePointsHokum(total[0]);

                total[1] = Players[1].Score + Players[3].Score;
                total[1] = CalculatePointsHokum(total[1]);


                if (total[balootRoundScript.BidingTeam] > total[(balootRoundScript.BidingTeam + 1) % 2] + 8)
                {
                    TeamsScore[0] = total[0];
                    TeamsScore[1] = total[1];
                }
                else
                {
                    TeamsScore[balootRoundScript.BidingTeam] = 0;
                    TeamsScore[(balootRoundScript.BidingTeam + 1) % 2] = 16;
                }
                break;
            case BalootGameType.Ashkal:
                SetScoreSuns();
                break;
        }

        bool finished  = false;

        for (int i = 0; i < 2; i++)
        {
            TeamsTotalScore[i] += TeamsScore[i];

            if (TeamsTotalScore[i] >= 152)
            {
                finished = true;
            }
        }

        return finished;
    }

    private void SetScoreSuns()
    {
        int[] total = new int[2];

        total[0] = Players[0].Score + Players[2].Score;
        total[0] = CalculatePointsSuns(total[0]);

        total[1] = Players[1].Score + Players[3].Score;
        total[1] = CalculatePointsSuns(total[1]);


        if (total[balootRoundScript.BidingTeam] > total[(balootRoundScript.BidingTeam + 1) % 2] + 13)
        {
            TeamsScore[0] = total[0];
            TeamsScore[1] = total[1];
        }
        else
        {
            TeamsScore[balootRoundScript.BidingTeam] = 0;
            TeamsScore[(balootRoundScript.BidingTeam + 1) % 2] = 26;
        }
    }

    private int CalculatePointsSuns(int total)
    {
        if (total % 5 == 0)
        {
            total *= 2;
            total /= 10;
        }
        else
        {
            total = Mathf.RoundToInt((float)total / 10) * 2;
        }

        return total;
    }

    private int CalculatePointsHokum(int total)
    {
        return Mathf.RoundToInt(total / 10);
    }

    public void Players_SelectedType(int index, BalootGameType type)
    {
        OnPlayerSelectedType?.Invoke(index, type);
        balootRoundScript.PlayerSelectedType(index, type);
    }

    private void GameScript_OnCardReady(int playerIndex, Card card)
    {
        RoundScript.OnCardReady(playerIndex, card);
    }

    private void Deal_OnEvent(int eventIndex)
    {
        EventTypeBaloot eventType = (EventTypeBaloot)eventIndex;
        switch (eventType)
        {
            case EventTypeBaloot.CardsDealtBegin:
                OnStartCardsReady?.Invoke(balootRoundScript.BalootCard);
                ((BalootPlayer)Players[balootRoundScript.StartIndex]).CheckGameType();
                break;
            case EventTypeBaloot.RestartDeal:
                RestartGame();

                break;
            case EventTypeBaloot.CardsDealtFinished:
                SetCardsReady();
                SetStartGame(false);
                break;
            case EventTypeBaloot.TrickFinished:
                Deal_OnTrickFinished(RoundScript.PlayingIndex);
                break;
            case EventTypeBaloot.DealFinished:
                Deal_OnDealFinished();

                TeamsScore[0] = 0;
                TeamsScore[1] = 0;
                break;
        }
    }

    private async void RestartGame()
    {
        OnRestartDeal?.Invoke();
        await System.Threading.Tasks.Task.Delay(1500);
        StartGame();
    }
}
