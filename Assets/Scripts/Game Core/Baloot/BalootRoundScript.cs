using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BalootRoundScript:RoundScriptBase
{
    public int StartIndex;
    BalootRoundInfo balootRoundInfo => (BalootRoundInfo)RoundInfo;

    public Card BalootCard;
    List<Card> AllCards;
    public BalootRoundScript()
    {
        cardsOnDeck = new Dictionary<int, Card>();
        RoundInfo = new BalootRoundInfo();
        playingIndex = 0;
        StartIndex = -1;
    }

    public override void Deal()
    {
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

        value += CardHelper.GetCardValue(balootRoundInfo.BalootRoundType, winningCard);// GetValue(winningCard);

        for (int i = 1; i < 4; i++)
        {
            Card currentCard = cardsOnDeck.ElementAt(i).Value;
            value += CardHelper.GetCardValue(balootRoundInfo.BalootRoundType, currentCard);
            if (currentCard.Shape == winningCard.Shape)
            {
                if (CardHelper.GetCardRank(balootRoundInfo.BalootRoundType,currentCard) > CardHelper.GetCardRank(balootRoundInfo.BalootRoundType, winningCard))
                {
                    index = cardsOnDeck.ElementAt(i).Key;
                    winningCard = currentCard;
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
    }

    public override void StartNewGame()
    {
        IncrementStartIndex();

        Deal();
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
        DealContinue(index);
        players[StartIndex].SetTurn(RoundInfo);
    }
}

public class BalootRoundInfo: RoundInfo
{
    public BalootGameType BalootRoundType;

    public BalootRoundInfo():base()
    {

    }
}

public enum EventTypeBaloot
{
    CardsDealtBegin,
    CardsDealtFinished,
    TrickFinished,
    DealFinished
}
