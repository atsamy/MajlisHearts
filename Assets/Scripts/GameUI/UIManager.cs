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
    MainPlayer mainPlayer;
    public static UIManager Instance;

    [SerializeField]
    private Text debugText;
    [SerializeField]
    GameObject passCardsPanel;
    [SerializeField]
    DealResult DealFinishedPanel;
    [SerializeField]
    DealResult GameFinishedPanel;
    [SerializeField]
    DealResult TeamResultPanel;
    [SerializeField]
    GameObject waitingPanel;

    public DoublePanelScript DoublePanel;

    public GameObject Scores;

    public Text PassText;

    CardsUIManager cardsUIManager;
    GameScript game;

    public DebugCards[] debugCards;

    int doubleCardCount;
    [Space]
    [Header("Emojis Elements")]
    [Space]
    public Image[] EmojiImages;
    public Sprite[] Emojes;
    public GameObject EmojiButton;
    public GameObject EmojiPanel;
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

        cardsUIManager = GetComponentInChildren<CardsUIManager>();
        MultiGameScript.OnMessageRecieved += MessageRecieved;
    }

    internal void SetDoubleCard(Card card, bool value)
    {
        if (doubleCardCount == 1)
            waitingPanel.SetActive(true);

        mainPlayer.SetDoubleCard(card, value);
    }

    private void Game_OnCardDoubled(Card card, int playerIndex)
    {
        int index = CorrectIndex(playerIndex);
        cardsUIManager.AddDoubledCard(card, index);
    }

    private void Game_OnStartPlaying(bool isMulti)
    {
        waitingPanel.SetActive(false);
        SetScore();

        EmojiButton.SetActive(isMulti);
    }

    private void CardsPassed()
    {
        cardsUIManager.UpdateCards(mainPlayer);
    }

    private void Game_OnDealFinished(bool hostPlayer,bool isGameOver)
    {
        Player[] players = game.Players;
        players = players.OrderBy(a => a.TotalScore).ToArray();
        doubleCardCount = 0;
        Scores.SetActive(false);
        EmojiButton.SetActive(false);

        if (isGameOver)
        {
            GameFinishedPanel.Show(players, () => { SceneManager.LoadScene(0); });
            return;
        }

        if (hostPlayer)
        {
            DealFinishedPanel.Show(players, () =>
             {
                 game.StartNextDeal();
             });
        }
        else
        {
            DealFinishedPanel.Show(players, null);
        }
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
    }

    public void SetScore()
    {
        for (int i = 0; i < game.Players.Length; i++)
        {
            int correctIndex = i + game.MainPlayerIndex;
            correctIndex %= 4;

            cardsUIManager.SetScore(i, game.Players[correctIndex]);
        }
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
    }

    void MessageRecieved(int playerIndex, object message)
    {
        int index = CorrectIndex(playerIndex);
        ShowEmoji(index, (int)message);
    }

    public void OpenEmojiPanel()
    {
        EmojiPanel.SetActive(true);
    }

    public void SendEmoji(int index)
    {
        ((MultiGameScript)game).SendMessageToOthers(index);

        ShowEmoji(0, index);

        EmojiPanel.SetActive(false);
    }

    public void ShowEmoji(int playerIndex, int index)
    {
        EmojiImages[playerIndex].sprite = Emojes[index];

        EmojiImages[playerIndex].gameObject.SetActive(true);
        EmojiImages[playerIndex].transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.InOutCubic).OnComplete(() =>
        {
            StartCoroutine(ShowEmoAnimation(playerIndex));
        });
    }

    IEnumerator ShowEmoAnimation(int playerIndex)
    {
        yield return new WaitForSeconds(3);
        EmojiImages[playerIndex].transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.Flash);
        EmojiImages[playerIndex].gameObject.SetActive(false);
    }

    private void Game_OnCardsDealt()
    {
        if (!init)
        {
            mainPlayer = (MainPlayer)game.Players[game.MainPlayerIndex];
            mainPlayer.OnPlayerTurn += PlayerTurn;
            mainPlayer.OnWaitPassCards += MainPlayer_OnWaitPassCards;
            mainPlayer.OnWaitDoubleCards += MainPlayer_OnWaitDoubleCards;
            cardsUIManager.SetMainPlayer(mainPlayer);

            SetPlayers(game.Players);
            init = true;
        }
        SetScore();
        cardsUIManager.ShowPlayerCards(mainPlayer, true);
    }

    private void MainPlayer_OnWaitDoubleCards(Card card)
    {
        doubleCardCount++;
        waitingPanel.SetActive(false);
        DoublePanel.ShowPanel(card);
    }

    internal void Debug(string v)
    {
        debugText.text = v;
    }

    private void MainPlayer_OnWaitPassCards()
    {
        DealFinishedPanel.gameObject.SetActive(false);
        passCardsPanel.SetActive(true);
        PassText.text = "Pass Right";
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
        waitingPanel.SetActive(true);

        passCardsPanel.SetActive(false);
        mainPlayer.PassCards(selectedPassCards);
        cardsUIManager.RemovePassedCards();

        Scores.SetActive(true);
    }
}
