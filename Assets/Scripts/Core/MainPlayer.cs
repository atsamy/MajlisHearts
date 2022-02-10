using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MainPlayer : Player
{
    public delegate void PlayerTurn(bool firstHand);
    public PlayerTurn OnPlayerTurn;


    public delegate void WaitPassCards();
    public event WaitPassCards OnWaitPassCards;

    public List<Card> PassedCards { get; private set; } 

    public MainPlayer(int index) : base(index)
    {

    }

    public override void SelectPassCards()
    {
        OnWaitPassCards?.Invoke();
    }

    public override void AddPassCards(List<Card> cards)
    {
        PassedCards = cards;
        base.AddPassCards(cards);
    }

    public override void SetTurn(TrickInfo info, int hand)
    {
        OnPlayerTurn?.Invoke(hand == 0);
        base.SetTurn(info, hand);
    }

    public void OrderCards()
    {
        OwnedCards = OwnedCards.OrderBy(a => a.Shape).ToList();
    }
}
