//using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DealScript
{
    //public delegate void DealFinished();
    //public event DealFinished OnDealFinished;

    //public delegate void TrickFinished(int winningHand);
    //public event TrickFinished OnTrickFinished;

    //public delegate void CardsDealt(bool waitPass);
    //public event CardsDealt OnCardsDealt;

    //public delegate void CardsPassed();
    //public event CardsPassed OnCardsPassed;

    //public delegate void NextTurn();
    //public event NextTurn OnNextTurn;

    public delegate void Event(EventType eventType);
    public event Event OnEvent;

    public GameState CurrentState { get; private set; }

    Dictionary<int, Card> cardsOnDeck;

    int playingIndex = -1;

    public int PlayingIndex { get => playingIndex; }

    public DealInfo DealInfo;
    int passCardsCount = 0;

    bool isDoubleQueenOfSpades;
    bool isDoubleTenOfDiamonds;

    int doubleCount;

    Player[] players;

    public DealScript()
    {
        cardsOnDeck = new Dictionary<int, Card>();
        DealInfo = new DealInfo();
    }

    public void SetPlayers(Player[] players)
    {
        this.players = players;
    }

    public void StartDeal()
    {
        cardsOnDeck = new Dictionary<int, Card>();

        StartNewGame();
    }

    int noOfCards = 0;
    public void UpdateDealInfo(int playerIndex, Card card)
    {
        noOfCards++;

        DealInfo.CardsOntable.Add(card);
        DealInfo.ShapesOnGround[card.Shape]++;

        cardsOnDeck.Add(playerIndex, card);

        if (card.Shape == CardShape.Heart)
            DealInfo.heartBroken = true;

        if (noOfCards == 1)
        {
            DealInfo.TrickShape = card.Shape;
        }
        else if (noOfCards == 4)
        {
            int value = 0;
            int winningHand = EvaluateDeck(out value);
            cardsOnDeck.Clear();
            players[winningHand].IncrementScore(value);

            DealInfo.DrawCards();
            noOfCards = 0;
        }
    }

    internal void DoubleCard(Card card, bool value)
    {
        if (card.IsQueenOfSpades)
            isDoubleQueenOfSpades = value;
        if (card.IsTenOfDiamonds)
            isDoubleTenOfDiamonds = value;

        doubleCount++;

        if (doubleCount == 2)
        {
            OnEvent?.Invoke(EventType.DoubleCardsFinished);
        }
    }

    public void GameScript_OnCardReady(int playerIndex, Card card)
    {
        Debug.Log(playerIndex + " played " + card.ToString());
        cardsOnDeck.Add(playerIndex, card);

        DealInfo.CardsOntable.Add(card);
        //DealInfo.CardsDrawn.Add(card);
        DealInfo.ShapesOnGround[card.Shape]++;

        if (card.Shape == CardShape.Heart)
            DealInfo.heartBroken = true;

        if (cardsOnDeck.Count == 1)
        {
            DealInfo.TrickShape = card.Shape;
        }

        if (cardsOnDeck.Count == 4)
        {
            int value = 0;
            int winningHand = EvaluateDeck(out value);
            cardsOnDeck.Clear();
            players[winningHand].IncrementScore(value);

            playingIndex = winningHand;

            DealInfo.DrawCards();

            if (DealInfo.roundNumber < 13)
            {
                trickFinished();
            }
            else
            {
                dealFinished();
            }
        }
        else
        {
            playingIndex++;
            playingIndex %= 4;

            //OnNextTurn?.Invoke();
            players[playingIndex].SetTurn(DealInfo);
        }
    }

    public void SetTurn()
    {
        if (DealInfo.roundNumber < 13)
            players[playingIndex].SetTurn(DealInfo);
    }

    async void trickFinished()
    {
        await System.Threading.Tasks.Task.Delay(1000);
        OnEvent?.Invoke(EventType.TrickFinished);
        await System.Threading.Tasks.Task.Delay(1000);
        //players[PlayingIndex].SetTurn(DealInfo, 0);
    }

    async void dealFinished()
    {
        await System.Threading.Tasks.Task.Delay(1000);
        OnEvent?.Invoke(EventType.TrickFinished);
        await System.Threading.Tasks.Task.Delay(1000);
        OnEvent?.Invoke(EventType.DealFinished);
    }

    private int EvaluateDeck(out int value)
    {
        Card winningCard = cardsOnDeck.ElementAt(0).Value;
        int index = cardsOnDeck.ElementAt(0).Key;

        value = 0;

        value += GetValue(winningCard);

        for (int i = 1; i < 4; i++)
        {
            Card currentCard = cardsOnDeck.ElementAt(i).Value;
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

    private int GetValue(Card winningCard)
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

        players[playerIndex].AddPassCards(cards);

        passCardsCount++;

        if (passCardsCount == 4)
        {
            PassingCardsDone();
        }
    }

    public void PassingCardsDone()
    {
        GetStartingIndex();
        OnEvent?.Invoke(EventType.CardsPassed);
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

    public void Deal()
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

        Debug.Log("start Player: " + playingIndex);
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
        StartNewGame();
    }

    public void StartNewGame()
    {
        passCardsCount = 0;
        doubleCount = 0;

        Deal();

        //OnCardsDealt?.Invoke(CurrentState != GameState.DontPass);
        OnEvent?.Invoke(EventType.CardsDealt);

        DealInfo = new DealInfo();

        if (CurrentState == GameState.DontPass)
        {
            players[playingIndex].SetTurn(DealInfo);
        }
        else
        {
            foreach (var item in players)
            {
                item.SelectPassCards();
            }
        }
    }
}

public enum EventType
{
    CardsDealt,
    CardsPassed,
    TrickFinished,
    DealFinished,
    DoubleCardsFinished,
}

public enum GameState
{
    PassRight,
    PassLeft,
    PassAcross,
    DontPass
}
