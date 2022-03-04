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

    [SerializeField]
    private Text debugText;
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

        game.OnCardsReady += Game_OnCardsDealt;
        game.OnTrickFinished += Game_OnTrickFinished;
        game.OnStartPlaying += Game_OnStartPlaying;
        game.OnDealFinished += Game_OnDealFinished;

        cardsUIManager = GetComponentInChildren<CardsUIManager>();
    }

    private void Game_OnStartPlaying()
    {
        cardsUIManager.UpdateCards(mainPlayer);
    }

    private void Game_OnDealFinished()
    {
        Player[] players = game.Players;
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


    private void Game_OnTrickFinished(int winningHand)
    {
        int index = CorrectIndex(winningHand);

        for (int i = 0; i < game.Players.Length; i++)
        {
            int correctIndex = i + game.MainPlayerIndex;
            correctIndex %= 4;

            cardsUIManager.SetScore(i,game.Players[correctIndex]);
        }

        cardsUIManager.RemoveCards(index);
    }

    private int CorrectIndex(int index)
    {
        int correctedIndex = index - game.MainPlayerIndex;
        correctedIndex = (correctedIndex < 0 ? correctedIndex + 4 : correctedIndex);
        return correctedIndex;
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
        if (playerIndex != game.MainPlayerIndex)
        {
            int index = CorrectIndex(playerIndex);

            cardsUIManager.CardsPlayed(index, card);
        }
        else
            cardsUIManager.MainPlayerCard(card);
    }

    bool once;

    private void Game_OnCardsDealt()//bool waitPass)
    {
        Debug("myIndex: " + game.MainPlayerIndex);

        mainPlayer = (MainPlayer)game.Players[game.MainPlayerIndex];
        mainPlayer.OnPlayerTurn += PlayerTurn;
        mainPlayer.OnWaitPassCards += MainPlayer_OnWaitPassCards;


        if (!once)
        {
            SetPlayers(game.Players);
            once = true;
        }
        cardsUIManager.ResetScores();
        cardsUIManager.ShowPlayerCards(mainPlayer, true);
    }

    internal void Debug(string v)
    {
        debugText.text = v;
    }

    private void MainPlayer_OnWaitPassCards()
    {
        passCardsPanel.SetActive(true);
        PassText.text = game.Deal.CurrentState.ToString();
    }

    public void PlayerTurn()
    {
        cardsUIManager.SetPlayableCards(game.Deal.DealInfo, mainPlayer);
    }

    public void OnDisable()
    {

    }

    internal void PassCards(List<Card> selectedPassCards)
    {
        mainPlayer.PassCards(selectedPassCards);
        passCardsPanel.SetActive(false);
        cardsUIManager.RemovePassedCards();

        //for (int i = 1; i < 4; i++)
        //{
        //    debugCards[i - 1].UpdateCards(game.Players[i].OwnedCards);
        //}

        Scores.SetActive(true);
    }
}
