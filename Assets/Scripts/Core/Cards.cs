using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card
{
    public CardShape Shape;
    public CardRank Rank;

    public Card(CardShape cardShape,CardRank rank)
    {
        Shape = cardShape;
        Rank = rank;
    }
}



public enum CardShape
{
    Heart,
    Spade,
    Club,
    Diamond
}

public enum CardRank
{
    Two = 0,
    Three = 1,
    Four = 2,
    Five = 3,
    Six = 4,
    Seven = 5,
    Eight = 6,
    Nine = 7,
    Ten = 8,
    Knight = 9,
    Queen = 10,
    King = 11,
    Ace = 12
}
