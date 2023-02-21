using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManagerBaloot : UIManager
{
    GameScriptBaloot balootGame => (GameScriptBaloot)Game;
    //public static BalootUIManager Instance;
    BalootCardsUIManager balootCardsUI => (BalootCardsUIManager)CardsUI;
    bool init;

    [SerializeField]
    GameTypePanel gameTypePanel;
    [SerializeField]
    SelectShapePanel selectShapePanel;
    [SerializeField]
    GameInfoPanel gameInfoPanel;

    [SerializeField]
    TextMeshProUGUI[] typeTexts;
    BalootMainPlayer mainPlayer;

    private void OnEnable()
    {
        Instance = this;
        Game = GameScriptBaloot.Instance;

        CardsUI = GetComponentInChildren<BalootCardsUIManager>();

        Init();

        balootGame.OnStartCardsReady += BalootUIManager_OnStartCardsReady;
        balootGame.OnPlayerSelectedType += BalootGame_OnPlayerSelectedType;
        balootGame.OnRestartDeal += BalootGame_OnRestartDeal;
        gameTypePanel.OnGameTypeSelected += GameTypePanel_OnGameTypeSelected;
        gameTypePanel.OnOtherHokumSelected += GameTypePanel_OnOtherHokumSelected;
        selectShapePanel.OnShapeSelected += SelectShapePanel_OnShapeSelected;
        SetCardManager(balootCardsUI);
    }

    private void GameTypePanel_OnOtherHokumSelected()
    {
        selectShapePanel.Show(balootGame.balootRoundScript.BalootCard.Shape);
    }

    private void SelectShapePanel_OnShapeSelected(CardShape shape)
    {
        balootGame.balootRoundScript.HokumShape = shape;
        mainPlayer.SelectType(BalootGameType.Hokum);
    }

    private void BalootGame_OnRestartDeal()
    {
        balootCardsUI.RemoveAllCards();
        UIElementsHolder.ScoresHolder.SetActive(false);
    }

    private void BalootGame_OnPlayerSelectedType(int index, BalootGameType type)
    {
        typeTexts[index].text= type.ToString();
        typeTexts[index].transform.parent.gameObject.SetActive(true);

        StartCoroutine(ShowTypeAnimation(index));
    }

    IEnumerator ShowTypeAnimation(int playerIndex)
    {
        yield return new WaitForSeconds(1);
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
        }

        balootCardsUI.AddBalootCard(balootCard);
        balootCardsUI.ShowPlayerCards(mainPlayer, false, 5);


        //SetScore();
    }

    protected override void Game_OnDealFinished(bool hostPlayer, bool isGameOver)
    {
        base.Game_OnDealFinished(hostPlayer, isGameOver);
        gameInfoPanel.gameObject.SetActive(false);
    }

    protected override void Game_OnTrickFinished(int winningHand)
    {
        base.Game_OnTrickFinished(winningHand);
        gameInfoPanel.UpdateScore(Game.Players);
    }

    private void GameTypePanel_OnGameTypeSelected(BalootGameType type)
    {
        mainPlayer.SelectType(type);    
    }

    public override void Game_OnCardsReady()
    {
        balootCardsUI.AddRemaingCards(mainPlayer);

        if (balootGame.balootRoundScript.RoundType == BalootGameType.Hokum)
            gameInfoPanel.ShowHokum(balootGame.balootRoundScript.HokumShape);
        else
            gameInfoPanel.ShowSuns();
        //cardsUIManager.ShowPlayerCards(mainPlayer, true);
    }

    private void MainPlayer_OnWaitSelectType()
    {
        gameTypePanel.Show(balootGame.balootRoundScript.BiddingRound, 
            balootGame.balootRoundScript.HokumIndex,Game.MainPlayerIndex);
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
