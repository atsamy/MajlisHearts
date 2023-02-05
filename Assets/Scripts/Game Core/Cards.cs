using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card
{
    public CardShape Shape;
    public CardRank Rank;

    public Card(CardShape cardShape, CardRank rank)
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

    public bool IsQueenOfSpades { get => Shape == CardShape.Spade && Rank == CardRank.Queen; }
    public bool IsTenOfDiamonds { get => Shape == CardShape.Diamond && Rank == CardRank.Ten; }
    public static Card QueenOfSpades { get => new Card(CardShape.Spade, CardRank.Queen); }
    public static Card TenOfDiamonds { get => new Card(CardShape.Diamond, CardRank.Ten); }
}

//[System.Serializable]
//public class Card:CardBase
//{
//    public Card(CardShape cardShape, CardRank rank) : base(cardShape, rank) { }
//    public bool IsQueenOfSpades { get => Shape == CardShape.Spade && Rank == CardRank.Queen; }
//    public bool IsTenOfDiamonds { get => Shape == CardShape.Diamond && Rank == CardRank.Ten; }

//    public static Card QueenOfSpades { get => new Card(CardShape.Spade, CardRank.Queen); }
//    public static Card TenOfDiamonds { get => new Card(CardShape.Diamond, CardRank.Ten); }
//}

public class CardBaloot : Card
{
    public CardBaloot(CardShape cardShape, CardRank rank) : base(cardShape, rank) { }

    public Dictionary<CardRank, int> SunValue = new Dictionary<CardRank, int>()
    {
        {CardRank.Seven, 0},
        {CardRank.Eight, 1},
        {CardRank.Nine, 2},
        {CardRank.Knight, 3},
        {CardRank.Queen, 4},
        {CardRank.King, 5},
        {CardRank.Ten, 6},
        {CardRank.Ace, 7}
    };

    public Dictionary<CardRank, int> HokumValue = new Dictionary<CardRank, int>()
    {
        {CardRank.Seven, 0},
        {CardRank.Eight, 1},
        {CardRank.Queen, 2},
        {CardRank.King, 3},
        {CardRank.Ten, 4},
        {CardRank.Ace, 5},
        {CardRank.Nine, 6},
        {CardRank.Knight, 7}
    };

    public int GetCardValue(BalootRoundType balootDealType)
    {
        if (balootDealType == BalootRoundType.Sun)
        {
            return SunValue[Rank];
        }
        else
        {
            return HokumValue[Rank];
        }
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
