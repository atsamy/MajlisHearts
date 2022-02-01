using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public List<Card> OwnedCards;


    public Player()
    {
        OwnedCards = new List<Card>();
    }

    public virtual Card ChooseCard(Card card)
    {

        OwnedCards.Remove(card);
        return card;
    }
    public virtual List<Card> ChoosePassCards(List<Card> cards)
    {
        foreach (var item in cards)
        {
            OwnedCards.Remove(item);
        }
        return cards;
    }

    public void AddCard(Card card)
    {
        OwnedCards.Add(card);
    }

}

public enum PlayerState
{
    Waiting,
    ChoosePass,
    YourTurn
}
