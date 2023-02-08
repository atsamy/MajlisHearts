using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalootRoundScript:RoundScriptBase
{
    public delegate void Event(EventTypeBaloot eventType);
    public event Event OnEvent;

    public int StartIndex;
    public BalootRoundInfo RoundInfo;

    public Card BalootCard;

    List<Card> AllCards;
    public BalootRoundScript()
    {
        cardsOnDeck = new Dictionary<int, Card>();
        RoundInfo = new BalootRoundInfo();

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

        OnEvent?.Invoke(EventTypeBaloot.CardsDealtFinished);
    }

    public override void StartNewGame()
    {
        IncrementStartIndex();

        Deal();
        RoundInfo = new BalootRoundInfo();

        OnEvent?.Invoke(EventTypeBaloot.CardsDealtBegin);
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
        RoundInfo.BalootRoundType = type;
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
    DealFinished,
    DoubleCardsFinished,
}
