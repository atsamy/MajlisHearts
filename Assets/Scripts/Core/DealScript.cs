//using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DealScript
{
    public delegate void DealFinished();
    public event DealFinished OnDealFinished;

    public Player[] Players;
    GameState currentState;

    Dictionary<int, Card> cardsOnDeck;

    int PlayingIndex = -1;

    public TrickInfo TrickInfo;

    public void StartDeal()
    {
        Players = new Player[4];
        cardsOnDeck = new Dictionary<int, Card>();

        for (int i = 0; i < 4; i++)
        {
            Players[i] = new Player(i);
            Players[i].OnPassCardsReady += GameScript_OnPassCardsReady;
            Players[i].OnCardReady += GameScript_OnCardReady;
        }

        Deal();

        Players[PlayingIndex].SetTurn(0);

        TrickInfo = new TrickInfo();
    }

    private void GameScript_OnCardReady(int playerIndex, Card card)
    {
        cardsOnDeck.Add(playerIndex, card);

        if (card.Shape == CardShape.Heart)
            TrickInfo.heartBroken = true;

        if (cardsOnDeck.Count == 1)
        {
            TrickInfo.TrickShape = card.Shape;
        }

        if (cardsOnDeck.Count == 4)
        {
            int value = 0;
            int winningHand = EvaluateDeck(out value);

            Players[winningHand].IncrementScore(value);

            PlayingIndex = winningHand;
            TrickInfo.roundNumber++;

            if (TrickInfo.roundNumber < 13)
            {
                Players[PlayingIndex].SetTurn(0);
            }
            else
            {
                currentState++;

                if (currentState == GameState.DontPass)
                    currentState = GameState.PassLeft;

                OnDealFinished?.Invoke();
            }
        }
        else
        {
            PlayingIndex++;
            playerIndex %= 4;

            Players[PlayingIndex].SetTurn(cardsOnDeck.Count);
        }
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
        switch (currentState)
        {
            case GameState.PassLeft:
                playerIndex++;
                playerIndex %= 4;
                break;
            case GameState.PassRight:
                playerIndex += 3;
                playerIndex %= 4;
                break;
            case GameState.PassAcross:
                playerIndex += 2;
                playerIndex %= 4;
                break;
        }


        Players[playerIndex].AddCards(cards);
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
}

public enum GameState
{
    PassLeft,
    PassRight,
    PassAcross,
    DontPass,

}
