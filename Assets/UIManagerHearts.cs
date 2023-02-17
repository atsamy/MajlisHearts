using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIManagerHearts : UIManager
{
    GameScript heartsGame => (GameScript)Game;
    int doubleCardCount;
    MainPlayer mainPlayer;

    HeartsCardsUIManager heartsCardsUI => (HeartsCardsUIManager)CardsUI;

    [SerializeField]
    PassCardsPanel passCardsPanel;
    [SerializeField]
    DoublePanelScript doublePanel;

    bool init;

    private void OnEnable()
    {
        Instance = this;
        doubleCardCount = 0;
        Game = GameScript.Instance;
        CardsUI = GetComponentInChildren<HeartsCardsUIManager>();

        Init();

        heartsGame.OnCardsPassed += CardsPassed;
        heartsGame.OnCardDoubled += Game_OnCardDoubled;

        doublePanel.OnDoubleCardSet += SetDoubleCard;
    }


    public override void Game_OnCardsReady()
    {
        if (!init)
        {
            mainPlayer = (MainPlayer)Game.Players[Game.MainPlayerIndex];
            mainPlayer.OnWaitPassCards += MainPlayer_OnWaitPassCards;
            mainPlayer.OnWaitDoubleCards += MainPlayer_OnWaitDoubleCards;
            mainPlayer.WaitOthers += MainPlayer_WaitOthers;
            CardsUI.SetMainPlayer(mainPlayer);

            SetPlayers(Game.Players, mainPlayer);
            init = true;
        }
        SetScore();
        CardsUI.ShowPlayerCards(mainPlayer, true,13);
    }

    protected override void Game_OnDealFinished(bool hostPlayer, bool isGameOver)
    {
        doubleCardCount = 0;
        base.Game_OnDealFinished(hostPlayer, isGameOver);
    }

    internal void SetDoubleCard(Card card, bool value)
    {
        if (doubleCardCount == 1)
            UIElementsHolder.WaitingPanel.Show();

        mainPlayer.SetDoubleCard(card, value);
    }

    private void MainPlayer_OnWaitPassCards()
    {
        DealFinishedPanel.gameObject.SetActive(false);

        passCardsPanel.Show((cards) =>
        {
            PassCards(cards);
        });
    }

    public override void SetScore()
    {
        for (int i = 0; i < heartsGame.Players.Length; i++)
        {
            int correctIndex = i + heartsGame.MainPlayerIndex;
            correctIndex %= 4;

            CardsUI.SetScore(i, heartsGame.Players[correctIndex].Score);
        }
    }

    private void Game_OnCardDoubled(Card card, int playerIndex)
    {
        int index = CorrectIndex(playerIndex);
        heartsCardsUI.AddDoubledCard(card, index);
    }

    private void CardsPassed()
    {
        UIElementsHolder.WaitingPanel.Hide();
        StartCoroutine(heartsCardsUI.UpdateCards(mainPlayer));
    }



    internal bool AddCard(CardUI card)
    {
        GameSFXManager.Instance.PlayClipRandom("Card");
        return passCardsPanel.AddCard(card);
    }

    internal void RemoveCard(CardUI cardUI)
    {
        GameSFXManager.Instance.PlayClipRandom("Card");
        passCardsPanel.RemoveCard(cardUI);
    }

    private void MainPlayer_OnWaitDoubleCards(Card card)
    {
        doubleCardCount++;
        UIElementsHolder.WaitingPanel.Hide();
        doublePanel.ShowPanel(card);
    }

    private void OnDisable()
    {
        heartsGame.OnCardsPassed -= CardsPassed;
        heartsGame.OnCardDoubled -= Game_OnCardDoubled;
        doublePanel.OnDoubleCardSet -= SetDoubleCard;

        base.Disable();
    }

    internal void PassCards(List<Card> selectedPassCards)
    {
        UIElementsHolder.WaitingPanel.Show();
        mainPlayer.PassCards(selectedPassCards);
        //scoresHolder.SetActive(true);
    }
}
