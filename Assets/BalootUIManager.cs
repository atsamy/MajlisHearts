using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BalootUIManager : UIManager
{
    BalootGameScript balootGame => (BalootGameScript)Game;
    //public static BalootUIManager Instance;
    BalootCardsUIManager balootCardsUI => (BalootCardsUIManager)CardsUI;
    bool init;

    [SerializeField]
    GameTypePanel gameTypePanel;

    [SerializeField]
    TextMeshProUGUI[] typeTexts;
    BalootMainPlayer mainPlayer;

    private void OnEnable()
    {
        Instance = this;
        Game = BalootGameScript.Instance;

        CardsUI = GetComponentInChildren<BalootCardsUIManager>();

        Init();

        balootGame.OnStartCardsReady += BalootUIManager_OnStartCardsReady;
        balootGame.OnPlayerSelectedType += BalootGame_OnPlayerSelectedType;

        gameTypePanel.OnGameTypeSelected += GameTypePanel_OnGameTypeSelected;
        SetCardManager(balootCardsUI);
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
            mainPlayer = (BalootMainPlayer)Game.Players[Game.MainPlayerIndex];
            mainPlayer.OnWaitSelectType += MainPlayer_OnWaitSelectType;
            //mainPlayer.OnWaitPassCards += MainPlayer_OnWaitPassCards;
            //mainPlayer.OnWaitDoubleCards += MainPlayer_OnWaitDoubleCards;
            mainPlayer.WaitOthers += MainPlayer_WaitOthers;
            balootCardsUI.SetMainPlayer(mainPlayer);

            SetPlayers(Game.Players, mainPlayer);
            init = true;

            //for (int i = 0; i < 4; i++)
            //{
            //    ((BalootPlayer)game.Players[i]).OnTypeSelected +=  
            //}

            balootCardsUI.AddBalootCard(balootCard);
            balootCardsUI.ShowPlayerCards(mainPlayer, false, 5);
        }


        //SetScore();
    }

    private void GameTypePanel_OnGameTypeSelected(BalootGameType type)
    {
        mainPlayer.SelectType(type);    
    }

    public override void Game_OnCardsReady()
    {
        balootCardsUI.AddRemaingCards(mainPlayer);

        //cardsUIManager.ShowPlayerCards(mainPlayer, true);
    }

    private void MainPlayer_OnWaitSelectType()
    {
        gameTypePanel.Show();
    }

    public override void SetScore()
    {
        for (int i = 0; i < Game.Players.Length; i++)
        {
            int correctIndex = i + Game.MainPlayerIndex;
            correctIndex %= 4;

            balootCardsUI.SetScore(i, Game.Players[correctIndex].Score);
        }
    }

    private void OnDisable()
    {
        base.Disable();

        mainPlayer.OnWaitSelectType -= MainPlayer_OnWaitSelectType;
        mainPlayer.WaitOthers -= MainPlayer_WaitOthers;

        balootGame.OnStartCardsReady -= BalootUIManager_OnStartCardsReady;
        balootGame.OnPlayerSelectedType -= BalootGame_OnPlayerSelectedType;

        gameTypePanel.OnGameTypeSelected -= GameTypePanel_OnGameTypeSelected;
    }
}
