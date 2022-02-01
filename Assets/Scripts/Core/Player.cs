using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    List<Card> ownedCards;
    int index;
    int dealScore;
    int totalScore;


    public delegate void PassCardsReady(int playerIndex,List<Card> cards);
    public event PassCardsReady OnPassCardsReady;

    public delegate void CardReady(int playerIndex, Card card);
    public event CardReady OnCardReady;
    public Player(int index)
    {
        ownedCards = new List<Card>();
        this.index = index;
    }

    public virtual void ChooseCard(Card card)
    {
        ownedCards.Remove(card);
        OnCardReady?.Invoke(index,card);
    }
    public virtual void ChoosePassCards(List<Card> cards)
    {
        foreach (var item in cards)
        {
            ownedCards.Remove(item);
        }

        OnPassCardsReady?.Invoke(index,cards);
    }

    public virtual void SetTurn()
    {
        
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
        ownedCards.Add(card);
    }

    public void AddCards(List<Card> cards)
    {
        ownedCards.AddRange(cards);
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
