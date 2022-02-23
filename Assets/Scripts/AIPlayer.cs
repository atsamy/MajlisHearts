using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AIPlayer : Player
{
    public AIPlayer(int index) : base(index)
    {

    }

    public override void SetTurn(DealInfo info, int hand)
    {
        playCard(info, hand);
    }

    async void playCard(DealInfo info, int hand)
    {
        await System.Threading.Tasks.Task.Delay(1000);

        if (hand == 0 && info.roundNumber == 0)
        {
            ChooseCard(new Card(CardShape.Club, CardRank.Two));
            return;
        }

        var sorted = shapeCount.OrderByDescending(a => a.Value);

        if (hand == 0)
        {
            ChooseCard(ChooseFirstHand(info));
        }
        else
        {
            List<Card> specificShape = OwnedCards.Where(a => a.Shape == info.TrickShape).OrderBy(a => a.Rank).ToList();

            if (specificShape.Count > 0)
            {
                ChooseCard(ChooseSpecificShape(specificShape,info));
            }
            else
            {
                specificShape = OwnedCards.Where(a => a.Shape == CardShape.Heart).OrderBy(a => a.Rank).ToList();

                if (OwnedCards.Contains(Card.QueenOfSpades))
                {
                    ChooseCard(Card.QueenOfSpades);
                }
                else if (specificShape.Count > 0)
                {
                    ChooseCard(specificShape.Last());
                }
                else
                {
                    ChooseCard(ChooseRiskyCards(info));
                }
            }
        }

    }

    public void AddWeightToCards()
    {

    }

    public override void SelectPassCards()
    {
        Dictionary<Card, int> weightedCards = new Dictionary<Card, int>();


        foreach (var item in OwnedCards)
        {
            weightedCards.Add(item, (int)item.Rank);
        }

        weightedCards.OrderByDescending(a => a.Value);

        List<Card> passCards = new List<Card>();

        for (int i = 0; i < 3; i++)
        {
            passCards.Add(weightedCards.ElementAt(i).Key);
        }

        PassCards(passCards);
    }


    public Card ChooseRiskyCards(DealInfo info)
    {
        // revisit this code we need to choose hight cards from a stack with few options 

        Dictionary<Card, int> AllCards = new Dictionary<Card, int>();

        foreach (var item in OwnedCards)
        {
            int risk = GetRiskfactor(item, info);
            AllCards.Add(item, risk);
        }

        AllCards.OrderBy(a => a.Value);

        return AllCards.Last().Key;
    }

    public Card ChooseFirstHand(DealInfo info)
    {
        Dictionary<Card, int> AllCards = new Dictionary<Card, int>();

        foreach (var item in OwnedCards)
        {
            if (!info.heartBroken && item.Shape == CardShape.Heart)
                continue;

            int risk = GetRiskfactor(item, info);
            AllCards.Add(item, risk);
        }

        AllCards.OrderBy(a => a.Value);

        return AllCards.First().Key;
        //bug here
        //Card selectedCard = OwnedCards.First(a => a.Shape == selectedShape);

        //ChooseCard(selectedCard);
    }

    public Card ChooseSpecificShape(List<Card> specificShape, DealInfo info)
    {
        Dictionary<Card, int> AllCards = new Dictionary<Card, int>();

        int risk = AvoidHand(info);

        if (risk > 50)
        {
            return GetLeastAvoidCard(info,specificShape);
        }
        else
        {
            return specificShape.Last();
        }
    }

    int GetRiskfactor(Card card, DealInfo info)
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

        int riskToCut = groundShape.Count + sameShape.Count;

        return risk + riskToCut;
    }

    Card GetLeastAvoidCard(DealInfo info, List<Card> specificShape)
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
            //if (info.TrickShape == CardShape.Spade && !info.QueenOfSpade)
            //    return chosenOne;

            chosenOne = specificShape.Last();
        }

        return chosenOne;
    }

    int AvoidHand(DealInfo info)
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
            }
        }
        else
        {
            if (info.TrickShape == CardShape.Spade && !info.QueenOfSpade)
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
}
