using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;


public class PlayerBaloot : PlayerBase
{
    public delegate void TypeSelected(int playerIndex, BalootGameType type);
    public TypeSelected OnTypeSelected;

    public event Action BalootCardsPlayed;

    public delegate void DoubleSelected(int playerIndex, bool isDouble, int value);
    public DoubleSelected OnDoubleSelected;

    public event Action<CardShape> OnChangedHokumShape;
    public event Action<int> OnCheckType;
    public event Action<int, int> OnCheckDouble;

    protected List<Card> startCards;

    public int ProjectPower { get; protected set; }
    public int ProjectScore { get; protected set; }
    public Dictionary<List<Card>, Projects> PlayerProjects { get; private set; }

    protected bool haveBalootCards;
    CardShape hokumShape;
    int balootSequence = 0;

    int[] projectScores = new int[3] { 20, 50, 100 };
    public PlayerBaloot(int index) : base(index)
    {
        ProjectPower = 0;
        ProjectScore = 0;
        PlayerProjects = new();
    }

    public void SetProjects(Dictionary<List<Card>, Projects> playerProjects, int power, int score)
    {
        PlayerProjects = playerProjects;
        ProjectScore = score;
        ProjectPower = power;
    }

    public void SetProjects(Dictionary<List<Card>, Projects> playerProjects)
    {
        PlayerProjects = playerProjects;
    }

    public void RemoveProjects()
    {
        PlayerProjects.Clear();
        ProjectScore = 0;
        ProjectPower = 0;
    }

    public virtual void CheckGameType(RoundScriptBaloot roundScriptBaloot)
    {
        OnCheckType?.Invoke(index);
    }

    public void ChangedHokumShape(CardShape shape)
    {
        OnChangedHokumShape?.Invoke(shape);
    }

    public override void SetTotalScore()
    {
        totalScore = dealScore;
        dealScore = 0;
    }

    public virtual void CheckDouble(int value)
    {
        OnCheckDouble?.Invoke(index, value);
        //await Task.Delay(100);
    }

    public virtual void SelectDouble(bool value, int doubleValue)
    {
        OnDoubleSelected?.Invoke(index, value, doubleValue);
    }

    public override void ChooseCard(Card card)
    {
        base.ChooseCard(card);

        if (haveBalootCards)
        {
            if (card.Shape == hokumShape && (card.Rank == CardRank.King || card.Rank == CardRank.Queen))
            {
                balootSequence++;

                if (balootSequence == 2)
                {
                    BalootCardsPlayed?.Invoke();
                    ProjectScore += 20;
                    balootSequence = 0;
                }
            }
            else
            {
                balootSequence = 0;
            }
        }
    }

    public void CheckBalootCards(CardShape shape)
    {
        haveBalootCards = OwnedCards.Contains(new Card(shape, CardRank.King))
            && OwnedCards.Contains(new Card(shape, CardRank.Queen));

        hokumShape = shape;
    }

    public override void Reset()
    {
        base.Reset();
        PlayerProjects.Clear();
        ProjectPower = 0;
        ProjectScore = 0;
    }

    public virtual void SelectType(BalootGameType type)
    {
        OnTypeSelected?.Invoke(index, type);
    }

    public virtual void ChooseProjects(BalootGameType type)
    {

    }

    public void SetStartCards()
    {
        startCards = OwnedCards.OrderBy(a => a.Shape).ThenByDescending(a => a.Rank).ToList();
    }

    protected void CheckFourhundredProject()
    {
        List<Card> aceCount = startCards.Where(a => a.Rank == CardRank.Ace).ToList();
        if (aceCount.Count == 4)
        {
            PlayerProjects.Add(aceCount, Projects.FourHundred);

            ProjectScore += 400;
            ProjectPower += 44;

            startCards.RemoveAll(a => a.Rank == CardRank.Ace);
        }
    }

    public void CheckOneHundredProject(BalootGameType type)
    {
        for (int i = 8; i < 13; i++)
        {
            if (i == 12 && type != BalootGameType.Hokum)
                return;

            List<Card> rankCount = startCards.Where(a => a.Rank == (CardRank)i).ToList();

            if (rankCount.Count == 4)
            {
                if (i == 10 || i == 11)
                {
                    haveBalootCards = false;
                }

                PlayerProjects.Add(rankCount, Projects.OneHundred);
                ProjectScore += 100;
                ProjectPower += 40 + (i - 8);

                startCards.RemoveAll(a => a.Rank == (CardRank)i);
            }
        }
    }

    public void CheckSequenceProject(Projects project)
    {
        int count = 0;

        for (int i = 1; i < startCards.Count; i++)
        {
            if (startCards[i].Shape == startCards[i - 1].Shape)
            {
                if (startCards[i].Rank + 1 == startCards[i - 1].Rank)
                {
                    count++;
                }
                else
                {
                    if (count == ((int)project + 2))
                    {
                        AddSequenceProject(i, project, count + 1);
                        count = 0;
                        break;
                    }
                    count = 0;
                }
            }
            else
            {
                if (count == ((int)project + 2))
                {
                    AddSequenceProject(i, project, count + 1);
                    count = 0;
                    break;
                }
                count = 0;
            }
        }

        if (count == ((int)project + 2))
        {
            AddSequenceProject(startCards.Count, project, count + 1);
        }
    }

    private void AddSequenceProject(int index, Projects project, int count)
    {
        //Debug.Log(startCards.Count + " " + index + " " + count);
        PlayerProjects.Add(startCards.GetRange(index - count, count), project);

        ProjectScore += projectScores[(int)project];
        ProjectPower += ((int)project + ((int)startCards[index - count].Rank - 6));

        startCards.RemoveRange(index - count, count);
    }

    internal virtual void CancelDouble()
    {

    }

    public void CheckForProject(BalootGameType type, Projects project)
    {
        if (project != Projects.FourHundred)
        {
            if (project == Projects.OneHundred)
            {
                CheckOneHundredProject(type);
            }
            CheckSequenceProject(project);
        }
        else
        {
            CheckFourhundredProject();
        }
    }
}

public enum Projects
{
    Sira = 0,
    Fifty = 1,
    OneHundred = 2,
    FourHundred = 3
}

//public class Project
//{
//    public Projects projectName;
//    public List<Card> Cards;
//    public int Score;
//    public int Power;

//    public Project(Projects projectName, List<Card> Cards, int Score, int Power)
//    {
//        this.projectName = projectName;
//        this.Cards = Cards;
//        this.Score = Score;
//        this.Power = Power;
//    }
//}
