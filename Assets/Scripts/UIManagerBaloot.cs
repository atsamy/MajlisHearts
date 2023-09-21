using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManagerBaloot : UIManager
{
    GameScriptBaloot balootGame => (GameScriptBaloot)Game;
    //public static BalootUIManager Instance;
    BalootCardsUIManager balootCardsUI => (BalootCardsUIManager)CardsUI;

    [SerializeField]
    GameTypePanel gameTypePanel;
    [SerializeField]
    SelectShapePanel selectShapePanel;
    [SerializeField]
    GameInfoPanel gameInfoPanel;
    [SerializeField]
    ProjectsPanel projectsPanel;
    [SerializeField]
    DoubleHokumPanel doubleHokumPanel;

    [SerializeField]
    TextMeshProUGUI[] typeTexts;
    MainPlayerBaloot mainPlayer;

    [SerializeField]
    WordEffectScript wordEffect;
    [SerializeField]
    TextMeshProUGUI word;

    [SerializeField]
    Image cardsPower;

    [SerializeField]
    Sprite[] cardsPowerSprites;

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
        projectsPanel.OnProjectAdded += ProjectsPanel_OnProjectAdded;
        doubleHokumPanel.OnDoublePressed += DoubleHokumPanel_OnDoublePressed;
        balootGame.OnRevealProject += BalootGame_OnRevealProject;
        balootGame.OnHideProject += BalootGame_OnHideProject;
        balootGame.OnRoundDoubled += BalootGame_OnRoundDoubled;
        balootGame.balootRoundScript.OnGameTypeSelected += BalootRoundScript_OnGameTypeSelected;

        SetCardManager(balootCardsUI);
    }

    private void BalootRoundScript_OnGameTypeSelected(int arg1, BalootGameType arg2)
    {
        cardsPower.gameObject.SetActive(true);

        if (balootGame.balootRoundScript.RoundType == BalootGameType.Hokum)
        {
            gameInfoPanel.ShowHokum(balootGame.balootRoundScript.balootRoundInfo.HokumShape,
                balootGame.DoubleValue, balootGame);


            wordEffect.ShowWordAndIcon(balootGame.balootRoundScript.RoundType.ToString(), balootGame.balootRoundScript.balootRoundInfo.HokumShape);

            cardsPower.sprite = cardsPowerSprites[1];
        }
        else
        {
            gameInfoPanel.ShowSuns(balootGame);
            wordEffect.ShowWord(balootGame.balootRoundScript.RoundType.ToString());


            cardsPower.sprite = cardsPowerSprites[0];
        }


    }

    private void BalootGame_OnRoundDoubled(int playerIndex, int doubleValue)
    {
        string text = "";
        switch (doubleValue)
        {
            case 2:
                text = "double2";
                break;
            case 3:
                text = "triple";
                break;
            case 4:
                text = "quadruple";
                break;
            case 5:
                text = "Qahwa";
                break;
        }

        ShowPlayerText(playerIndex, text);
    }

    private void ShowPlayerText(int playerIndex, string message)
    {
        playerIndex = CorrectIndex(playerIndex);

        typeTexts[playerIndex].text = LanguageManager.Instance.GetString(message);
        typeTexts[playerIndex].transform.parent.gameObject.SetActive(true);

        StartCoroutine(ShowTypeAnimation(playerIndex));
    }

    private void DoubleHokumPanel_OnDoublePressed(bool isDouble, int value)
    {
        mainPlayer.SelectDouble(isDouble, value);
    }

    private void BalootGame_OnHideProject()
    {
        projectsPanel.gameObject.SetActive(false);
    }

    protected override void Game_OnGameReady()
    {
        mainPlayer = (MainPlayerBaloot)Game.Players[Game.MainPlayerIndex];
        mainPlayer.OnWaitSelectType += MainPlayer_OnWaitSelectType;
        mainPlayer.WaitOthers += MainPlayer_WaitOthers;
        mainPlayer.OnCheckDouble += MainPlayer_OnCheckDouble;
        mainPlayer.OnCancelDouble += MainPlayer_OnCancelDouble;

        balootCardsUI.SetMainPlayer(mainPlayer);

        SetPlayers(Game.Players, mainPlayer);

        foreach (PlayerBaloot item in Game.Players)
        {
            item.BalootCardsPlayed += Item_BalootCardsPlayed;
        }
    }

    private void MainPlayer_OnCancelDouble()
    {
        doubleHokumPanel.gameObject.SetActive(false);
    }

    private void MainPlayer_OnCheckDouble(int index,int doubleValue)
    {
        doubleHokumPanel.Show(doubleValue);
    }

    private void BalootGame_OnRevealProject(int index)
    {
        int correctIndex = CorrectIndex(index);
        balootCardsUI.RevealCards(correctIndex, ((PlayerBaloot)Game.Players[index]).PlayerProjects);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(2);
        }
    }

    private void ProjectsPanel_OnProjectAdded(Projects project, int count)
    {
        mainPlayer.AddProject(project, count);
    }

    private void GameTypePanel_OnOtherHokumSelected()
    {
        selectShapePanel.Show(balootGame.balootRoundScript.BalootCard.Shape);
    }

    private void SelectShapePanel_OnShapeSelected(CardShape shape)
    {
        mainPlayer.ChangedHokumShape(shape);
        balootGame.balootRoundScript.balootRoundInfo.HokumShape = shape;
        mainPlayer.SelectType(BalootGameType.Hokum);
    }

    private void BalootGame_OnRestartDeal()
    {
        balootCardsUI.RemoveAllCards();
        UIElementsHolder.ScoresHolder.SetActive(false);

        cardsPower.gameObject.SetActive(false);
    }

    private void BalootGame_OnPlayerSelectedType(int index, BalootGameType type)
    {
        ShowPlayerText(index, type.ToString());
    }

    IEnumerator ShowTypeAnimation(int playerIndex)
    {
        yield return new WaitForSeconds(1);
        typeTexts[playerIndex].transform.parent.gameObject.SetActive(false);
    }

    private async void BalootUIManager_OnStartCardsReady(Card balootCard, int startIndex)
    {
        ResetScore();
        HideScores();
        UIElementsHolder.ScoresHolder.SetActive(true);
        await balootCardsUI.ShowPlayerCardsBaloot(mainPlayer, balootCard, startIndex);
        balootGame.CheckType();
    }

    private void Item_BalootCardsPlayed()
    {
        wordEffect.ShowWord("baloot");
    }





    protected override void Game_OnDealFinished(bool hostPlayer, bool isGameOver)
    {
        base.Game_OnDealFinished(hostPlayer, isGameOver);
        gameInfoPanel.gameObject.SetActive(false);

        cardsPower.gameObject.SetActive(false);
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

    public override async Task Game_OnCardsReady()
    {
        await balootCardsUI.AddRemaingCards(mainPlayer, balootGame.balootRoundScript.RoundType, balootGame.DeclarerIndex);

        projectsPanel.Show(balootGame.balootRoundScript.RoundType);
        doubleHokumPanel.gameObject.SetActive(false);


        //if (GameManager.Instance.GameType == GameType.Single)
        //    Game.SetStartGame();
    }

    private void MainPlayer_OnWaitSelectType(RoundScriptBaloot roundScript)
    {
        gameTypePanel.Show(roundScript.BiddingRound, roundScript.HokumIndex, Game.MainPlayerIndex);
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

    public void ResetScore()
    {
        for (int i = 0; i < Game.Players.Length; i++)
        {
            balootCardsUI.SetScore(i, 0);
        }
    }

    private void OnDisable()
    {
        base.Disable();

        mainPlayer.OnWaitSelectType -= MainPlayer_OnWaitSelectType;
        mainPlayer.WaitOthers -= MainPlayer_WaitOthers;

        balootGame.OnStartCardsReady -= BalootUIManager_OnStartCardsReady;
        balootGame.OnPlayerSelectedType -= BalootGame_OnPlayerSelectedType;
        balootGame.balootRoundScript.OnGameTypeSelected -= BalootRoundScript_OnGameTypeSelected;
        gameTypePanel.OnGameTypeSelected -= GameTypePanel_OnGameTypeSelected;
    }
}
