using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerBaloot : PlayerBase
{
    public delegate void TypeSelected(int playerIndex, BalootGameType type);
    public TypeSelected OnTypeSelected;

    Dictionary<Projects, int> ProjectsCount;

    Dictionary<List<Card>, Projects> AvailableProjects;
    public PlayerBaloot(int index) : base(index)
    {
        ProjectsCount = new();
        AvailableProjects = new();
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

    public bool AddProject(Projects project)
    {
        if ((int)project <= 20 && ProjectsCount[project] == 2)
            return false;
        if (project == Projects.FourHundred && ProjectsCount[project] == 1)
            return false;

        ProjectsCount[project]++;
        return true;
    }

    public void CheckAvailableProjects(BalootGameType type)
    {
        for (int i = 1; i < OwnedCards.Count; i++)
        {
            int count = 0;
            if (OwnedCards[i].Shape == OwnedCards[i - 1].Shape)
            {
                if (OwnedCards[i].Rank == OwnedCards[i - 1].Rank + 1)
                {
                    count++;
                }
                else
                {
                    CheckCount(count,i);
                    count = 0;
                }
            }
            else
            {
                CheckCount(count, i);
                count = 0;
            }
        }

        for (int i = 8; i < 12; i++)
        {
            List<Card> rankCount = OwnedCards.Where(a => a.Rank == (CardRank)i).ToList();
            
            if (rankCount.Count == 4)
            {
                AvailableProjects.Add(rankCount, Projects.OneHundred);
            }
        }

        List<Card> aceCount = OwnedCards.Where(a => a.Rank == CardRank.Ace).ToList();

        if (type == BalootGameType.Hokum)
        {
            AvailableProjects.Add(aceCount, Projects.OneHundred);
        }
        else
        {
            AvailableProjects.Add(aceCount, Projects.FourHundred);
        }

    }

    public void CheckCount(int count,int index)
    {
        switch (count)
        {
            case 2:
                AvailableProjects.Add(AddProjectCardsToList(index, 3), Projects.Sira);
                break;
            case 3:
                AvailableProjects.Add(AddProjectCardsToList(index, 4), Projects.Fifty);
                goto case 2; // falls through to previous case
            case 4:
                AvailableProjects.Add(AddProjectCardsToList(index, 5), Projects.OneHundred);
                goto case 3; // falls through to previous case
        }
    }

    private List<Card> AddProjectCardsToList(int i,int num)
    {
        List<Card> list = new List<Card>();
        for (int j = 0; j < num; j++)
        {
            list.Add(OwnedCards[i - i]);
        }

        return list;
    }
}

public enum Projects
{
    Sira = 4,
    Fifty = 10,
    OneHundred = 20,
    FourHundred = 40
}
