using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GameScriptBase : MonoBehaviour
{
    public delegate Task CardsReady();
    public event CardsReady OnCardsReady;

    public delegate void StartPlaying(bool isMulti);
    public StartPlaying OnStartPlaying;

    public delegate void GameReady();
    public event GameReady OnGameReady;

    public delegate void DealFinished(bool hostPlayer, bool gameFinished);
    public event DealFinished OnDealFinished;

    public delegate void TrickFinished(int winningHand);
    public event TrickFinished OnTrickFinished;

    public delegate void SetPlayEnvironment(Sprite tableTop, Sprite cardBack);
    public event SetPlayEnvironment OnSetPlayEnvironment;

    protected const int Seconds = 10;
    //protected const int FinishScore = 150;

    public RoundScriptBase RoundScript;

    public PlayerBase[] Players;
    [HideInInspector]
    public int MainPlayerIndex = 0;
    protected Coroutine playerTimer;

    public PlayerBase MyPlayer => Players[MainPlayerIndex];

    public void SetEnvironment(string tableTop, string cardBack)
    {
        OnSetPlayEnvironment?.Invoke(Resources.Load<Sprite>("TableTop/Tables/" + tableTop),
            Resources.Load<Sprite>("CardBack/" + cardBack));
    }

    public void SetGameReady()
    {
        OnGameReady?.Invoke();
    }

    private void SetDealFinished(bool hostPlayer)
    {
        bool isFinished = SetFinalScore();
        OnDealFinished?.Invoke(hostPlayer, isFinished);
    }

    public void SetTrickFinished(int winningHand)
    {
        GameSFXManager.Instance.PlayClipRandom("CardDraw");
        OnTrickFinished?.Invoke(winningHand);
    }

    public async Task SetCardsReady()
    {
        await OnCardsReady?.Invoke();
    }

    public void Deal_OnTrickFinished(int winningHand)
    {
        OnTrickFinished?.Invoke(winningHand);
        RoundScript.SetTurn();
    }

    public void StartGame()
    {
        RoundScript.StartNewRound();
    }

    public virtual void SetPlaying(bool isMulti)
    {
        //RoundScript.StartFirstTurn();
    }

    public virtual void StartNextDeal()
    {
        RoundScript.StartNewRound();
    }

    public virtual bool SetFinalScore()
    {
        return false;
    }

    //public void Deal_OnDealFinished()
    //{
    //    SetDealFinished(true);
    //}

    public async void RoundFinished(int winningHand,bool isHost)
    {
        OnTrickFinished?.Invoke(winningHand);
        await System.Threading.Tasks.Task.Delay(1000);
        SetDealFinished(isHost);
    }

    //private void MainPlayerTurn(int index, RoundInfo info)
    //{
    //    //playerTimer = StartCoroutine(StartTimer());
    //}

    public void StartTimer()
    {
        print("call start timer");
        playerTimer = StartCoroutine(TimerRoutine());
    }

    protected IEnumerator TimerRoutine()
    {
        yield return new WaitForSeconds(Seconds);
        MyPlayer.ForcePlay();
    }

    internal void StopPlayerTimer()
    {
        StopCoroutine(playerTimer);
    }

    internal virtual PlayerBase CreateMainPlayer(int index)
    {
        return null;
    }

    internal virtual PlayerBase CreateAIPlayer(int index)
    {
        return null;
    }

    internal virtual PlayerBase CreatePlayer(int index)
    {
        return null;
    }
}
