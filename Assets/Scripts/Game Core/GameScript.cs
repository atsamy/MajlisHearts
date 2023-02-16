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
    protected Coroutine playerTimer;

    public MainPlayer MyPlayer => (MainPlayer)Players[MainPlayerIndex];
    //public new RoundScript RoundScript;
    private void Awake()
    {
        Instance = this;
        RoundScript = new RoundScriptHeats();
    }

    void Start()
    {
        ((RoundScriptHeats)RoundScript).OnEvent += Deal_OnEvent;

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

        ((RoundScriptHeats)RoundScript).SetPlayers(Players);

        SetEnvironment(GameManager.Instance.EquippedItem["TableTop"],
             GameManager.Instance.EquippedItem["CardBack"]);

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

    //private void MainPlayerTurn(int index, RoundInfo info)
    //{
    //    //playerTimer = StartCoroutine(StartTimer());
    //}

    protected IEnumerator StartTimer()
    {
        yield return new WaitForSeconds(Seconds);
        MyPlayer.ForcePlay();
    }

    private void GameScript_OnDoubleCard(Card card, bool value, int playerIndex)
    {
        ((RoundScriptHeats)RoundScript).DoubleCard(card, value);

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
                SetStartGame(false);
                RoundScript.SetTurn();
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
        ((RoundScriptHeats)RoundScript).GameScript_OnPassCardsReady(playerIndex, cards);
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

        foreach (var item in Players)
        {
            if (!((Player)item).DidLead)
                item.IncrementScore(-15);

            item.SetTotalScore();

            if (item.TotalScore >= GameManager.Instance.GameData.TargetScore)
                isGameOver = true;

            item.Reset();
        }

        return isGameOver;
    }
}
