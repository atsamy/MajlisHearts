using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalootUIManager : MonoBehaviour
{
    BalootGameScript game;
    public static BalootUIManager Instance;
    CardsUIManager cardsUIManager;
    UIManager uiManager;
    bool init;

    [SerializeField]
    GameTypePanel gameTypePanel;

    BalootMainPlayer mainPlayer;
    void Awake()
    {
        Instance = this;
        game = BalootGameScript.Instance;

        uiManager = GetComponent<UIManager>();
        uiManager.game = game;

        game.OnCardsReady += Game_OnCardsReady;
        ((BalootGameScript)game).OnStartCardsReady += BalootUIManager_OnStartCardsReady;

        gameTypePanel.OnGameTypeSelected += GameTypePanel_OnGameTypeSelected;

        cardsUIManager = GetComponentInChildren<CardsUIManager>();  
    }

    private void BalootUIManager_OnStartCardsReady(Card balootCard)
    {
        if (!init)
        {
            mainPlayer = (BalootMainPlayer)game.Players[game.MainPlayerIndex];
            mainPlayer.OnWaitSelectType += MainPlayer_OnWaitSelectType;
            //mainPlayer.OnWaitPassCards += MainPlayer_OnWaitPassCards;
            //mainPlayer.OnWaitDoubleCards += MainPlayer_OnWaitDoubleCards;
            mainPlayer.WaitOthers += uiManager.MainPlayer_WaitOthers;
            cardsUIManager.SetMainPlayer(mainPlayer);

            uiManager.SetPlayers(game.Players, mainPlayer);
            init = true;
        }

        cardsUIManager.AddBalootCard(balootCard);
        cardsUIManager.ShowPlayerCards(mainPlayer, false,5);
        //SetScore();
    }

    private void GameTypePanel_OnGameTypeSelected(BalootGameType type)
    {
        mainPlayer.SelectType(type);    
    }

    private void Game_OnCardsReady()
    {

        //cardsUIManager.ShowPlayerCards(mainPlayer, true);
    }

    private void MainPlayer_OnWaitSelectType()
    {
        gameTypePanel.Show();
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
}
