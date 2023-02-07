using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HeartsUIManager : UIManagerBase
{
    public static HeartsUIManager Instance;

    GameScript game;
    int doubleCardCount;
    MainPlayer mainPlayer;

    CardsUIManager cardsUIManager;
    UIManager uiManager;

    [SerializeField]
    PassCardsPanel passCardsPanel;
    [SerializeField]
    DealResult DealFinishedPanel;
    [SerializeField]
    DoublePanelScript doublePanel;

    bool init;
    void Awake()
    {
        Instance = this;
        doubleCardCount = 0;
        game = GameScript.Instance;

        uiManager = GetComponent<UIManager>();
        uiManager.game= game;

        game.OnCardsReady += Game_OnCardsDealt;
        game.OnTrickFinished += Game_OnTrickFinished;
        game.OnStartPlaying += uiManager.Game_OnStartPlaying;
        game.OnCardsPassed += CardsPassed;
        game.OnDealFinished += Game_OnDealFinished;
        game.OnCardDoubled += Game_OnCardDoubled;
        game.OnSetPlayEnvironment += uiManager.Game_OnSetPlayEnvironment;

        cardsUIManager = GetComponentInChildren<CardsUIManager>();
    }

    private void Game_OnCardsDealt()
    {
        if (!init)
        {
            mainPlayer = (MainPlayer)game.Players[game.MainPlayerIndex];
            mainPlayer.OnWaitPassCards += MainPlayer_OnWaitPassCards;
            mainPlayer.OnWaitDoubleCards += MainPlayer_OnWaitDoubleCards;
            mainPlayer.WaitOthers += uiManager.MainPlayer_WaitOthers;
            cardsUIManager.SetMainPlayer(mainPlayer);

            uiManager.SetPlayers(game.Players, mainPlayer);
            init = true;
        }
        SetScore();
        cardsUIManager.ShowPlayerCards(mainPlayer, true,13);
    }

    internal void SetDoubleCard(Card card, bool value)
    {
        if (doubleCardCount == 1)
            uiManager.WaitingPanel.Show();

        mainPlayer.SetDoubleCard(card, value);
    }

    public void ShowScores()
    {
        DealFinishedPanel.ShowInGame(game.Players);
    }

    public void HideScores()
    {
        DealFinishedPanel.gameObject.SetActive(false);
    }

    private void Game_OnTrickFinished(int winningHand)
    {
        SetScore();

        int index = uiManager.CorrectIndex(winningHand);
        cardsUIManager.RemoveCards(index);
        GameSFXManager.Instance.PlayClipRandom("CardDraw");
    }

    private void MainPlayer_OnWaitPassCards()
    {
        DealFinishedPanel.gameObject.SetActive(false);

        passCardsPanel.Show((cards) =>
        {
            PassCards(cards);
        });
    }

    public void SetScore()
    {
        for (int i = 0; i < game.Players.Length; i++)
        {
            int correctIndex = i + game.MainPlayerIndex;
            correctIndex %= 4;

            cardsUIManager.SetScore(i, game.Players[correctIndex].Score);
        }
    }

    private void Game_OnCardDoubled(Card card, int playerIndex)
    {
        int index = uiManager.CorrectIndex(playerIndex);
        cardsUIManager.AddDoubledCard(card, index);
    }

    private void CardsPassed()
    {
        uiManager.WaitingPanel.Hide();
        StartCoroutine(cardsUIManager.UpdateCards(mainPlayer));
    }

    private void Game_OnDealFinished(bool hostPlayer, bool isGameOver)
    {
        doubleCardCount = 0;

        uiManager.scoresHolder.SetActive(false);
        uiManager.EmojiButton.SetActive(false);
        uiManager.GameOver = isGameOver;

        if (isGameOver)
        {
            uiManager.GamePanel.SetActive(false);
            DealFinishedPanel.ShowRound(game.Players, false, true, (rank) =>
            {
                if (GameManager.Instance.GameType != GameType.Single)
                {
                    uiManager.LevelPanel.Open(rank, game.MyPlayer.TotalScore, () =>
                    {
                        uiManager.GoToMainMenu();
                    });
                }
                else
                {
                    uiManager.GoToMainMenu();
                }
            });


            return;
        }
        if (hostPlayer)
        {
            DealFinishedPanel.ShowRound(game.Players, false, false, (rank) =>
            {
                game.StartNextDeal();
            });
        }
        else
        {
            DealFinishedPanel.ShowRound(game.Players, false, false, null);
        }

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
        uiManager.WaitingPanel.Hide();
        doublePanel.ShowPanel(card);
    }

    public void OnDisable()
    {
        game.OnCardsReady -= Game_OnCardsDealt;
        game.OnTrickFinished -= Game_OnTrickFinished;
        game.OnStartPlaying -= uiManager.Game_OnStartPlaying;
        game.OnCardsPassed -= CardsPassed;
        game.OnDealFinished -= Game_OnDealFinished;
        game.OnCardDoubled -= Game_OnCardDoubled;
        game.OnSetPlayEnvironment -= uiManager.Game_OnSetPlayEnvironment;

        foreach (var player in game.Players)
        {
            player.OnCardReady -= uiManager.Player_OnCardReady;
            player.OnPlayerTurn -= uiManager.Player_OnPlayerTurn;
        }
    }

    internal void PassCards(List<Card> selectedPassCards)
    {
        uiManager.WaitingPanel.Show();
        mainPlayer.PassCards(selectedPassCards);
        //scoresHolder.SetActive(true);
    }

    internal void HostLeft()
    {
        uiManager.HostLeft();
        //throw new NotImplementedException();
    }
}
