using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundScriptBase 
{
    protected int playingIndex = -1;
    public int PlayingIndex { get => playingIndex; }
    protected Dictionary<int, Card> cardsOnDeck;

    public virtual void SetTurn()
    {

    }

    public virtual void StartRound()
    {
        
    }

    public virtual void StartNewGame()
    {
        
    }

    public virtual void OnCardReady(int playerIndex, Card card)
    {

    }
}
