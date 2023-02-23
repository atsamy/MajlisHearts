using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerBaloot : PlayerBase
{
    public delegate void TypeSelected(int playerIndex, BalootGameType type);
    public TypeSelected OnTypeSelected;

    List<Card> startCards;

    public int ProjectPower { get; protected set; }
    public int ProjectScore { get; protected set; }
    public Dictionary<List<Card>, Projects> PlayerProjects { get; private set; }
    public PlayerBaloot(int index) : base(index)
    {
        ProjectPower = 0;
        ProjectScore = 0;
        PlayerProjects = new();
    }

    public virtual void CheckGameType()
    {

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
                goto case 2; // falls through to previous case
            case 4:
                AddAvailableProject(index, count + 1, 100, Projects.OneHundred, allProjects);
                goto case 3; // falls through to previous case
        }
    }

    private void AddAvailableProject(int index, int count, int score, Projects project, List<Project> allProjects)
    {
        allProjects.Add(new Project(project, startCards.GetRange(index - count, count), score, ((int)project + ((int)startCards[index].Rank - 6))));
        startCards.RemoveRange(index - count, count);
    }

    internal void RemoveProjects()
    {
        PlayerProjects.Clear();
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
