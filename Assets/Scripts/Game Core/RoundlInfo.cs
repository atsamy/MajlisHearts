using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundInfo
{
    public CardShape TrickShape;
    public int TrickNumber;
    public Dictionary<CardShape, int> ShapesOnGround;
    public List<Card> CardsDrawn;
    public List<Card> CardsOntable;

    public bool QueenOfSpade { get => CardsDrawn.Contains(CardHelper.QueenOfSpades); }
    public bool TenOfDiamonds { get => CardsDrawn.Contains(CardHelper.TenOfDiamonds); }

    public RoundInfo()
    {
        TrickShape = CardShape.Club;
        //heartBroken = false;
        TrickNumber = 0;

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
        TrickNumber++;
        CardsDrawn.AddRange(CardsOntable);
        CardsOntable.Clear();
    }

    //public void IncrementTrickNumber()
    //{

    //}
}
