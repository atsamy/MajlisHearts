//using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DealScript
{
    public delegate void DealFinished();
    public event DealFinished OnDealFinished;

    public delegate void TrickFinished(int winningHand);
    public event TrickFinished OnTrickFinished;

    public delegate void CardsDealt(bool waitPass);
    public event CardsDealt OnCardsDealt;

    public delegate void CardsPassed();
    public event CardsPassed OnCardsPassed;

    public Player[] Players;
    public GameState CurrentState { get; private set; }

    Dictionary<int, Card> cardsOnDeck;

    int PlayingIndex = -1;

    public DealInfo DealInfo;
    int passCardsCount = 0;

    public void StartDeal()
    {
        Players = new Player[4];
        cardsOnDeck = new Dictionary<int, Card>();

        for (int i = 0; i < 4; i++)
        {
            if (i == 0)
                Players[i] = new MainPlayer(i);
            else
                Players[i] = new AIPlayer(i);

            Players[i].OnPassCardsReady += GameScript_OnPassCardsReady;
            Players[i].OnCardReady += GameScript_OnCardReady;

            Players[i].Name = "Player " + (i + 1);
        }

        StartNewGame();
    }

    private void GameScript_OnCardReady(int playerIndex, Card card)
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
            Players[winningHand].IncrementScore(value);

            PlayingIndex = winningHand;

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
            PlayingIndex++;
            PlayingIndex %= 4;

            Players[PlayingIndex].SetTurn(DealInfo, cardsOnDeck.Count);
        }
    }

    async void trickFinished()
    {
        await System.Threading.Tasks.Task.Delay(1000);
        OnTrickFinished?.Invoke(PlayingIndex);
        await System.Threading.Tasks.Task.Delay(1000);
        Players[PlayingIndex].SetTurn(DealInfo, 0);
    }

    async void dealFinished()
    {
        await System.Threading.Tasks.Task.Delay(1000);
        OnTrickFinished?.Invoke(PlayingIndex);
        await System.Threading.Tasks.Task.Delay(1000);

        //if (CurrentState == GameState.DontPass)
        //    CurrentState = GameState.PassLeft;
        //else
        //    CurrentState++;



        bool isMoonShot = Players.Any(a => a.Score == 26);

        foreach (var item in Players)
        {
            if (isMoonShot)
            {
                if (item.Score == 26)
                {
                    item.Score = 0;
                }
                else
                {
                    item.Score = 13;
                }
            }

            item.SetTotalScore();
        }

        OnDealFinished?.Invoke();
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
        else if (winningCard.Shape == CardShape.Spade && winningCard.Rank == CardRank.Queen)
            return 13;
        return 0;
    }

    private void GameScript_OnPassCardsReady(int playerIndex, List<Card> cards)
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

        Players[playerIndex].AddPassCards(cards);

        passCardsCount++;

        if (passCardsCount == Players.Length)
        {
            OnCardsPassed?.Invoke();
            GetStartingIndex();
            Players[PlayingIndex].SetTurn(DealInfo, 0);
        }
    }

    void GetStartingIndex()
    {
        for (int i = 0; i < Players.Length; i++)
        {
            foreach (var item in Players[i].OwnedCards)
            {
                if (item.Shape == CardShape.Club && item.Rank == CardRank.Two)
                    PlayingIndex = i;
            }
        }
    }

    public void Deal()
    {
        List<Card> AllCards = GetAllCards();


        for (int i = 0; i < Players.Length; i++)
        {
            for (int j = 0; j < 13; j++)
            {
                int getRandom = Random.Range(0, AllCards.Count);

                Players[i].AddCard(AllCards[getRandom]);

                if (PlayingIndex == -1)
                {
                    if (AllCards[getRandom].Shape == CardShape.Club && AllCards[getRandom].Rank == CardRank.Two)
                    {
                        PlayingIndex = i;
                    }
                }

                AllCards.RemoveAt(getRandom);
            }
        }

        Debug.Log("start Player: " + PlayingIndex);
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

        Deal();

        OnCardsDealt?.Invoke(CurrentState != GameState.DontPass);

        DealInfo = new DealInfo();

        if (CurrentState == GameState.DontPass)
        {
            Players[PlayingIndex].SetTurn(DealInfo, 0);
        }
        else
        {
            //OnPassCards?.Invoke();

            foreach (var item in Players)
            {
                item.SelectPassCards();
            }
        }
    }
}

public enum GameState
{
    PassRight,
    PassLeft,
    PassAcross,
    DontPass
}
