using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Analytics;
using UnityEngine;

public class RoundScriptBase
{
    public RoundInfo RoundInfo;
    //public delegate void Event(int eventType);
    public Action<int> OnEvent;

    protected int playingIndex = -1;
    public int PlayingIndex { get => playingIndex; }
    protected Dictionary<int, Card> cardsOnDeck;

    protected PlayerBase[] players;

    public void SetPlayers(PlayerBase[] players)
    {
        this.players = players;
    }

    public virtual void SetTurn()
    {

    }

    public virtual void StartRound()
    {
        
    }

    public virtual int GetValue(Card winningCard)
    {
        return 0;
    }

    public virtual int EvaluateDeck(out int value)
    {
        value = 0;
        return 0;
    }

    public virtual void StartNewGame()
    {
        
    }

    public virtual void Deal()
    {
        
    }

    public virtual void OnCardReady(int playerIndex, Card card)
    {

    }

    public async void TrickFinished(int TrickFinished)
    {
        await System.Threading.Tasks.Task.Delay(1000);
        OnEvent?.Invoke(TrickFinished);
        await System.Threading.Tasks.Task.Delay(1000);
        players[PlayingIndex].SetTurn(RoundInfo);
    }

    public async void DealFinished(int TrickFinished, int DealFinished)
    {
        await System.Threading.Tasks.Task.Delay(1000);
        OnEvent?.Invoke(TrickFinished);
        await System.Threading.Tasks.Task.Delay(1000);
        OnEvent?.Invoke(DealFinished);
    }

}
