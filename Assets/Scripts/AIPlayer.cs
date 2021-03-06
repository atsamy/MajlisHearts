using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;

public class AIPlayer : Player
{
    //List<Card> passCards;
    public AIPlayer(int index) : base(index)
    {
        isPlayer = false;
    }

    public override void SetTurn(DealInfo info)
    {
        playCard(info);
    }

    async void playCard(DealInfo info)
    {
        await System.Threading.Tasks.Task.Delay(1000);

        int hand = info.CardsOntable.Count;

        if (hand == 0 && info.roundNumber == 0)
        {
            ChooseCard(new Card(CardShape.Club, CardRank.Two));
            return;
        }

        shapeCount = shapeCount.OrderByDescending(a => a.Value).ToDictionary(x => x.Key, x => x.Value);

        if (hand == 0)
        {
            ChooseCard(ChooseFirstHand(info));
        }
        else
        {
            List<Card> specificShape = OwnedCards.Where(a => a.Shape == info.TrickShape).OrderBy(a => a.Rank).ToList();

            if (specificShape.Count > 0)
            {
                ChooseCard(ChooseSpecificShape(specificShape, info));
            }
            else
            {
                specificShape = OwnedCards.Where(a => a.Shape == CardShape.Heart).OrderBy(a => a.Rank).ToList();

                if (OwnedCards.Contains(Card.QueenOfSpades) && info.roundNumber != 0)
                {
                    ChooseCard(Card.QueenOfSpades);
                }
                else if (OwnedCards.Contains(Card.TenOfDiamonds) && info.roundNumber != 0)
                {
                    ChooseCard(Card.TenOfDiamonds);
                }
                else if (specificShape.Count > 0 && info.roundNumber != 0)
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

        Dictionary<CardShape, int> ShapesValue = new Dictionary<CardShape, int>();

        for (int i = 0; i < 4; i++)
        {
            CardShape shape = (CardShape)i;
            List<Card> cards = OwnedCards.Where(a => a.Shape == shape).ToList();

            int totalValue = 0;

            foreach (var item in cards)
            {
                totalValue += (int)item.Rank;
            }

            ShapesValue.Add(shape, totalValue);
        }



        foreach (var item in OwnedCards)
        {
            weightedCards.Add(item, CalculatePassCardsRisk(item, ShapesValue[item.Shape]));
            //UIManager.Instance.AddDebugWeight(Index - 1, item, weightedCards.Last().Value);
        }

        weightedCards = weightedCards.OrderByDescending(a => a.Value).ToDictionary(x => x.Key, x => x.Value);

        List<Card> passCards = new List<Card>();

        for (int i = 0; i < 3; i++)
        {
            passCards.Add(weightedCards.ElementAt(i).Key);
        }

        PassAICards(passCards);
    }

    async void PassAICards(List<Card> passCards)
    {
        await Task.Yield();
        PassCards(passCards);
    }

    int CalculatePassCardsRisk(Card card, int totalValue)
    {
        if (totalValue == 0)
            return 0;

        if (card.Shape == CardShape.Spade)
        {
            return (int)card.Rank * 150 / totalValue;
        }
        else if (card.Shape == CardShape.Diamond)
        {
            return (int)card.Rank * 120 / totalValue;
        }
        else
        {
            return (int)card.Rank * 100 / totalValue;
        }
    }

    protected override void CheckDoubleCards(Card card)
    {
        float value = 0;

        if (shapeCount[card.Shape] > 4)
        {
            value = Random.value;
        }

        SetDoubleCard(card, value > 0.6f);
    }

    public Card ChooseRiskyCards(DealInfo info)
    {
        // revisit this code we need to choose hight cards from a stack with few options 

        Dictionary<Card, int> AllCards = new Dictionary<Card, int>();

        Debug.Log("cards count: " + OwnedCards.Count);

        foreach (var item in OwnedCards)
        {
            if (item.Shape == CardShape.Heart)
                continue;

            int risk = GetRiskfactor(item, info);
            AllCards.Add(item, risk);
        }

        AllCards = AllCards.OrderBy(a => a.Value).ToDictionary(x => x.Key, x => x.Value);

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

        AllCards = AllCards.OrderBy(a => a.Value).ToDictionary(x => x.Key, x => x.Value);

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
            return GetLeastAvoidCard(info, specificShape);
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

        if (card.IsQueenOfSpades || card.IsTenOfDiamonds)
            risk += 100;

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

    public void MergeFromPlayer(Player player)
    {
        OwnedCards = player.OwnedCards;
        Score = player.Score;
        TotalScore = player.TotalScore;
        shapeCount = player.ShapeCount;
        DidLead = player.DidLead;
        Name = player.Name;

        OnDoubleCard = player.OnDoubleCard;
        OnCardReady = player.OnCardReady;
        OnPassCardsReady = player.OnPassCardsReady;
    }
}
