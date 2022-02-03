using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MainPlayer : Player
{
    public delegate void PlayerTurn();
    public PlayerTurn OnPlayerTurn;

    public MainPlayer(int index) : base(index)
    {

    }

    public override void SetTurn()
    {
        OnPlayerTurn.Invoke();
        base.SetTurn();
    }

    public void OrderCards()
    {
        OwnedCards = OwnedCards.OrderBy(a => a.Shape).ToList();
    }
}
