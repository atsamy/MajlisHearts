using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class MainPlayer : Player
{
    public delegate void PlayerTurn(DealInfo info);
    public PlayerTurn OnPlayerTurn;


    public delegate void WaitPassCards();
    public event WaitPassCards OnWaitPassCards;

    public delegate void WaitDoubleCards(Card card);
    public event WaitDoubleCards OnWaitDoubleCards;

    //public delegate void ForcePlay();
    public event Action OnForcePlay;

    public List<Card> PassedCards { get; private set; } 

    public MainPlayer(int index) : base(index)
    {
        isPlayer = false;
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

    public override void SetTurn(DealInfo info, int hand)
    {
        OnPlayerTurn?.Invoke(info);
        base.SetTurn(info, hand);
    }

    public void OrderCards()
    {
        OwnedCards = OwnedCards.OrderBy(a => a.Shape).ToList();
    }

    protected override void CheckDoubleCards(Card card)
    {
        OnWaitDoubleCards?.Invoke(card);
    }

    internal void ForcePlay()
    {
        OnForcePlay?.Invoke();
    }

    //public override void Reset()
    //{
    //    base.Reset();
    //    PassedCards.Clear();
    //}
}
