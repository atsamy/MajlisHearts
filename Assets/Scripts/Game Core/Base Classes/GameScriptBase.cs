using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScriptBase : MonoBehaviour
{
    public delegate void CardsReady();
    public event CardsReady OnCardsReady;

    public delegate void StartPlaying(bool isMulti);
    public event StartPlaying OnStartPlaying;

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

    public void SetEnvironment(string tableTop, string cardBack)
    {
        OnSetPlayEnvironment?.Invoke(Resources.Load<Sprite>("TableTop/Tables/" + tableTop),
            Resources.Load<Sprite>("CardBack/" + cardBack));
    }

    public void SetGameReady()
    {
        OnGameReady?.Invoke();
    }

    public void SetDealFinished(bool hostPlayer)
    {
        bool isFinished = SetFinalScore();
        OnDealFinished?.Invoke(hostPlayer, isFinished);
    }

    public void SetTrickFinished(int winningHand)
    {
        GameSFXManager.Instance.PlayClipRandom("CardDraw");
        OnTrickFinished?.Invoke(winningHand);
    }

    public void SetCardsReady()
    {
        OnCardsReady?.Invoke();
    }

    public void Deal_OnTrickFinished(int winningHand)
    {
        RoundScript.SetTurn();
        OnTrickFinished?.Invoke(winningHand);
    }

    public void StartGame()
    {
        RoundScript.StartNewRound();
    }

    public void SetStartGame(bool isMulti)
    {
        OnStartPlaying?.Invoke(isMulti);
    }

    public virtual void StartNextDeal()
    {
        RoundScript.StartNewRound();
    }

    public virtual bool SetFinalScore()
    {
        return false;
    }

    public void Deal_OnDealFinished()
    {
        SetDealFinished(true);
    }
}