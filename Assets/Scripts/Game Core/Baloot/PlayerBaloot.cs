using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using static UIParticleSystem;

public class PlayerBaloot : PlayerBase
{
    public delegate void TypeSelected(int playerIndex, BalootGameType type);
    public TypeSelected OnTypeSelected;

    public event Action BalootCardsPlayed;

    public delegate void DoubleSelected(int playerIndex,bool isDouble, int value);
    public DoubleSelected OnDoubleSelected;

    //public event Action<bool,int,int> OnDoubleSelected;

    List<Card> startCards;

    public int ProjectPower { get; protected set; }
    public int ProjectScore { get; protected set; }
    public Dictionary<List<Card>, Projects> PlayerProjects { get; private set; }

    bool haveBalootCards;
    CardShape hokumShape;
    int balootSequence = 0;
    public PlayerBaloot(int index) : base(index)
    {
        ProjectPower = 0;
        ProjectScore = 0;
        PlayerProjects = new();
    }

    public virtual void CheckGameType()
    {

    }

    public virtual void CheckDouble(int value)
    {
        //await Task.Delay(100);
    }

    public virtual void SelectDouble(bool value,int doubleValue)
    {
        OnDoubleSelected?.Invoke(index,value,doubleValue);
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
        haveBalootCards = OwnedCards.Contains(new Card(shape,CardRank.King)) 
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

    public List<Project> GetAllProjects(BalootGameType type)
    {
        List<Project> allProjects = new List<Project>();
        List<Card> aceCount = startCards.Where(a => a.Rank == CardRank.Ace).ToList();

        if (aceCount.Count == 4)
        {
            if (type == BalootGameType.Hokum)
            {
                allProjects.Add(new Project(Projects.OneHundred, aceCount, 100, 44));
            }
            else
            {
                allProjects.Add(new Project(Projects.FourHundred, aceCount, 400, 44));
            }

            startCards.RemoveAll(a => a.Rank == CardRank.Ace);
        }

        for (int i = 8; i < 12; i++)
        {
            List<Card> rankCount = startCards.Where(a => a.Rank == (CardRank)i).ToList();

            if (rankCount.Count == 4)
            {
                allProjects.Add(new Project(Projects.OneHundred, rankCount, 100, 40 + (i - 8)));
                startCards.RemoveAll(a => a.Rank == (CardRank)i);

                if (i == 10 || i == 11)
                {
                    haveBalootCards = false;
                }
            }
        }


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
                    CheckCount(count, i,allProjects);
                    count = 0;
                }
            }
            else
            {
                CheckCount(count, i,allProjects);
                count = 0;
            }
        }

        return allProjects;
    }

    public void CheckCount(int count, int index, List<Project> allProjects)
    {
        switch (count)
        {
            case 2:
                AddAvailableProject(index, count + 1, 20, Projects.Sira, allProjects);
                break;
            case 3:
                AddAvailableProject(index, count + 1, 50, Projects.Fifty, allProjects);
                goto case 2; //falls through to previous case
            case 4:
                AddAvailableProject(index, count + 1, 100, Projects.OneHundred, allProjects);

                if (haveBalootCards)
                {
                    if (allProjects.Last().Cards.Contains(new Card(hokumShape, CardRank.King)) ||
                    allProjects.Last().Cards.Contains(new Card(hokumShape, CardRank.Queen)))
                    {
                        haveBalootCards = false;
                    }
                }
                goto case 3; //falls through to previous case
        }
    }

    private void AddAvailableProject(int index, int count, int score, Projects project, List<Project> allProjects)
    {
        //bug here
        if (index - count < 0)
        {
            Debug.LogError("index:" + index + " count:" + count + " both is less than zero");
            return;
        }
        allProjects.Add(new Project(project, startCards.GetRange(index - count, count), score, ((int)project + ((int)startCards[index].Rank - 6))));
        startCards.RemoveRange(index - count, count);
    }

    internal void RemoveProjects()
    {
        PlayerProjects.Clear();
    }

    internal virtual void CancelDouble()
    {

    }
}

public enum Projects
{
    Sira = 0,
    Fifty = 1,
    OneHundred = 2,
    FourHundred = 3
}

public class Project
{
    public Projects projectName;
    public List<Card> Cards;
    public int Score;
    public int Power;

    public Project(Projects projectName, List<Card> Cards, int Score, int Power)
    {
        this.projectName = projectName;
        this.Cards = Cards;
        this.Score = Score;
        this.Power = Power;
    }
}
