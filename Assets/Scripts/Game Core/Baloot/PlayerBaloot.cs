using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerBaloot : PlayerBase
{
    public delegate void TypeSelected(int playerIndex, BalootGameType type);
    public TypeSelected OnTypeSelected;

    Dictionary<Projects, int> ProjectsCount;
    List<Card> startCards;
    //Dictionary<List<Card>, Projects> AvailableProjects;
    public PlayerBaloot(int index) : base(index)
    {
        ProjectsCount = new();
        //AvailableProjects = new();
    }

    public virtual void CheckGameType()
    {

    }

    public override void Reset()
    {
        base.Reset();
        ProjectsCount.Clear();
    }

    public virtual void SelectType(BalootGameType type)
    {
        OnTypeSelected?.Invoke(index, type);
    }

    public void AddProject(Projects project, int count)
    {
        if (count == 0)
        {
            ProjectsCount.Remove(project);
            return;
        }
        ProjectsCount[project] = count;
    }

    public void SetStartCards()
    {
        startCards = OwnedCards.OrderBy(a => a.Shape).ThenByDescending(a => a.Rank).ToList();
    }

    public Dictionary<List<Card>, Projects> CheckAvailableProjects(BalootGameType type)
    {
        if (ProjectsCount.Count == 0)
            return new Dictionary<List<Card>, Projects>();

         
        Dictionary<List<Card>, Projects> AvailableProjects = new ();
        //order cards
        if (ProjectsCount.ContainsKey(Projects.FourHundred) || ProjectsCount.ContainsKey(Projects.OneHundred))
        {
            List<Card> aceCount = startCards.Where(a => a.Rank == CardRank.Ace).ToList();

            if (aceCount.Count == 4)
            {
                if (type == BalootGameType.Hokum)
                {
                    AvailableProjects.Add(aceCount, Projects.OneHundred);
                }
                else
                {
                    AvailableProjects.Add(aceCount, Projects.FourHundred);
                }

                startCards.RemoveAll(a => a.Rank == CardRank.Ace);
            }

            for (int i = 8; i < 12; i++)
            {
                List<Card> rankCount = startCards.Where(a => a.Rank == (CardRank)i).ToList();

                if (rankCount.Count == 4)
                {
                    AvailableProjects.Add(rankCount, Projects.OneHundred);
                    startCards.RemoveAll(a => a.Rank == (CardRank)i);
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
                    CheckCount(AvailableProjects, count, i);
                    count = 0;
                }
            }
            else
            {
                CheckCount(AvailableProjects, count, i);
                count = 0;
            }
        }

        return AvailableProjects;
    }

    public void CheckCount(Dictionary<List<Card>, Projects> AvailableProjects, int count, int index)
    {
        switch (count)
        {
            case 2:
                AddAvailableProject(AvailableProjects, index, count + 1, Projects.Sira);
                break;
            case 3:
                AddAvailableProject(AvailableProjects, index, count + 1, Projects.Fifty);
                goto case 2; // falls through to previous case
            case 4:
                AddAvailableProject(AvailableProjects, index, count + 1, Projects.OneHundred);
                goto case 3; // falls through to previous case
        }
    }

    private void AddAvailableProject(Dictionary<List<Card>, Projects> AvailableProjects, int index, int count, Projects project)
    {
        if (ProjectsCount.ContainsKey(project) && ProjectsCount[project] > 0)
        {
            AvailableProjects.Add(startCards.GetRange(index - count, count), project);
            ProjectsCount[project]--;
            startCards.RemoveRange(index - count, count);
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
