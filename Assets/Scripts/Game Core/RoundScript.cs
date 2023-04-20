//using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoundScriptHearts: RoundScriptBase
{
    //public delegate void Event(EventType eventType);
    //public event Event OnEvent;

    public GameState CurrentState { get; private set; }
    int passCardsCount = 0;

    bool isDoubleQueenOfSpades;
    bool isDoubleTenOfDiamonds;

    int doubleCount;

    public RoundScriptHearts()
    {
        cardsOnDeck = new Dictionary<int, Card>();
        RoundInfo = new RoundInfo();
    }

    public override void StartRound()
    {
        cardsOnDeck = new Dictionary<int, Card>();

        StartNewRound();
    }


    //public void UpdateDealInfo(int playerIndex, Card card)
    //{
    //    noOfCards++;

    //    RoundInfo.CardsOntable.Add(card);
    //    RoundInfo.ShapesOnGround[card.Shape]++;

    //    cardsOnDeck.Add(playerIndex, card);

    //    if (noOfCards == 1)
    //    {
    //        RoundInfo.TrickShape = card.Shape;
    //    }
    //    else if (noOfCards == 4)
    //    {
    //        int value = 0;
    //        int winningHand = EvaluateDeck(out value);
    //        cardsOnDeck.Clear();
    //        players[winningHand].IncrementScore(value);

    //        RoundInfo.DrawCards();
    //        noOfCards = 0;
    //    }
    //}

    internal void DoubleCard(Card card, bool value)
    {
        if (card.IsQueenOfSpades)
            isDoubleQueenOfSpades = value;
        if (card.IsTenOfDiamonds)
            isDoubleTenOfDiamonds = value;

        doubleCount++;

        if (doubleCount == 2)
        {
            OnEvent?.Invoke((int)EventType.DoubleCardsFinished);
        }
    }

    public override void OnCardReady(int playerIndex, Card card)
    {
        //Debug.Log(playerIndex + " " + card);
        cardsOnDeck.Add(playerIndex, card);
        RoundInfo.CardsOntable.Add(card);
        RoundInfo.ShapesOnGround[card.Shape]++;
        
        //Debug.Log(cardsOnDeck.Count);
        
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
            //Debug.Log("trick " + RoundInfo.TrickNumber);
            //RoundInfo.DrawCards();
            if (RoundInfo.TrickNumber < 12)
            {
                TrickFinished((int)EventType.TrickFinished);
            }
            else
            {
                DealFinished((int)EventType.TrickFinished, (int)EventType.DealFinished);
            }
            //RoundInfo.TrickNumber++;
        }
        else
        {
            playingIndex++;
            playingIndex %= 4;
            players[playingIndex].SetTurn(RoundInfo);
        }
    }

    public override async void TrickFinished(int TrickFinished)
    {
        await System.Threading.Tasks.Task.Delay(1000);

        RoundInfo.DrawCards();
        OnEvent?.Invoke(TrickFinished);
    }

    public override async void DealFinished(int TrickFinished, int DealFinished)
    {
        await System.Threading.Tasks.Task.Delay(1000);
        RoundInfo.DrawCards();
        OnEvent?.Invoke(TrickFinished);
        await System.Threading.Tasks.Task.Delay(1000);
        OnEvent?.Invoke(DealFinished);
    }

    internal override void StartFirstTurn()
    {
        players[playingIndex].SetTurn(RoundInfo);
    }

    public override void SetTurn()
    {
        if (RoundInfo.TrickNumber < 13)
            players[playingIndex].SetTurn(RoundInfo);
    }

    public override int EvaluateDeck(out int value)
    {
        Card winningCard = (Card)cardsOnDeck.ElementAt(0).Value;
        int index = cardsOnDeck.ElementAt(0).Key;

        value = 0;

        value += GetValue(winningCard);

        for (int i = 1; i < 4; i++)
        {
            Card currentCard = (Card)cardsOnDeck.ElementAt(i).Value;
            value += GetValue(currentCard);
            if (currentCard.Shape == winningCard.Shape)
            {
                if (currentCard.Rank > winningCard.Rank)
                {
                    index = cardsOnDeck.ElementAt(i).Key;
                    winningCard = currentCard;
                }
            }
        }

        return index;
    }

    public override int GetValue(Card winningCard)
    {
        if (winningCard.Shape == CardShape.Heart)
            return 1;
        else if (winningCard.IsQueenOfSpades)
            return 13 * (isDoubleQueenOfSpades ? 2 : 1);
        else if (winningCard.IsTenOfDiamonds)
            return 10 * (isDoubleTenOfDiamonds ? 2 : 1);
        return 0;
    }

    public void GameScript_OnPassCardsReady(int playerIndex, List<Card> cards)
    {
        switch (CurrentState)
        {
            case GameState.PassRight:
                playerIndex++;
                playerIndex %= 4;
                break;
            case GameState.PassLeft:
                playerIndex += 3;
                playerIndex %= 4;
                break;
            case GameState.PassAcross:
                playerIndex += 2;
                playerIndex %= 4;
                break;
        }

        ((Player)players[playerIndex]).AddPassCards(cards);

        passCardsCount++;

        if (passCardsCount == 4)
        {
            PassingCardsDone();
        }
    }

    public void PassingCardsDone()
    {
        GetStartingIndex();
        OnEvent?.Invoke((int)EventType.CardsPassed);
        //players[playingIndex].SetTurn(DealInfo, 0);
    }

    void GetStartingIndex()
    {
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].HasCard(new Card(CardShape.Club, CardRank.Two)))
            {
                playingIndex = i;
            }
        }
    }

    public override void Deal()
    {
        List<Card> AllCards = GetAllCards();

        for (int i = 0; i < players.Length; i++)
        {
            players[i].Reset();

            for (int j = 0; j < 13; j++)
            {
                int getRandom = Random.Range(0, AllCards.Count);

                players[i].AddCard(AllCards[getRandom]);
                AllCards.RemoveAt(getRandom);
            }
        }
    }

    private List<Card> GetAllCards()
    {
        List<Card> cards = new List<Card>();

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 13; j++)
            {
                cards.Add(new Card((CardShape)i, (CardRank)j));
            }
        }

        return cards;
    }

    public void Reset()
    {
        StartNewRound();
    }

    public override void StartNewRound()
    {
        passCardsCount = 0;
        doubleCount = 0;

        Deal();
        OnEvent?.Invoke((int)EventType.CardsDealt);

        RoundInfo = new RoundInfo();

        if (CurrentState == GameState.DontPass)
        {
            players[playingIndex].SetTurn(RoundInfo);
        }
        else
        {
            foreach (var item in players)
            {
                ((Player)item).SelectPassCards();
            }
        }
    }
}

public enum EventType
{
    CardsDealt = 0,
    CardsPassed = 1,
    TrickFinished = 2,
    DealFinished = 3,
    DoubleCardsFinished = 4,
}

public enum GameState
{
    PassRight,
    PassLeft,
    PassAcross,
    DontPass
}
