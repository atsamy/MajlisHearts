using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalootRoundScript:RoundScriptBase
{
    public delegate void Event(EventTypeBaloot eventType);
    public event Event OnEvent;


    public RoundInfo DealInfo;

    Player[] players;

    public BalootRoundScript()
    {
        cardsOnDeck = new Dictionary<int, Card>();
        DealInfo = new RoundInfo();
    }
}

public class RoundInfo
{
    public CardShape TrickShape;
    public int roundNumber;
    public Dictionary<CardShape, int> ShapesOnGround;
    public List<CardBaloot> CardsDrawn;
    public List<CardBaloot> CardsOntable;

    public BalootRoundType BalootRoundType;

    public RoundInfo()
    {
        TrickShape = CardShape.Club;
        roundNumber = 0;

        ShapesOnGround = new Dictionary<CardShape, int>();

        for (int i = 0; i < 4; i++)
        {
            ShapesOnGround.Add((CardShape)i, 0);
        }

        CardsOntable = new List<CardBaloot>();
        CardsDrawn = new List<CardBaloot>();
    }

    internal void DrawCards()
    {
        roundNumber++;
        CardsDrawn.AddRange(CardsOntable);
        CardsOntable.Clear();
    }
}

public enum EventTypeBaloot
{
    CardsDealt,
    CardsPassed,
    TrickFinished,
    DealFinished,
    DoubleCardsFinished,
}

public enum BalootRoundType
{
    Sun = 0,
    Hokum = 1
}
