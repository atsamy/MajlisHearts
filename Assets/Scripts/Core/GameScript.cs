using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScript : MonoBehaviour
{
    public delegate void CardsReady();
    public event CardsReady OnCardsReady;

    public delegate void StartPlaying();
    public event StartPlaying OnStartPlaying;

    public DealScript Deal;
    public static GameScript Instance;
    public Player[] Players;
    
    [HideInInspector]
    public int MainPlayerIndex = 0;

    private void Awake()
    {
        Instance = this;
        Deal = new DealScript();
    }

    void Start()
    {
        Deal.OnDealFinished += Deal_OnDealFinished;
        Deal.OnCardsDealt += Deal_OnCardsDealt;

        Deal.OnCardsPassed += SetStartPlaying;

        Deal.OnTrickFinished += Deal_OnTrickFinished;
        //Deal.OnNextTurn += Deal_OnNextTurn;

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

    //private void Deal_OnNextTurn()
    //{
    //    Deal.SetTurn();
    //}

    private void Deal_OnTrickFinished(int winningHand)
    {
        Deal.SetTurn();
    }

    private void Deal_OnCardsDealt(bool waitPass)
    {
        SetCardsReady();
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
        OnStartPlaying?.Invoke();
        Deal.SetTurn();
    }

    public void SetStartPlaying()
    {
        OnStartPlaying?.Invoke();
    }

    public void SetCardsReady()
    {
        OnCardsReady?.Invoke();
    }

    public void AddPlayer(int index, Player player)
    {
        Players[index] = player;
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
