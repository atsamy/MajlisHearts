using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class GameScriptBaloot : GameScriptBase
{
    public static GameScriptBaloot Instance;
    //MainPlayerBaloot myPlayer => (MainPlayerBaloot)Players[0];

    public delegate void StartCardsReady(Card balootCard);
    public event StartCardsReady OnStartCardsReady;

    public delegate void PlayerSelectedType(int index, BalootGameType type);
    public event PlayerSelectedType OnPlayerSelectedType;

    public delegate void RestartDeal();
    public event RestartDeal OnRestartDeal;

    public delegate void RoundDoubled(int playerIndex, int doubleValue);
    public event RoundDoubled OnRoundDoubled;


    public event Action<int> OnRevealProject;
    public event Action OnHideProject;

    public RoundScriptBaloot balootRoundScript => (RoundScriptBaloot)RoundScript;

    [HideInInspector]
    public int[] TeamsScore;

    [HideInInspector]
    public int[] ProjectsScore;

    [HideInInspector]
    public int[] TeamsTotalScore;

    int declarerIndex = 0;
    int doublerIndex = -2;

    [HideInInspector]
    public int DoubleValue;
    private void Awake()
    {
        Instance = this;
        RoundScript = new RoundScriptBaloot();
        TeamsScore = new int[2];
        TeamsTotalScore = new int[2];
        ProjectsScore = new int[2];
    }

    void Start()
    {
        balootRoundScript.OnEvent += Deal_OnEvent;

        Players = new PlayerBaloot[4];

        for (int i = 0; i < 4; i++)
        {
            if (i == 0)
            {
                Players[i] = new MainPlayerBaloot(i);
                //Players[i].Avatar = AvatarManager.Instance.playerAvatar;
                Players[i].Name = GameManager.Instance.MyPlayer.Name;
            }
            else
            {
                Players[i] = new AIPlayerBaloot(i);
                //Players[i].Avatar = AvatarManager.Instance.RobotAvatar;
                Players[i].Name = "Player " + i;
            }

            ((PlayerBaloot)Players[i]).OnTypeSelected += Players_SelectedType;
            ((PlayerBaloot)Players[i]).OnDoubleSelected += GameScriptBaloot_OnDoubleSelected; ;
            Players[i].OnCardReady += GameScript_OnCardReady;

        }

        RoundScript.SetPlayers(Players);
        balootRoundScript.OnGameTypeSelected += BalootRoundScript_OnGameTypeSelected;
        //SetEnvironment(GameManager.Instance.EquippedItem["TableTop"],
        //     GameManager.Instance.EquippedItem["CardBack"]);
        SetGameReady();
        StartGame();
    }

    private void GameScriptBaloot_OnDoubleSelected(int playerIndex, bool isDouble, int value)
    {
        if (isDouble)
        {
            DoubleValue = value + 2;
            if (value == 3)
            {
                balootRoundScript.DealContinue(declarerIndex);
            }
            else if (playerIndex != declarerIndex)
            {
                ((PlayerBaloot)Players[declarerIndex]).CheckDouble(value + 1);
                doublerIndex = playerIndex;
            }
            else
            {
                ((PlayerBaloot)Players[doublerIndex]).CheckDouble(value + 1);
            }

            if (value == 0)
            {
                ((PlayerBaloot)Players[(playerIndex + 2) % 4]).CancelDouble();
            }


            OnRoundDoubled?.Invoke(playerIndex, DoubleValue);
        }
        else
        {
            if (DoubleValue == 0 && doublerIndex == -1)
            {
                doublerIndex = -2;
                return;
            }
            balootRoundScript.DealContinue(declarerIndex);
            DoubleValue = value + 1;
        }
    }

    private void BalootRoundScript_OnGameTypeSelected(int index, BalootGameType gameType)
    {
        declarerIndex = index;
        doublerIndex = -1;

        if (gameType == BalootGameType.Hokum)
        {
            ((PlayerBaloot)Players[(index + 1) % 4]).CheckDouble(0);
            ((PlayerBaloot)Players[(index + 3) % 4]).CheckDouble(0);
        }
        else
        {
            balootRoundScript.DealContinue(index);
        }
    }

    public override bool SetFinalScore()
    {
        switch (balootRoundScript.RoundType)
        {
            case BalootGameType.Sun:
                SetScoreSuns();
                break;
            case BalootGameType.Hokum:
                int[] total = new int[2];

                total[0] = Players[0].Score + Players[2].Score;
                ProjectsScore[0] = ((PlayerBaloot)Players[0]).ProjectScore + ((PlayerBaloot)Players[2]).ProjectScore;
                total[0] = CalculatePointsHokum(total[0]);

                total[1] = Players[1].Score + Players[3].Score;
                ProjectsScore[1] = ((PlayerBaloot)Players[1]).ProjectScore + ((PlayerBaloot)Players[3]).ProjectScore;
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

                TeamsScore[0] += ProjectsScore[0] / 10;
                TeamsScore[1] += ProjectsScore[1] / 10;

                break;
            case BalootGameType.Ashkal:
                SetScoreSuns();
                break;
        }

        bool finished = false;

        for (int i = 0; i < 2; i++)
        {
            TeamsTotalScore[i] += TeamsScore[i];

            if (TeamsTotalScore[i] >= 152 || DoubleValue == 5)
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
        ProjectsScore[0] = ((PlayerBaloot)Players[0]).ProjectScore + ((PlayerBaloot)Players[2]).ProjectScore;
        total[0] = CalculatePointsSuns(total[0]);

        total[1] = Players[1].Score + Players[3].Score;
        ProjectsScore[1] = ((PlayerBaloot)Players[1]).ProjectScore + ((PlayerBaloot)Players[3]).ProjectScore;
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

        TeamsScore[0] += ProjectsScore[0] / 5;
        TeamsScore[1] += ProjectsScore[1] / 5;

        TeamsScore[0] *= DoubleValue;
        TeamsScore[1] *= DoubleValue;
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
        if ((playerIndex == MainPlayerIndex) && RoundScript.RoundInfo.TrickNumber == 0)
        {
            OnHideProject?.Invoke();
        }
        if (RoundScript.RoundInfo.TrickNumber == 1)
        {
            OnRevealProject?.Invoke(playerIndex);
        }
        RoundScript.OnCardReady(playerIndex, card);
    }

    private void Deal_OnEvent(int eventIndex)
    {
        EventTypeBaloot eventType = (EventTypeBaloot)eventIndex;
        switch (eventType)
        {
            case EventTypeBaloot.CardsDealtBegin:
                OnStartCardsReady?.Invoke(balootRoundScript.BalootCard);
                ((PlayerBaloot)Players[balootRoundScript.StartIndex]).CheckGameType();
                break;
            case EventTypeBaloot.RestartDeal:
                RestartGame();

                break;
            case EventTypeBaloot.CardsDealtFinished:
                for (int i = 0; i < Players.Length; i++)
                {
                    Players[i].Score = 0;
                    ((PlayerBaloot)Players[i]).SetStartCards();
                    if (balootRoundScript.RoundType == BalootGameType.Hokum)
                    {
                        ((PlayerBaloot)Players[i]).CheckBalootCards(balootRoundScript.HokumShape);
                    }
                }
                SetCardsReady();
                SetStartGame(false);
                break;
            case EventTypeBaloot.TrickFinished:
                Deal_OnTrickFinished(RoundScript.PlayingIndex);

                if (RoundScript.RoundInfo.TrickNumber == 1)
                {
                    foreach (PlayerBaloot item in Players)
                    {
                        item.ChooseProjects(balootRoundScript.RoundType);
                    }

                    CompareProjects();
                }

                break;
            case EventTypeBaloot.DealFinished:
                Deal_OnDealFinished();

                TeamsScore[0] = 0;
                TeamsScore[1] = 0;
                break;
        }
    }

    private void CompareProjects()
    {
        int bestScore = 0;
        int bestPower = 0;
        int winIndex = -1;

        for (int i = 0; i < Players.Length; i++)
        {
            if (((PlayerBaloot)Players[i]).ProjectScore > bestScore)
            {
                winIndex = i;
                bestScore = ((PlayerBaloot)Players[i]).ProjectScore;
                bestPower = ((PlayerBaloot)Players[i]).ProjectPower;
            }
            else if (((PlayerBaloot)Players[i]).ProjectScore == bestScore && bestScore > 0)
            {
                if (((PlayerBaloot)Players[i]).ProjectPower > bestPower)
                {
                    winIndex = i;
                    bestPower = ((PlayerBaloot)Players[i]).ProjectPower;
                }
                else if (((PlayerBaloot)Players[i]).ProjectPower == bestPower)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        int nearestPlayer = (j + balootRoundScript.StartIndex) % 4;

                        if (nearestPlayer == i)
                        {
                            winIndex = i;
                            break;
                        }
                        else if (nearestPlayer == winIndex)
                        {
                            break;
                        }
                    }
                }
            }
        }

        ((PlayerBaloot)Players[(winIndex + 1) % 4]).RemoveProjects();
        ((PlayerBaloot)Players[(winIndex + 3) % 4]).RemoveProjects();
    }

    private async void RestartGame()
    {
        OnRestartDeal?.Invoke();
        await System.Threading.Tasks.Task.Delay(1500);
        StartGame();
    }
}
