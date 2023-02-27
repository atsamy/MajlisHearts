//using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoundScriptBaloot : RoundScriptBase
{
    public int StartIndex;
    BalootRoundInfo balootRoundInfo => (BalootRoundInfo)RoundInfo;

    public CardShape HokumShape;
    public Card BalootCard;
    List<Card> AllCards;

    public BalootGameType RoundType => balootRoundInfo.BalootRoundType;

    public int BidingTeam { get; internal set; }
    public int BiddingRound { get; private set; }
    public int HokumIndex { get; private set; }

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
        HokumIndex = -1;
        AllCards = GetAllCards();

        for (int i = 0; i < players.Length; i++)
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

    private void IncrementStartIndex()
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
                else if (currentCard.Shape == HokumShape)
                {
                    if (winningCard.Shape != HokumShape)
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

            playingIndex = winningHand;

            RoundInfo.DrawCards();

            if (RoundInfo.TrickNumber < 8)
            {
                TrickFinished((int)EventTypeBaloot.TrickFinished);
            }
            else
            {
                players[winningHand].IncrementScore(10);
                DealFinished((int)EventTypeBaloot.TrickFinished, (int)EventTypeBaloot.DealFinished);
            }
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

        OnEvent?.Invoke((int)EventTypeBaloot.CardsDealtFinished);

        players[StartIndex].SetTurn(RoundInfo);
    }

    public override void StartNewRound()
    {
        IncrementStartIndex();

        Deal();
        HokumShape = BalootCard.Shape;
        RoundInfo = new BalootRoundInfo();

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
        if (index == StartIndex)
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
                    ((PlayerBaloot)players[nextIndex]).CheckGameType();
                }
                else
                    SetGameType(index, type);
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
                    ((PlayerBaloot)players[nextIndex]).CheckGameType();
                }
                break;
        }
    }
}

public class BalootRoundInfo : RoundInfo
{
    public BalootGameType BalootRoundType;

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
