using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class MainPlayer : Player
{
    public delegate void WaitPassCards();
    public event WaitPassCards OnWaitPassCards;

    public delegate void WaitDoubleCards(Card card);
    public event WaitDoubleCards OnWaitDoubleCards;

    public event Action WaitOthers;
    //public delegate void ForcePlay();

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

    //public override void SetTurn(DealInfo info)
    //{
    //    base.SetTurn(info);
    //}

    protected override void CheckDoubleCards(Card card)
    {
        OnWaitDoubleCards?.Invoke(card);
    }

    protected override void WaitForOthers()
    {
        WaitOthers?.Invoke();
    }

    internal void ForcePlay()
    {
        OnForcePlay?.Invoke();
    }
}
