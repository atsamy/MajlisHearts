using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class AIPlayerBaloot : PlayerBaloot
{
    public bool FakePlayer { private get; set; }
    bool cancelDouble;

    BalootGameType balootGameType;
    public AIPlayerBaloot(int index) : base(index)
    {
        isPlayer = false;
    }

    public override void SetTurn(RoundInfo info)
    {
        base.SetTurn(info);
        playCard(info);
    }

    public override async void CheckDouble(int value)
    {
        await Task.Delay(2000);

        if (!cancelDouble)
            SelectDouble(false, value);
    }

    public override void ChooseProjects(BalootGameType type)
    {
        List<Project> projects = GetAllProjects(type);

        foreach (Project project in projects)
        {
            PlayerProjects.Add(project.Cards, project.projectName);
            ProjectScore += project.Score;
            ProjectPower += project.Power;
        }
    }

    public override async void CheckGameType()
    {
        await Task.Delay(1000);
        SelectType( index == 1 ? BalootGameType.Hokum : BalootGameType.Pass);
    }

    internal override void CancelDouble()
    {
        cancelDouble = true;
    }

    async void playCard(RoundInfo info)
    {
        if (!FakePlayer)
            await Task.Delay(1000);

        int hand = info.CardsOntable.Count;

        if (hand == 0 && info.TrickNumber == 0)
        {
            if (FakePlayer)
                await Task.Delay(600);

            ChooseCard(OwnedCards[Random.Range(0, OwnedCards.Count)]);
            return;
        }

        shapeCount = shapeCount.OrderByDescending(a => a.Value).ToDictionary(x => x.Key, x => x.Value);

        if (hand == 0)
        {
            if (FakePlayer)
                await Task.Delay(Mathf.Max(800, OwnedCards.Count * Random.Range(250, 300)));

            ChooseCard(OwnedCards[Random.Range(0, OwnedCards.Count)]);
        }
        else
        {
            List<Card> specificShape = OwnedCards.Where(a => a.Shape == info.TrickShape).OrderBy(a => a.Rank).ToList();

            if (specificShape.Count > 0)
            {
                if (FakePlayer)
                    await Task.Delay(Mathf.Max(800, specificShape.Count * Random.Range(300, 400)));

                ChooseCard(specificShape[Random.Range(0, specificShape.Count)]);
            }
            else
            {
                ChooseCard(OwnedCards[Random.Range(0, OwnedCards.Count)]);
            }
        }

    }

    public Card ChooseFirstHand(RoundInfo info)
    {
        Dictionary<Card, int> AllCards = new Dictionary<Card, int>();

        foreach (var item in OwnedCards)
        {
            //int risk = GetRiskfactor(item, info);
            AllCards.Add(item, 0);
        }

        if (balootGameType == BalootGameType.Hokum)
        {
            AllCards = AllCards.OrderBy(a => a.Value).ToDictionary(x => x.Key, x => x.Value);
        }
        else
        {
            
        }

        if (AllCards.Count == 0)
        {
            foreach (var item in OwnedCards)
            {
                Debug.Log(item);
            }

            return null;
        }

        return AllCards.First().Key;
    }

    public int GetCardValueSun(Card card,BalootRoundInfo roundInfo)
    {
        if (IsWinCard(card,roundInfo) == 0)
        {
            return 200;
        }
        else
        {
            return 0;
        }
        //else if()
        //CardHelper.SunValue
    }

    public int IsWinCard(Card card, BalootRoundInfo balootInfo)
    {
        int value = 0;

        for (int i = CardHelper.SunRank[card.Rank] + 1; i <= CardHelper.SunRank[CardRank.Ace]; i++)
        {
            Card higherCard = new Card(card.Shape, CardHelper.SunRank.FirstOrDefault(x => x.Value == i).Key);

            if (OwnedCards.Contains(higherCard) || balootInfo.CardsDrawn.Contains(higherCard))
            {
                continue;
            }
            else
            {
                value++;
            }
        }

        return value;
    }

    public override void Reset()
    {
        cancelDouble = false;
        base.Reset();
    }
}
