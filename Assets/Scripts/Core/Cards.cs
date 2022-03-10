using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Card
{
    public CardShape Shape;
    public CardRank Rank;

    public bool IsQueenOfSpades { get => Shape == CardShape.Spade && Rank == CardRank.Queen; }
    public bool IsTenOfDiamonds { get => Shape == CardShape.Diamond && Rank == CardRank.Ten; }

    public static Card QueenOfSpades { get => new Card(CardShape.Spade, CardRank.Queen); }
    public static Card TenOfDiamonds { get => new Card(CardShape.Diamond, CardRank.Ten); }

    public Card(CardShape cardShape,CardRank rank)
    {
        Shape = cardShape;
        Rank = rank;
    }

    public override string ToString()
    {
        return Rank + " of " + Shape;
    }

    public override bool Equals(object obj)
    {
        if (!(obj is Card))
            return false;

        return ((Card)obj).Rank == Rank && ((Card)obj).Shape == Shape;
    }

    public override int GetHashCode()
    {
        int hashCode = 114147325;
        hashCode = hashCode * -1521134295 + Shape.GetHashCode();
        hashCode = hashCode * -1521134295 + Rank.GetHashCode();
        return hashCode;
    }
}



public enum CardShape
{
    Heart = 0,
    Spade = 1,
    Diamond = 2,
    Club = 3
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
