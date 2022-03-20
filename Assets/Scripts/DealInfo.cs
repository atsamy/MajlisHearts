using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealInfo
{
    public CardShape TrickShape;
    public bool heartBroken;
    public int roundNumber;
    public Dictionary<CardShape, int> ShapesOnGround;
    public List<Card> CardsDrawn;
    public List<Card> CardsOntable;

    public bool QueenOfSpade { get => CardsDrawn.Contains(Card.QueenOfSpades); }
    public bool TenOfDiamonds { get => CardsDrawn.Contains(Card.TenOfDiamonds); }

    public DealInfo()
    {
        TrickShape = CardShape.Club;
        heartBroken = false;
        roundNumber = 0;

        ShapesOnGround = new Dictionary<CardShape, int>();

        for (int i = 0; i < 4; i++)
        {
            ShapesOnGround.Add((CardShape)i, 0);
        }

        CardsOntable = new List<Card>();
        CardsDrawn = new List<Card>();
    }

    internal void DrawCards()
    {
        roundNumber++;
        CardsDrawn.AddRange(CardsOntable);
        CardsOntable.Clear();
    }
}
