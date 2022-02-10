using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    MainPlayer mainPlayer;
    public static UIManager Instance;

    //Player[] players;
    [SerializeField]
    GameObject passCardsPanel;
    CardsUIManager cardsUIManager;
    GameScript game;
    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        game = GameScript.Instance;

        game.Deal.OnCardsDealt += Deal_OnCardsDealt;
        game.Deal.OnTrickFinished += Deal_OnTrickFinished;
        game.Deal.OnCardsPassed += Deal_OnCardsPassed;

        cardsUIManager = GetComponentInChildren<CardsUIManager>();
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

    private void Deal_OnCardsDealt(bool waitPass)
    {
        mainPlayer = (MainPlayer)game.Deal.Players[0];
        mainPlayer.OnPlayerTurn += PlayerTurn;
        mainPlayer.OnWaitPassCards += MainPlayer_OnWaitPassCards;

        SetPlayers(game.Deal.Players);
        cardsUIManager.ResetScores();
        DealCards(waitPass);
    }

    private void MainPlayer_OnWaitPassCards()
    {
        passCardsPanel.SetActive(true);
    }

    public void PlayerTurn(bool firstHand)
    {
        cardsUIManager.SetPlayableCards(game.Deal.TrickInfo, mainPlayer, firstHand);
    }

    public void DealCards(bool waitPass)
    {
        cardsUIManager.ShowPlayerCards(mainPlayer,waitPass);
    }

    public void OnDisable()
    {

    }

    internal void PassCards(List<Card> selectedPassCards)
    {
        mainPlayer.PassCards(selectedPassCards);
        passCardsPanel.SetActive(false);
        cardsUIManager.RemovePassedCards();

    }
}
