using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditorInternal;
using UnityEngine;

public class GameScriptBaloot : GameScriptBase
{
    public static GameScriptBaloot Instance;
    //MainPlayerBaloot myPlayer => (MainPlayerBaloot)Players[0];

    public delegate void StartCardsReady(Card balootCard,int startIndex);
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

    [HideInInspector]
    public int DeclarerIndex = 0;
    int doublerIndex = -2;

    [HideInInspector]
    public int DoubleValue;

    public int WinningTeam;
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

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Keypad1))
            PrintCard(1);
        else if (Input.GetKeyDown(KeyCode.Keypad2))
            PrintCard(2);
        else if (Input.GetKeyDown(KeyCode.Keypad3))
            PrintCard(3);
    }

    void PrintCard(int index)
    {
        foreach (var item in Players[index].ShapeCount)
        {
            print(item);
        }
        foreach (var item in Players[index].OwnedCards)
        {
            print(item);
        }
    }

    private void GameScriptBaloot_OnDoubleSelected(int playerIndex, bool isDouble, int value)
    {
        if (isDouble)
        {
            DoubleValue = value + 2;
            if (value == 3)
            {
                balootRoundScript.DealContinue(DeclarerIndex);
            }
            else if (playerIndex != DeclarerIndex)
            {
                ((PlayerBaloot)Players[DeclarerIndex]).CheckDouble(value + 1);
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
            //check bug here the first condition is not true for first player
            //print(string.Format("double values: {0} {1}" ,doublerIndex , DoubleValue));
            if (DoubleValue == 0 && doublerIndex == -1)
            {
                doublerIndex = -2;
                return;
            }
            balootRoundScript.DealContinue(DeclarerIndex);
            DoubleValue = value + 1;
        }
    }

    private void BalootRoundScript_OnGameTypeSelected(int index, BalootGameType gameType)
    {
        DeclarerIndex = index;
        doublerIndex = -1;
        DoubleValue = 0;

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
        int team1TrickCount = Players[0].TricksCount + Players[2].TricksCount;
        //print("team 1 count: " + team1TrickCount);

        switch (balootRoundScript.RoundType)
        {
            case BalootGameType.Sun:
                SetScoreSuns(team1TrickCount);
                break;
            case BalootGameType.Hokum:
                int[] total = new int[2];

                total[0] = Players[0].Score + Players[2].Score;
                ProjectsScore[0] = ((PlayerBaloot)Players[0]).ProjectScore + ((PlayerBaloot)Players[2]).ProjectScore;
                total[0] = CalculatePointsHokum(total[0]);

                total[1] = Players[1].Score + Players[3].Score;
                ProjectsScore[1] = ((PlayerBaloot)Players[1]).ProjectScore + ((PlayerBaloot)Players[3]).ProjectScore;
                total[1] = CalculatePointsHokum(total[1]);

                if (team1TrickCount == 8)
                {
                    TeamsScore[0] = 25;
                    TeamsScore[1] = 0;
                    WinningTeam = 0;
                }
                else if (team1TrickCount == 0)
                {
                    TeamsScore[0] = 0;
                    TeamsScore[1] = 25;
                    WinningTeam = 1;
                }
                else
                {
                    if (total[balootRoundScript.BidingTeam] > total[balootRoundScript.OtherTeam])
                    {
                        TeamsScore[0] = total[0];
                        TeamsScore[1] = total[1];

                        WinningTeam = balootRoundScript.BidingTeam;
                    }
                    else
                    {
                        TeamsScore[balootRoundScript.BidingTeam] = 0;
                        TeamsScore[balootRoundScript.OtherTeam] = 16;

                        WinningTeam = balootRoundScript.OtherTeam;
                    }
                }

                TeamsScore[0] += ProjectsScore[0] / 10;
                TeamsScore[1] += ProjectsScore[1] / 10;

                TeamsScore[0] *= DoubleValue;
                TeamsScore[1] *= DoubleValue;

                break;
            case BalootGameType.Ashkal:
                SetScoreSuns(team1TrickCount);
                break;
        }

        bool finished = false;

        for (int i = 0; i < 2; i++)
        {
            TeamsTotalScore[i] += TeamsScore[i];

            if (TeamsTotalScore[i] >= 152)
            {
                if (!finished)
                {
                    finished = true;
                    WinningTeam = i;
                }
                else
                {
                    WinningTeam = TeamsTotalScore[0] > TeamsTotalScore[1] ? 0 : 1;
                }
            }
        }

        if (DoubleValue == 5)
        {
            WinningTeam = TeamsScore[0] > TeamsScore[1] ? 0 : 1;
            finished = true;
        }

        foreach (PlayerBaloot player in Players)
        {
            player.SetTotalScore();
        }

        return finished;
    }

    private void SetScoreSuns(int team1TrickCount)
    {
        int[] total = new int[2];

        total[0] = Players[0].Score + Players[2].Score;
        ProjectsScore[0] = ((PlayerBaloot)Players[0]).ProjectScore + ((PlayerBaloot)Players[2]).ProjectScore;
        total[0] = CalculatePointsSuns(total[0]);

        total[1] = Players[1].Score + Players[3].Score;
        ProjectsScore[1] = ((PlayerBaloot)Players[1]).ProjectScore + ((PlayerBaloot)Players[3]).ProjectScore;
        total[1] = CalculatePointsSuns(total[1]);

        if (team1TrickCount == 8)
        {
            TeamsScore[0] = 44;
            TeamsScore[1] = 0;
            WinningTeam = 0;
        }
        else if (team1TrickCount == 0)
        {
            TeamsScore[0] = 0;
            TeamsScore[1] = 44;
            WinningTeam = 1;
        }
        else
        {
            if (total[balootRoundScript.BidingTeam] > total[balootRoundScript.OtherTeam])
            {
                TeamsScore[0] = total[0];
                TeamsScore[1] = total[1];

                WinningTeam = balootRoundScript.BidingTeam;
            }
            else
            {
                TeamsScore[balootRoundScript.BidingTeam] = 0;
                TeamsScore[balootRoundScript.OtherTeam] = 26;

                WinningTeam = balootRoundScript.OtherTeam;
            }
        }
        TeamsScore[0] += ProjectsScore[0] / 5;
        TeamsScore[1] += ProjectsScore[1] / 5;
    }

    private int CalculatePointsSuns(int total)
    {
        return Mathf.RoundToInt(((float)total / 10) * 2);
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
                DealCards();
                break;
            case EventTypeBaloot.RestartDeal:
                RestartGame();
                break;
            case EventTypeBaloot.CardsDealtFinished:
                DealRemaingCards();
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

    private void DealRemaingCards()
    {
        foreach (PlayerBaloot player in Players)
        {
            player.SetStartCards();
            if (balootRoundScript.RoundType == BalootGameType.Hokum)
            {
                player.CheckBalootCards(balootRoundScript.balootRoundInfo.HokumShape);
            }
        }

        SetCardsReady();
    }

    private void DealCards()
    {
        OnStartCardsReady?.Invoke(balootRoundScript.BalootCard, balootRoundScript.StartIndex);
    }

    public void CheckType()
    {
        ((PlayerBaloot)Players[balootRoundScript.StartIndex]).CheckGameType(balootRoundScript);
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
        await Task.Delay(1500);
        StartGame();
    }

    internal override PlayerBase InstantiateMainPlayer(int index)
    {
        MainPlayerIndex = index;
        return new MainPlayerBaloot(index);
    }
}
