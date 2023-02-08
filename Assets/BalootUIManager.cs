using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BalootUIManager : MonoBehaviour
{
    BalootGameScript game;
    public static BalootUIManager Instance;
    BalootCardsUIManager cardsUIManager;
    UIManager uiManager;
    bool init;

    [SerializeField]
    GameTypePanel gameTypePanel;

    [SerializeField]
    TextMeshProUGUI[] typeTexts;
    BalootMainPlayer mainPlayer;

    BalootGameScript balootGame => (BalootGameScript)game;
    void Awake()
    {
        Instance = this;
        game = BalootGameScript.Instance;
        uiManager = GetComponent<UIManager>();
        uiManager.game = game;

        game.OnCardsReady += Game_OnCardsReady;
        balootGame.OnStartCardsReady += BalootUIManager_OnStartCardsReady;
        balootGame.OnPlayerSelectedType += BalootGame_OnPlayerSelectedType;
        gameTypePanel.OnGameTypeSelected += GameTypePanel_OnGameTypeSelected;

        cardsUIManager = GetComponentInChildren<BalootCardsUIManager>();
        uiManager.SetCardManager(cardsUIManager);
    }

    private void BalootGame_OnPlayerSelectedType(int index, BalootGameType type)
    {
        typeTexts[index].text= type.ToString();
        typeTexts[index].transform.parent.gameObject.SetActive(true);

        StartCoroutine(ShowTypeAnimation(index));
    }

    IEnumerator ShowTypeAnimation(int playerIndex)
    {
        yield return new WaitForSeconds(3);
        typeTexts[playerIndex].transform.parent.gameObject.SetActive(false);
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

            //for (int i = 0; i < 4; i++)
            //{
            //    ((BalootPlayer)game.Players[i]).OnTypeSelected +=  
            //}
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
        cardsUIManager.AddRemaingCards(mainPlayer);
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
