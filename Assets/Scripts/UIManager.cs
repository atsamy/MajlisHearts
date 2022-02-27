using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    MainPlayer mainPlayer;
    public static UIManager Instance;

    //Player[] players;
    [SerializeField]
    GameObject passCardsPanel;
    [SerializeField]
    DealResult DealFinishedPanel;

    public GameObject Scores;

    public Text PassText;

    CardsUIManager cardsUIManager;
    GameScript game;

    public DebugCards[] debugCards;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        game = GameScript.Instance;

        game.Deal.OnCardsDealt += Deal_OnCardsDealt;
        game.Deal.OnTrickFinished += Deal_OnTrickFinished;
        game.Deal.OnCardsPassed += Deal_OnCardsPassed;
        game.Deal.OnDealFinished += Deal_OnDealFinished;

        cardsUIManager = GetComponentInChildren<CardsUIManager>();
    }

    private void Deal_OnDealFinished()
    {
        Player[] players = game.Deal.Players;
        players = players.OrderBy(a => a.TotalScore).ToArray();

        DealFinishedPanel.Show(players,() =>
        {
            game.Deal.StartNewGame();
        });

        Scores.SetActive(false);
    }

    public void AddDebugWeight(int playerIndex,Card card,int Weight)
    {
        debugCards[playerIndex].ShowWeight(card, Weight);
    }

    private void Deal_OnCardsPassed()
    {
        cardsUIManager.UpdateCards(mainPlayer);
    }

    private void Deal_OnTrickFinished(int winningHand)
    {
        cardsUIManager.SetScores(game.Deal.Players);
        cardsUIManager.RemoveCards(winningHand);
    }

    public void SetPlayers(Player[] players)
    {
        //this.players = players;

        foreach (var player in players)
        {
            player.OnCardReady += Player_OnCardReady;
        }
    }

    private void Player_OnCardReady(int playerIndex, Card card)
    {
        if (playerIndex != 0)
            cardsUIManager.CardsPlayed(playerIndex, card);
        else
            cardsUIManager.MainPlayerCard(card);
    }

    bool once;

    private void Deal_OnCardsDealt(bool waitPass)
    {
        mainPlayer = (MainPlayer)game.Deal.Players[0];
        mainPlayer.OnPlayerTurn += PlayerTurn;
        mainPlayer.OnWaitPassCards += MainPlayer_OnWaitPassCards;


        if (!once)
        {
            SetPlayers(game.Deal.Players);
            once = true;
        }
        cardsUIManager.ResetScores();
        cardsUIManager.ShowPlayerCards(mainPlayer, waitPass);

        for (int i = 1; i < game.Deal.Players.Length; i++)
        {
            debugCards[i - 1].AddCards(game.Deal.Players[i].OwnedCards);
        }
    }

    private void MainPlayer_OnWaitPassCards()
    {
        passCardsPanel.SetActive(true);
        PassText.text = game.Deal.CurrentState.ToString();
    }

    public void PlayerTurn(bool firstHand)
    {
        cardsUIManager.SetPlayableCards(game.Deal.DealInfo, mainPlayer, firstHand);
    }

    public void OnDisable()
    {

    }

    internal void PassCards(List<Card> selectedPassCards)
    {
        for (int i = 1; i < 4; i++)
        {
            ((AIPlayer)game.Deal.Players[i]).PassCards();
        }

        mainPlayer.PassCards(selectedPassCards);
        passCardsPanel.SetActive(false);
        cardsUIManager.RemovePassedCards();

        for (int i = 1; i < 4; i++)
        {
            debugCards[i - 1].UpdateCards(game.Deal.Players[i].OwnedCards);
        }

        Scores.SetActive(true);
    }
}
