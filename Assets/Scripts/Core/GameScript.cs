using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameScript : MonoBehaviour
{
    public delegate void CardsReady();
    public event CardsReady OnCardsReady;

    public delegate void StartPlaying();
    public event StartPlaying OnStartPlaying;

    public delegate void TrickFinished(int winningHand);
    public event TrickFinished OnTrickFinished;

    public delegate void DealFinished(bool hostPlayer);
    public event DealFinished OnDealFinished;

    protected DealScript Deal;
    public static GameScript Instance;
    public Player[] Players;
    
    [HideInInspector]
    public int MainPlayerIndex = 0;

    private void Awake()
    {
        Instance = this;
        Deal = new DealScript();
        Deal.OnEvent += Deal_OnEvent;
    }

    void Start()
    {
        Players = new Player[4];

        for (int i = 0; i < 4; i++)
        {
            if (i == 0)
                Players[i] = new MainPlayer(i);
            else
                Players[i] = new AIPlayer(i);

            Players[i].OnPassCardsReady += GameScript_OnPassCardsReady;
            Players[i].OnCardReady += GameScript_OnCardReady;

            Players[i].Name = "Player " + (i + 1);
        }

        Deal.SetPlayers(Players);

        StartGame();
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
                SetStartPlaying();
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

    public void Deal_OnTrickFinished(int winningHand)
    {
        Deal.SetTurn();
        OnTrickFinished?.Invoke(winningHand);
    }

    private void GameScript_OnCardReady(int playerIndex, Card card)
    {
        Deal.GameScript_OnCardReady(playerIndex, card);
    }

    private void GameScript_OnPassCardsReady(int playerIndex, List<Card> cards)
    {
        Deal.GameScript_OnPassCardsReady(playerIndex,cards);
    }

    private void Deal_OnDealFinished()
    {
        SetDealFinished(true);
    }

    public void SetDealFinished(bool hostPlayer)
    {
        SetFinalScore();
        OnDealFinished?.Invoke(hostPlayer);
    }

    public void SetStartPlaying()
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

    public void SetFinalScore()
    {
        bool isMoonShot = Players.Any(a => a.Score == 26);

        foreach (var item in Players)
        {
            if (isMoonShot)
            {
                if (item.Score == 26)
                {
                    item.Score = 0;
                }
                else
                {
                    item.Score = 13;
                }
            }

            item.SetTotalScore();
        }
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
