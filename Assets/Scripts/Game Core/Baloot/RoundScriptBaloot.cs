//using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoundScriptBaloot : RoundScriptBase
{
    public int StartIndex;
    public BalootRoundInfo balootRoundInfo => (BalootRoundInfo)RoundInfo;


    public Card BalootCard;
    List<Card> AllCards;

    public BalootGameType RoundType => balootRoundInfo.BalootRoundType;

    public int BidingTeam { get; internal set; }
    public int OtherTeam => (BidingTeam + 1) % 2;
    public int BiddingRound;
    public int HokumIndex;

    public int AllTricks = -1;

    public event System.Action<int, BalootGameType> OnGameTypeSelected;

    public RoundScriptBaloot()
    {
        cardsOnDeck = new Dictionary<int, Card>();
        RoundInfo = new BalootRoundInfo();
        playingIndex = 0;
        StartIndex = -1;
    }


    public override void Deal()
    {
        AllCards = GetAllCards();

        players[0].Reset();
        players[0].AddCard(new Card(CardShape.Club,CardRank.Knight));
        AllCards.Remove(new Card(CardShape.Club, CardRank.Knight));

        players[0].AddCard(new Card(CardShape.Club, CardRank.Queen));
        AllCards.Remove(new Card(CardShape.Club, CardRank.Queen));

        players[0].AddCard(new Card(CardShape.Club, CardRank.King));
        AllCards.Remove(new Card(CardShape.Club, CardRank.King));

        players[0].AddCard(new Card(CardShape.Heart, CardRank.Ten));
        AllCards.Remove(new Card(CardShape.Heart, CardRank.Ten));

        players[0].AddCard(new Card(CardShape.Diamond, CardRank.Nine));
        AllCards.Remove(new Card(CardShape.Diamond, CardRank.Nine));

        players[3].AddCard(new Card(CardShape.Heart, CardRank.Ace));
        AllCards.Remove(new Card(CardShape.Heart, CardRank.Ace));

        players[3].AddCard(new Card(CardShape.Heart, CardRank.Knight));
        AllCards.Remove(new Card(CardShape.Heart, CardRank.Knight));

        players[3].AddCard(new Card(CardShape.Heart, CardRank.Queen));
        AllCards.Remove(new Card(CardShape.Heart, CardRank.Queen));

        players[3].AddCard(new Card(CardShape.Diamond, CardRank.Knight));
        AllCards.Remove(new Card(CardShape.Diamond, CardRank.Knight));

        players[3].AddCard(new Card(CardShape.Diamond, CardRank.Queen));
        AllCards.Remove(new Card(CardShape.Diamond, CardRank.Queen));

        for (int i = 1; i < players.Length - 1; i++)
        {
            players[i].Reset();

            for (int j = 0; j < 5; j++)
            {
                int getRandom = Random.Range(0, AllCards.Count);

                players[i].AddCard(AllCards[getRandom]);
                AllCards.RemoveAt(getRandom);
            }
        }

        BalootCard = AllCards[Random.Range(0, AllCards.Count)];
        AllCards.Remove(BalootCard);
    }

    //public override void Deal()
    //{
    //    AllCards = GetAllCards();

    //    for (int i = 0; i < players.Length; i++)
    //    {
    //        players[i].Reset();

    //        for (int j = 0; j < 5; j++)
    //        {
    //            int getRandom = Random.Range(0, AllCards.Count);

    //            players[i].AddCard(AllCards[getRandom]);
    //            AllCards.RemoveAt(getRandom);
    //        }
    //    }

    //    BalootCard = AllCards[Random.Range(0, AllCards.Count)];
    //    AllCards.Remove(BalootCard);
    //}

    public void IncrementStartIndex()
    {
        StartIndex++;
        StartIndex %= 4;
    }

    public override int EvaluateDeck(out int value)
    {
        Card winningCard = cardsOnDeck.ElementAt(0).Value;
        int index = cardsOnDeck.ElementAt(0).Key;

        value = 0;
        value += CardHelper.GetCardValue(balootRoundInfo.BalootRoundType, winningCard);

        if (balootRoundInfo.BalootRoundType == BalootGameType.Hokum)
        {
            for (int i = 1; i < 4; i++)
            {
                Card currentCard = cardsOnDeck.ElementAt(i).Value;
                value += CardHelper.GetCardValue(balootRoundInfo.BalootRoundType, currentCard);
                if (currentCard.Shape == winningCard.Shape)
                {
                    if (CardHelper.GetCardRank(balootRoundInfo.BalootRoundType, currentCard) >
                        CardHelper.GetCardRank(balootRoundInfo.BalootRoundType, winningCard))
                    {
                        index = cardsOnDeck.ElementAt(i).Key;
                        winningCard = currentCard;
                    }
                }
                else if (currentCard.Shape == balootRoundInfo.HokumShape)
                {
                    if (winningCard.Shape != balootRoundInfo.HokumShape)
                    {
                        winningCard = currentCard;
                        index = cardsOnDeck.ElementAt(i).Key;
                    }
                    else if (CardHelper.GetCardRank(balootRoundInfo.BalootRoundType, currentCard) >
                        CardHelper.GetCardRank(balootRoundInfo.BalootRoundType, winningCard))
                    {
                        winningCard = currentCard;
                        index = cardsOnDeck.ElementAt(i).Key;
                    }
                }
            }
        }
        else
        {
            for (int i = 1; i < 4; i++)
            {
                Card currentCard = cardsOnDeck.ElementAt(i).Value;
                value += CardHelper.GetCardValue(balootRoundInfo.BalootRoundType, currentCard);
                if (currentCard.Shape == winningCard.Shape)
                {
                    if (CardHelper.GetCardRank(balootRoundInfo.BalootRoundType, currentCard) > CardHelper.GetCardRank(balootRoundInfo.BalootRoundType, winningCard))
                    {
                        index = cardsOnDeck.ElementAt(i).Key;
                        winningCard = currentCard;
                    }
                }
            }
        }
        return index;
    }

    public override void OnCardReady(int playerIndex, Card card)
    {
        cardsOnDeck.Add(playerIndex, card);
        RoundInfo.CardsOntable.Add(card);
        RoundInfo.ShapesOnGround[card.Shape]++;

        if (cardsOnDeck.Count == 1)
        {
            RoundInfo.TrickShape = card.Shape;
        }

        if (cardsOnDeck.Count == 4)
        {
            int value = 0;
            int winningHand = EvaluateDeck(out value);
            cardsOnDeck.Clear();
            players[winningHand].IncrementScore(value);
            players[winningHand].IncrementTricks();

            playingIndex = winningHand;


            if (RoundInfo.TrickNumber < 7)
            {
                TrickFinished((int)EventTypeBaloot.TrickFinished);
            }
            else
            {
                players[winningHand].IncrementScore(10);
                DealFinished((int)EventTypeBaloot.TrickFinished, (int)EventTypeBaloot.DealFinished);
            }

            //RoundInfo.DrawCards();
        }
        else
        {
            playingIndex++;
            playingIndex %= 4;

            //OnNextTurn?.Invoke();
            players[playingIndex].SetTurn(RoundInfo);
        }
    }

    public void DealContinue(int playerIndex)
    {
        //Debug.Log("deal continue called");
        players[playerIndex].AddCard(BalootCard);
        int index = playerIndex;

        while (AllCards.Count > 0)
        {
            index++;
            index %= 4;
            int getRandom = Random.Range(0, AllCards.Count);
            players[index].AddCard(AllCards[getRandom]);
            AllCards.RemoveAt(getRandom);
        }

        foreach (PlayerBaloot player in players) 
        {
            if (player is AIPlayerBaloot)
            {
                ((AIPlayerBaloot)player).SetGameType(RoundType);
            }
        }

        OnEvent?.Invoke((int)EventTypeBaloot.CardsDealtFinished);

        //players[StartIndex].SetTurn(RoundInfo);
    }

    public void ResetValues()
    {
        BiddingRound = 0;
        HokumIndex = -1;
    }

    public override void StartNewRound()
    {
        ResetValues();
        IncrementStartIndex();
        Deal();

        RoundInfo = new BalootRoundInfo();
        balootRoundInfo.HokumShape = BalootCard.Shape;
        OnEvent?.Invoke((int)EventTypeBaloot.CardsDealtBegin);
    }

    private List<Card> GetAllCards()
    {
        List<Card> cards = new List<Card>();

        for (int i = 0; i < 4; i++)
        {
            for (int j = 5; j < 13; j++)
            {
                cards.Add(new Card((CardShape)i, (CardRank)j));
            }
        }

        return cards;
    }

    internal void SetGameType(int index, BalootGameType type)
    {
        balootRoundInfo.BalootRoundType = type;
        playingIndex = StartIndex;
        BidingTeam = (index == 0 || index == 2) ? 0 : 1;

        OnGameTypeSelected?.Invoke(index,type);
    }

    //internal void ContinueDeal(int index)
    //{
    //    DealContinue(index);
    //}


    //bool hokumConfirmed;
    internal void PlayerSelectedType(int index, BalootGameType type)
    {
        //but only increment when all players
        int lastIndex = StartIndex - 1;

        if (lastIndex < 0)
            lastIndex += 4;

        if (index == lastIndex)
            BiddingRound++;

        int nextIndex = (index + 1) % 4;

        switch (type)
        {
            case BalootGameType.Sun:
                SetGameType(index, type);
                break;
            case BalootGameType.Hokum:
                if (HokumIndex == -1)
                {
                    HokumIndex = index;
                    ((PlayerBaloot)players[nextIndex]).CheckGameType(this);
                }
                else
                {
                    SetGameType(index, type);
                }
                break;
            case BalootGameType.Ashkal:
                SetGameType((index + 2) % 4, BalootGameType.Sun);
                break;
            case BalootGameType.Pass:
                if (HokumIndex == index)
                    HokumIndex = -1;

                if (StartIndex == nextIndex && BiddingRound == 2 && HokumIndex == -1)
                {
                    //start new deal
                    BiddingRound = 0;
                    OnEvent?.Invoke((int)EventTypeBaloot.RestartDeal);
                }
                else
                {
                    ((PlayerBaloot)players[nextIndex]).CheckGameType(this);
                }
                break;
        }
    }

    internal override void StartFirstTurn()
    {
        players[StartIndex].SetTurn(RoundInfo);
    }
}

public class BalootRoundInfo : RoundInfo
{
    public BalootGameType BalootRoundType;
    public CardShape HokumShape;

    public BalootRoundInfo() : base()
    {

    }
}

public enum EventTypeBaloot
{
    CardsDealtBegin,
    CardsDealtFinished,
    RestartDeal,
    TrickFinished,
    DealFinished
}
