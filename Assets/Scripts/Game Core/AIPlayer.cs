using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;

public class AIPlayer : Player
{
    public bool FakePlayer { private get; set; }

    public AIPlayer(int index) : base(index)
    {
        isPlayer = false;
    }

    public override void SetTurn(DealInfo info)
    {
        base.SetTurn(info);
        playCard(info);
    }

    async void playCard(DealInfo info)
    {
        //int dedduct = (3000 - OwnedCards.Count * 230);
        //int time = FakePlayer ? Random.Range(1000, 4000 - dedduct) : 1000;
        //await Task.Delay(time);
        if(!FakePlayer)
            await Task.Delay(1000);

        int hand = info.CardsOntable.Count;

        if (hand == 0 && info.roundNumber == 0)
        {
            if (FakePlayer)
                await Task.Delay(600);

            ChooseCard(new Card(CardShape.Club, CardRank.Two));
            return;
        }

        shapeCount = shapeCount.OrderByDescending(a => a.Value).ToDictionary(x => x.Key, x => x.Value);

        if (hand == 0)
        {
            if (FakePlayer)
                await Task.Delay(Mathf.Max(800, OwnedCards.Count * Random.Range(250,300)));

            ChooseCard(ChooseFirstHand(info));
        }
        else
        {
            List<Card> specificShape = OwnedCards.Where(a => a.Shape == info.TrickShape).OrderBy(a => a.Rank).ToList();

            if (specificShape.Count > 0)
            {
                if (FakePlayer)
                    await Task.Delay(Mathf.Max(800, specificShape.Count * Random.Range(300, 400)));
                ChooseCard(ChooseSpecificShape(specificShape, info));
            }
            else
            {
                specificShape = OwnedCards.Where(a => a.Shape == CardShape.Heart).OrderBy(a => a.Rank).ToList();
                bool isTeamPlayer = checkIfTeamPlayer(info,hand);


                if (OwnedCards.Contains(Card.QueenOfSpades))// && info.roundNumber != 0 && !isTeamPlayer)
                {
                    if (FakePlayer)
                        await Task.Delay(Random.Range(800, 1200));
                    ChooseCard(Card.QueenOfSpades);
                }
                else if (OwnedCards.Contains(Card.TenOfDiamonds))// && info.roundNumber != 0 && !isTeamPlayer)
                {
                    if (FakePlayer)
                        await Task.Delay(Random.Range(800, 1200));
                    ChooseCard(Card.TenOfDiamonds);
                }
                else if (specificShape.Count > 0)// && info.roundNumber != 0 && !isTeamPlayer)
                {
                    if (FakePlayer)
                        await Task.Delay(Random.Range(800, 1200));
                    ChooseCard(specificShape.Last());
                }
                else
                {
                    if (FakePlayer)
                        await Task.Delay(Mathf.Max(800, OwnedCards.Count * Random.Range(200, 400)));
                    ChooseCard(ChooseRiskyCards(info));
                }
            }
        }

    }

    public void AddWeightToCards()
    {

    }

    bool checkIfTeamPlayer(DealInfo info,int hand)
    {
        if (GameManager.Instance.IsTeam)
        {
            if (hand > 1)
            {
                int hightestCard = 0;

                CardShape shape = info.CardsOntable.First().Shape;
                CardRank heighestRank = info.CardsOntable.First().Rank;

                for (int i = 1; i < info.CardsOntable.Count; i++)
                {
                    if (info.CardsOntable[i].Shape == shape && info.CardsOntable[i].Rank > heighestRank)
                    {
                        hightestCard = i;
                        heighestRank = info.CardsOntable[i].Rank;
                    }
                }

                return hightestCard == (hand - 2);
            }
            else return false;
        }
        else
        {
            return false;
        }
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
            if (card.IsQueenOfSpades)
            {
                return (int)card.Rank * Mathf.Max(300 - shapeCount[CardShape.Spade] * 50,50) / totalValue;
            }
            return (int)card.Rank * 150 / totalValue;
        }
        else if (card.Shape == CardShape.Diamond)
        {
            if (card.IsTenOfDiamonds)
            {
                return (int)card.Rank * Mathf.Max(270 - shapeCount[CardShape.Diamond] * 50, 20) / totalValue;
            }
            return (int)card.Rank * 120 / totalValue;
        }
        else
        {
            return (int)card.Rank * 100 / totalValue;
        }
    }

    protected override void CheckDoubleCards(Card card)
    {
         DecideOnCard(card);
    }

    void DecideOnCard(Card card)
    {
        //await System.Threading.Tasks.Task.Delay(2000);

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

        foreach (var item in OwnedCards)
        {
            int risk = GetRiskfactor(item, info);
            AllCards.Add(item, risk);
        }
        //Debug.Log("AI Cards:"+ OwnedCards.Count + " " + AllCards.Count);

        AllCards = AllCards.OrderBy(a => a.Value).ToDictionary(x => x.Key, x => x.Value);

        return AllCards.Last().Key;
    }

    public Card ChooseFirstHand(DealInfo info)
    {
        Dictionary<Card, int> AllCards = new Dictionary<Card, int>();

        foreach (var item in OwnedCards)
        {
            //if (!info.heartBroken && item.Shape == CardShape.Heart && !HasOnlyHearts())
            //    continue;

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
