using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField]
    Text debugText;
    [SerializeField]
    PausePanel pausePanel;
    [SerializeField]
    PassCardsPanel passCardsPanel;
    [SerializeField]
    DealResult DealFinishedPanel;
    //[SerializeField]
    //ResultPanel ResultPanel;
    [SerializeField]
    LevelPanel LevelPanel;
    [SerializeField]
    WaitingScript waitingPanel;
    [SerializeField]
    Image tableTop;
    [SerializeField]
    GameObject scoresHolder;

    MainPlayer mainPlayer;

    public DoublePanelScript DoublePanel;
    public Transform DragCardHolder;

    CardsUIManager cardsUIManager;
    GameScript game;
    public DebugCards[] debugCards;

    int doubleCardCount;
    [Space]
    [Header("Emojis Elements")]
    [Space]
    public Image[] EmojiImages;
    public Sprite[] Emojes;
    [SerializeField]
    GameObject emojiButton;
    [SerializeField]
    GameObject emojiPanel;
    bool init;

    void Awake()
    {
        Instance = this;
        doubleCardCount = 0;
        game = GameScript.Instance;

        game.OnCardsReady += Game_OnCardsDealt;
        game.OnTrickFinished += Game_OnTrickFinished;
        game.OnStartPlaying += Game_OnStartPlaying;
        game.OnCardsPassed += CardsPassed;
        game.OnDealFinished += Game_OnDealFinished;
        game.OnCardDoubled += Game_OnCardDoubled;
        game.OnSetPlayEnvironment += Game_OnSetPlayEnvironment;

        cardsUIManager = GetComponentInChildren<CardsUIManager>();
        MultiGameScript.OnMessageRecieved += MessageRecieved;

        FadeScreen.Instance.FadeOut(2);
    }

    private void Game_OnSetPlayEnvironment(Sprite tableTop, Sprite cardBack)
    {
        //if (GameManager.Instance.EquippedItem.ContainsKey("TableTop"))
        this.tableTop.sprite = tableTop;// Resources.Load<Sprite>("TableTop/" + GameManager.Instance.EquippedItem["TableTop"]);
        cardsUIManager.SetCardBack(cardBack);
    }

    private void Game_OnStartPlaying(bool isMulti)
    {
        waitingPanel.Hide();
        SetPlayer();

        emojiButton.SetActive(isMulti);
    }

    public void PauseGame()
    {
        pausePanel.Show();
        GameSFXManager.Instance.PlayClip("Click");
    }

    internal void SetDoubleCard(Card card, bool value)
    {
        if (doubleCardCount == 1)
            waitingPanel.gameObject.SetActive(true);

        mainPlayer.SetDoubleCard(card, value);
    }

    private void Game_OnCardDoubled(Card card, int playerIndex)
    {
        int index = CorrectIndex(playerIndex);
        cardsUIManager.AddDoubledCard(card, index);
    }

    private void CardsPassed()
    {
        waitingPanel.Hide();
        StartCoroutine(cardsUIManager.UpdateCards(mainPlayer));
    }

    public void SetCardLocations()
    {
        cardsUIManager.SetCardLocations();
    }

    private void Game_OnDealFinished(bool hostPlayer, bool isGameOver)
    {
        doubleCardCount = 0;
        scoresHolder.SetActive(false);
        emojiButton.SetActive(false);


        if (isGameOver)
        {

            DealFinishedPanel.ShowRound(game.Players,false,true, (rank) =>
                 {
                     if (GameManager.Instance.GameType != GameType.Single)
                     {
                         LevelPanel.Open(rank, () =>
                         {
                             GoToMainMenu();
                         });
                     }
                     else
                     {
                         GoToMainMenu();
                     }
                 });


            return;
        }
        //if (game.MyPlayer.Score < 6)
        //{
        //    // win effect
        //    GameSFXManager.Instance.PlayClip("Win");
        //}

        if (hostPlayer)
        {
            DealFinishedPanel.ShowRound(game.Players, false,false, (rank) =>
              {
                  game.StartNextDeal();
              });
        }
        else
        {
            DealFinishedPanel.ShowRound(game.Players, false,false, null);
        }

    }

    private void GoToMainMenu()
    {
        LeaveRoom();
        FadeScreen.Instance.FadeIn(2, () =>
        {
            SceneManager.LoadScene(1);
        });
    }

    public void ShowScores()
    {
        DealFinishedPanel.ShowInGame(game.Players);
    }

    public void LeaveRoom()
    {
        if (GameManager.Instance.GameType == GameType.Friends || GameManager.Instance.GameType == GameType.Online)
        {
            ((MultiGameScript)game).LeaveRoom();
        }
    }

    public Player[] OrderTeamPlayers()
    {
        if (!GameManager.Instance.IsTeam)
        {
            return game.Players.OrderBy(a => a.TotalScore).ToArray();
        }

        Player[] players = game.Players;

        int index = Array.FindIndex(players, a => a.Score == players.Max(b => b.Score));

        Player[] orderedPlayers = new Player[4];
        orderedPlayers[3] = players[index];
        orderedPlayers[2] = players[(index + 2) % 4];

        Player player1 = players[(index + 1) % 4];
        Player player2 = players[(index + 3) % 4];

        if (player1.Score > player2.Score)
        {
            orderedPlayers[0] = player2;
            orderedPlayers[1] = player1;
        }
        else
        {
            orderedPlayers[0] = player1;
            orderedPlayers[1] = player2;
        }

        return orderedPlayers;
    }

    internal bool AddCard(CardUI card)
    {
        GameSFXManager.Instance.PlayClipRandom("Card");
        return passCardsPanel.AddCard(card);
    }

    public void AddDebugWeight(int playerIndex, Card card, int Weight)
    {
        debugCards[playerIndex].ShowWeight(card, Weight);
    }

    private void Game_OnTrickFinished(int winningHand)
    {
        SetScore();

        int index = CorrectIndex(winningHand);
        cardsUIManager.RemoveCards(index);
        GameSFXManager.Instance.PlayClipRandom("CardDraw");
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

    public void SetPlayer()
    {
        scoresHolder.SetActive(true);

        for (int i = 1; i < game.Players.Length; i++)
        {
            int correctIndex = i + game.MainPlayerIndex;
            correctIndex %= 4;

            cardsUIManager.SetPlayers(i, game.Players[correctIndex]);
        }
    }

    internal void RemoveCard(CardUI cardUI)
    {
        GameSFXManager.Instance.PlayClipRandom("Card");
        passCardsPanel.RemoveCard(cardUI);
    }

    private int CorrectIndex(int index)
    {
        //int correctedIndex = index + game.MainPlayerIndex;
        //correctedIndex %= 4;

        int correctedIndex = index - game.MainPlayerIndex;
        correctedIndex = (correctedIndex < 0 ? correctedIndex + 4 : correctedIndex);

        return correctedIndex;
    }

    public void SetPlayers(Player[] players)
    {
        foreach (var player in players)
        {
            player.OnCardReady += Player_OnCardReady;
        }
    }

    private void Player_OnCardReady(int playerIndex, Card card)
    {
        GameSFXManager.Instance.PlayClipRandom("Card");

        if (playerIndex != game.MainPlayerIndex)
        {
            int index = CorrectIndex(playerIndex);

            cardsUIManager.CardsPlayed(index, card);
        }
    }

    void MessageRecieved(int playerIndex, object message)
    {
        int index = CorrectIndex(playerIndex);
        ShowEmoji(index, (int)message);
    }

    public void OpenEmojiPanel()
    {
        GameSFXManager.Instance.PlayClip("Click");
        emojiPanel.SetActive(true);
    }

    public void SendEmoji(int index)
    {
        ((MultiGameScript)game).SendMessageToOthers(index);

        ShowEmoji(0, index);

        emojiPanel.SetActive(false);
    }

    public void ShowEmoji(int playerIndex, int index)
    {
        EmojiImages[playerIndex].sprite = Emojes[index];
        EmojiImages[playerIndex].transform.parent.gameObject.SetActive(true);
        //EmojiImages[playerIndex].transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.InOutCubic).OnComplete(() =>
        //{
        StartCoroutine(ShowEmoAnimation(playerIndex));
        //});

        GameSFXManager.Instance.PlayClip("Pop");
    }

    IEnumerator ShowEmoAnimation(int playerIndex)
    {
        yield return new WaitForSeconds(3);
        //EmojiImages[playerIndex].transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.Flash);
        EmojiImages[playerIndex].transform.parent.gameObject.SetActive(false);
    }

    private void Game_OnCardsDealt()
    {
        if (!init)
        {
            mainPlayer = (MainPlayer)game.Players[game.MainPlayerIndex];
            mainPlayer.OnPlayerTurn += PlayerTurn;
            mainPlayer.OnWaitPassCards += MainPlayer_OnWaitPassCards;
            mainPlayer.OnWaitDoubleCards += MainPlayer_OnWaitDoubleCards;
            mainPlayer.WaitOthers += MainPlayer_WaitOthers;
            cardsUIManager.SetMainPlayer(mainPlayer);

            SetPlayers(game.Players);
            init = true;
        }
        SetScore();
        cardsUIManager.ShowPlayerCards(mainPlayer, true);
    }

    private void MainPlayer_WaitOthers()
    {
        waitingPanel.Show();
    }

    private void MainPlayer_OnWaitDoubleCards(Card card)
    {
        doubleCardCount++;
        waitingPanel.Hide();
        DoublePanel.ShowPanel(card);
    }

    internal void Debug(string v)
    {
        debugText.text = v;
    }

    private void MainPlayer_OnWaitPassCards()
    {
        DealFinishedPanel.gameObject.SetActive(false);

        passCardsPanel.Show((cards) =>
        {
            PassCards(cards);
        });
    }

    public void PlayerTurn(DealInfo info)
    {
        cardsUIManager.SetPlayableCards(info, mainPlayer);
    }

    public void OnDisable()
    {

    }

    internal void PassCards(List<Card> selectedPassCards)
    {
        waitingPanel.Show();
        mainPlayer.PassCards(selectedPassCards);
        //scoresHolder.SetActive(true);
    }
}
