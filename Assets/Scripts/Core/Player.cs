using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public List<Card> OwnedCards { get; protected set; }

    int index;
    int dealScore;
    int totalScore;

    Dictionary<CardShape,int> shapeCount;


    public delegate void PassCardsReady(int playerIndex,List<Card> cards);
    public event PassCardsReady OnPassCardsReady;

    public delegate void CardReady(int playerIndex, Card card);
    public event CardReady OnCardReady;
    public Player(int index)
    {
        shapeCount = new Dictionary<CardShape, int>();
        OwnedCards = new List<Card>();
        this.index = index;
    }

    public virtual void ChooseCard(Card card)
    {
        OwnedCards.Remove(card);
        shapeCount[card.Shape]--;
        OnCardReady?.Invoke(index,card);
    }

    public virtual void ChoosePassCards(List<Card> cards)
    {
        foreach (var item in cards)
        {
            OwnedCards.Remove(item);
            shapeCount[item.Shape]--;
        }

        OnPassCardsReady?.Invoke(index,cards);
    }

    public virtual void SetTurn(int hand)
    {
        
    }

    public bool HasShape(CardShape shape)
    {
        return shapeCount[shape] > 0;
    }

    //public bool CheckStart()
    //{
    //    foreach (var item in ownedCards)
    //    {
    //        if (item.Shape == CardShape.Club && item.Rank == CardRank.Two)
    //        {
    //            return true;
    //        }
    //    }

    //    return false;
    //}



    public void AddCard(Card card)
    {
        OwnedCards.Add(card);
        shapeCount[card.Shape]++;
    }

    public void AddCards(List<Card> cards)
    {
        foreach (var item in cards)
        {
            AddCard(item);
        }
    }

    public void IncrementScore(int score)
    {
        dealScore += score;
    }

    public void Reset()
    {
        dealScore = 0;
    }

}

public enum PlayerState
{
    Waiting,
    ChoosePass,
    YourTurn
}
