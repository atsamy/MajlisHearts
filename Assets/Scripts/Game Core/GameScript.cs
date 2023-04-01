using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class GameScript : GameScriptBase
{
    public delegate void CardsPassed();
    public event CardsPassed OnCardsPassed;

    public delegate void CardDoubled(Card card, int playerIndex);
    public event CardDoubled OnCardDoubled;

    public static GameScript Instance;

    //public new RoundScript RoundScript;
    private void Awake()
    {
        Instance = this;
        RoundScript = new RoundScriptHearts();
    }

    void Start()
    {
        ((RoundScriptHearts)RoundScript).OnEvent += Deal_OnEvent;

        Players = new Player[4];

        for (int i = 0; i < 4; i++)
        {
            if (i == 0)
            {
                Players[i] = new MainPlayer(i);
                Players[i].Avatar = AvatarManager.Instance.playerAvatar;
                Players[i].Name = GameManager.Instance.MyPlayer.Name;
            }
            else
            {
                Players[i] = new AIPlayer(i);
                Players[i].Avatar = AvatarManager.Instance.RobotAvatar;
                Players[i].Name = "Player " + i;
            }

            ((Player)Players[i]).OnPassCardsReady += GameScript_OnPassCardsReady;
            Players[i].OnCardReady += GameScript_OnCardReady;
            ((Player)Players[i]).OnDoubleCard += GameScript_OnDoubleCard;
            
        }

        ((RoundScriptHearts)RoundScript).SetPlayers(Players);

        SetEnvironment(GameManager.Instance.EquippedItem["TableTop"],
             GameManager.Instance.EquippedItem["CardBack"]);

        SetGameReady();
        StartGame();
    }

    //public override void StartGame()
    //{

    //}

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private void OnDisable()
    {

    }

    private void GameScript_OnDoubleCard(Card card, bool value, int playerIndex)
    {
        ((RoundScriptHearts)RoundScript).DoubleCard(card, value);

        SetCardDoubled(card, value, playerIndex);
    }

    public void SetCardDoubled(Card card, bool value, int playerIndex)
    {
        if (value)
        {
            OnCardDoubled?.Invoke(card, playerIndex);
        }
    }

    private void Deal_OnEvent(int eventIndex)
    {
        EventType eventType = (EventType)eventIndex;

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
                //RoundScript.SetTurn();
                break;
            case EventType.TrickFinished:
                Deal_OnTrickFinished(RoundScript.PlayingIndex);
                break;
            case EventType.DealFinished:
                Deal_OnDealFinished();
                break;
            default:
                break;
        }
    }

    public override void SetStartGame()
    {
        base.SetStartGame();
        OnStartPlaying?.Invoke(false);
    }
    protected void CheckDoubleCards()
    {
        foreach (var item in Players)
        {
            ((Player)item).CheckForDoubleCards();
        }
    }

    private void GameScript_OnCardReady(int playerIndex, Card card)
    {
        RoundScript.OnCardReady(playerIndex, card);
    }

    private void GameScript_OnPassCardsReady(int playerIndex, List<Card> cards)
    {
        ((RoundScriptHearts)RoundScript).GameScript_OnPassCardsReady(playerIndex, cards);
    }

    public void SetCardsPassed()
    {
        OnCardsPassed?.Invoke();
    }

    public void AddPlayer(int index, Player player)
    {
        Players[index] = player;
    }

    public override bool SetFinalScore()
    {
        bool isGameOver = false;

        foreach (Player item in Players)
        {
            if (!item.DidLead)
                item.IncrementScore(-15);

            item.SetTotalScore();

            if (item.TotalScore >= GameManager.Instance.GameData.TargetScore)
                isGameOver = true;

            item.Reset();
        }

        return isGameOver;
    }

    internal override PlayerBase CreateMainPlayer(int index)
    {
        MainPlayerIndex = index;
        return new MainPlayer(index);
    }

    internal override PlayerBase CreatePlayer(int index)
    {
        return new Player(index);
    }

    internal override PlayerBase CreateAIPlayer(int index)
    {
        return new AIPlayer(index);
    }
}
