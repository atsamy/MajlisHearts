using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MainPlayer : Player
{
    public delegate void PlayerTurn(bool firstHand);
    public PlayerTurn OnPlayerTurn;

    public MainPlayer(int index) : base(index)
    {

    }

    public override void SetTurn(int hand)
    {
        OnPlayerTurn.Invoke(hand == 0);
        base.SetTurn(hand);
    }

    public void OrderCards()
    {
        OwnedCards = OwnedCards.OrderBy(a => a.Shape).ToList();
    }
}
