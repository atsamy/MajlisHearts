using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class GameScript : MonoBehaviour
{
    public delegate void CardsReady();
    public event CardsReady OnCardsReady;

    public delegate void CardsPassed();
    public event CardsPassed OnCardsPassed;

    public delegate void StartPlaying();
    public event StartPlaying OnStartPlaying;

    public delegate void TrickFinished(int winningHand);
    public event TrickFinished OnTrickFinished;

    public delegate void DealFinished(bool hostPlayer,bool gameFinished);
    public event DealFinished OnDealFinished;

    public delegate void CardDoubled(Card card, int playerIndex);
    public event CardDoubled OnCardDoubled;

    protected DealScript Deal;
    public static GameScript Instance;
    public Player[] Players;

    protected MainPlayer myPlayer;

    protected Coroutine playerTimer;

    [HideInInspector]
    public int MainPlayerIndex = 0;

    private void Awake()
    {
        Instance = this;
        Deal = new DealScript();

    }

    void Start()
    {
        Deal.OnEvent += Deal_OnEvent;

        Players = new Player[4];

        for (int i = 0; i < 4; i++)
        {
            if (i == 0)
                Players[i] = new MainPlayer(i);
            else
                Players[i] = new AIPlayer(i);

            Players[i].OnPassCardsReady += GameScript_OnPassCardsReady;
            Players[i].OnCardReady += GameScript_OnCardReady;
            Players[i].OnDoubleCard += GameScript_OnDoubleCard;

            Players[i].Name = "Player " + (i + 1);
        }
        myPlayer = (MainPlayer)Players[0];
        myPlayer.OnPlayerTurn += MainPlayerTurn;

        Deal.SetPlayers(Players);

        StartGame();
    }

    private void MainPlayerTurn(DealInfo info)
    {
        playerTimer = StartCoroutine(StartTimer());
    }

    protected IEnumerator StartTimer()
    {
        yield return new WaitForSeconds(1);

        myPlayer.ForcePlay();
    }

    private void GameScript_OnDoubleCard(Card card, bool value,int playerIndex)
    {
        Deal.DoubleCard(card, value);

        SetCardDoubled(card, value, playerIndex);
    }

    public void SetCardDoubled(Card card, bool value, int playerIndex)
    {
        if (value)
        {
            OnCardDoubled?.Invoke(card, playerIndex);
        }
    }

    public virtual void StartNextDeal()
    {
        Deal.StartNewGame();
    }

    private void Deal_OnEvent(EventType eventType)
    {
        switch (eventType)
        {
            case EventType.CardsDealt:
                SetCardsReady();
                break;
            case EventType.CardsPassed:
                SetCardsPassed();
                CheckDoubleCards();
                break;
            case EventType.DoubleCardsFinished:
                SetStartGame();
                Deal.SetTurn();
                break;
            case EventType.TrickFinished:
                Deal_OnTrickFinished(Deal.PlayingIndex);
                break;
            case EventType.DealFinished:
                Deal_OnDealFinished();
                break;
            default:
                break;
        }
    }

    protected void CheckDoubleCards()
    {
        foreach (var item in Players)
        {
            item.CheckForDoubleCards();
        }
    }

    public void Deal_OnTrickFinished(int winningHand)
    {
        Deal.SetTurn();
        OnTrickFinished?.Invoke(winningHand);
    }

    private void GameScript_OnCardReady(int playerIndex, Card card)
    {
        Deal.GameScript_OnCardReady(playerIndex, card);

        if (playerIndex == 0)
        {
            StopCoroutine(playerTimer);
        }
    }

    private void GameScript_OnPassCardsReady(int playerIndex, List<Card> cards)
    {
        Deal.GameScript_OnPassCardsReady(playerIndex, cards);
    }

    private void Deal_OnDealFinished()
    {
        SetDealFinished(true);
    }

    public void SetDealFinished(bool hostPlayer)
    {
        bool isFinished = SetFinalScore();
        OnDealFinished?.Invoke(hostPlayer, isFinished);
    }

    public void SetCardsPassed()
    {
        OnCardsPassed?.Invoke();
    }

    public void SetStartGame()
    {
        OnStartPlaying?.Invoke();
    }

    public void SetTrickFinished(int winningHand)
    {
        OnTrickFinished?.Invoke(winningHand);
    }

    public void SetCardsReady()
    {
        OnCardsReady?.Invoke();
    }

    public void AddPlayer(int index, Player player)
    {
        Players[index] = player;
    }

    public bool SetFinalScore()
    {
        bool isGameOver = false;
        //bool isMoonShot = Players.Any(a => a.Score == 26);

        foreach (var item in Players)
        {
            if (!item.DidLead)
                item.IncrementScore(-15);

            //if (isMoonShot)
            //{
            //    if (item.Score == 26)
            //    {
            //        item.Score = 0;
            //    }
            //    else
            //    {
            //        item.Score = 26;
            //    }
            //}

            item.SetTotalScore();

            if (item.TotalScore >= 100)
                isGameOver = true;
        }

        return isGameOver;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            List<Card> cards = new List<Card>();

            for (int i = 0; i < 4; i++)
            {
                foreach (var item in Players[i].OwnedCards)
                {
                    if (!cards.Contains(item))
                        cards.Add(item);
                    else
                    {
                        Debug.Log("dublicate card: " + item.ToString());
                    }
                }
            }
        }
    }

    public void StartGame()
    {
        Deal.StartDeal();
    }

    GameState GameState;
}
