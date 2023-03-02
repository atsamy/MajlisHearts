using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class AIPlayerBaloot : PlayerBaloot
{
    public bool FakePlayer { private get; set; }
    bool cancelDouble;
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
            SelectDouble(true, value);
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
        //int dedduct = (3000 - OwnedCards.Count * 230);
        //int time = FakePlayer ? Random.Range(1000, 4000 - dedduct) : 1000;
        //await Task.Delay(time);
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
                //specificShape = OwnedCards.Where(a => a.Shape == CardShape.Heart).OrderBy(a => a.Rank).ToList();
                //bool isTeamPlayer = checkIfTeamPlayer(info, hand);

                ChooseCard(OwnedCards[Random.Range(0, OwnedCards.Count)]);
                //if (OwnedCards.Contains(Card.QueenOfSpades))// && info.roundNumber != 0 && !isTeamPlayer)
                //{
                //    if (FakePlayer)
                //        await Task.Delay(Random.Range(800, 1200));
                //    ChooseCard(Card.QueenOfSpades);
                //}
                //else if (OwnedCards.Contains(Card.TenOfDiamonds))// && info.roundNumber != 0 && !isTeamPlayer)
                //{
                //    if (FakePlayer)
                //        await Task.Delay(Random.Range(800, 1200));
                //    ChooseCard(Card.TenOfDiamonds);
                //}
                //else if (specificShape.Count > 0)// && info.roundNumber != 0 && !isTeamPlayer)
                //{
                //    if (FakePlayer)
                //        await Task.Delay(Random.Range(800, 1200));
                //    ChooseCard(specificShape.Last());
                //}
                //else
                //{
                //    if (FakePlayer)
                //        await Task.Delay(Mathf.Max(800, OwnedCards.Count * Random.Range(200, 400)));
                //    ChooseCard(ChooseRiskyCards(info));
                //}
            }
        }

    }

    public Card ChooseRiskyCards(RoundInfo info)
    {
        // revisit this code we need to choose hight cards from a stack with few options
        Dictionary<Card, int> AllCards = new Dictionary<Card, int>();

        foreach (var item in OwnedCards)
        {
            int risk = GetRiskfactor(item, info);
            AllCards.Add(item, risk);
        }
        //Debug.Log("AI Cards:"+ OwnedCards.Count + " " + AllCards.Count);

        AllCards = AllCards.OrderBy(a => a.Value).ToDictionary(x => x.Key, x => x.Value);

        return AllCards.Last().Key;
    }

    public Card ChooseFirstHand(RoundInfo info)
    {
        Dictionary<Card, int> AllCards = new Dictionary<Card, int>();

        foreach (var item in OwnedCards)
        {
            int risk = GetRiskfactor(item, info);
            AllCards.Add(item, risk);
        }
        AllCards = AllCards.OrderBy(a => a.Value).ToDictionary(x => x.Key, x => x.Value);

        if (AllCards.Count == 0)
        {
            foreach (var item in OwnedCards)
            {
                Debug.Log(item);
            }

            return null;
        }

        return AllCards.First().Key;
        //bug here
    }

    public Card ChooseSpecificShape(List<Card> specificShape, RoundInfo info)
    {
        Dictionary<Card, int> AllCards = new Dictionary<Card, int>();

        int risk = AvoidHand(info);

        if (risk > 50)
        {
            return GetLeastAvoidCard(info, specificShape);
        }
        else
        {
            return specificShape.Last();
        }
    }

    int GetRiskfactor(Card card, RoundInfo info)
    {
        int risk = (int)card.Rank;

        List<Card> sameShape = OwnedCards.Where(a => a.Shape == card.Shape).ToList();

        foreach (var item in sameShape)
        {
            if (item.Rank < card.Rank)
                risk--;
        }

        List<Card> groundShape = info.CardsDrawn.Where(a => a.Shape == card.Shape).ToList();

        foreach (var item in groundShape)
        {
            if (item.Rank < card.Rank)
                risk--;
        }

        if (((Card)card).IsQueenOfSpades || ((Card)card).IsTenOfDiamonds)
            risk += 100;

        int riskToCut = groundShape.Count + sameShape.Count;

        return risk + riskToCut;
    }

    Card GetLeastAvoidCard(RoundInfo info, List<Card> specificShape)
    {
        Card HighestCard = new Card(info.TrickShape, CardRank.Two);

        foreach (var item in info.CardsOntable)
        {
            if (item.Shape == info.TrickShape)
            {
                if (item.Rank > HighestCard.Rank)
                {
                    HighestCard = item;
                }
            }
        }

        Card chosenOne = specificShape.First();
        bool canAvoid = false;

        foreach (var item in specificShape)
        {
            if (item.Rank < HighestCard.Rank)
            {
                chosenOne = item;
                canAvoid = true;
            }
        }

        if (!canAvoid && info.CardsOntable.Count == 3)
        {
            chosenOne = specificShape.Last();
        }

        return chosenOne;
    }

    int AvoidHand(RoundInfo info)
    {
        int avoidWeight = 0;

        if (info.CardsOntable.Count == 3)
        {
            foreach (var item in info.CardsOntable)
            {
                if (item.Shape == CardShape.Heart)
                {
                    avoidWeight += 50;
                }
                else if (item.IsQueenOfSpades)
                {
                    avoidWeight += 100;
                }
                else if (item.IsTenOfDiamonds)
                {
                    avoidWeight += 100;
                }
            }
        }
        else
        {
            if (info.TrickShape == CardShape.Spade && !info.QueenOfSpade)
            {
                avoidWeight += 100;
            }
            else if (info.TrickShape == CardShape.Diamond && !info.TenOfDiamonds)
            {
                avoidWeight += 100;
            }
            else if (info.TrickShape == CardShape.Heart)
            {
                avoidWeight += 100;
            }
            else
            {
                avoidWeight = info.ShapesOnGround[info.TrickShape] + GetShapeCount(info.TrickShape);
            }
        }

        return avoidWeight;
    }

    public override void Reset()
    {
        cancelDouble = false;
        base.Reset();
    }
}
