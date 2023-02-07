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
    public LevelPanel LevelPanel;
    public WaitingScript WaitingPanel;
    [SerializeField]
    Image tableTop;
    [SerializeField]
    public GameObject scoresHolder;
    [SerializeField]
    Popup hostLeftPopup;
    public GameObject GamePanel;

    PlayerBase mainPlayer;
    public Transform DragCardHolder;

    CardsUIManager cardsUIManager;
    //GameScript game;
    public DebugCards[] debugCards;


    [Space]
    [Header("Emojis Elements")]
    [Space]
    public Image[] EmojiImages;
    public Sprite[] Emojes;
    public GameObject EmojiButton;
    public GameObject EmojiPanel;

    public GameScriptBase game { set; private get; }
    internal bool GameOver;

    public Sprite cardBack;
    void Awake()
    {
        Instance = this;
        cardsUIManager = GetComponentInChildren<CardsUIManager>();
        MultiGameScript.OnMessageRecieved += MessageRecieved;

        //remove later
        cardsUIManager.SetCardBack(cardBack);

        //uncomment later
        //FadeScreen.Instance?.FadeOut(2);
    }

    public void Game_OnSetPlayEnvironment(Sprite tableTop, Sprite cardBack)
    {
        this.tableTop.sprite = tableTop;
        cardsUIManager.SetCardBack(cardBack);
    }

    public void Game_OnStartPlaying(bool isMulti)
    {
        WaitingPanel.Hide();
        SetPlayer();

        EmojiButton.SetActive(isMulti);
    }

    public void PauseGame()
    {
        pausePanel.Show();
        GameSFXManager.Instance.PlayClip("Click");
    }



    public void SetCardLocations()
    {
        cardsUIManager.SetCardLocations();
    }



    public void GoToMainMenu()
    {
        LeaveRoom();
        FadeScreen.Instance.FadeIn(2, () =>
        {
            SceneManager.LoadScene(1);
        });
    }


    public void LeaveRoom()
    {
        if (GameManager.Instance.GameType == GameType.Friends || GameManager.Instance.GameType == GameType.Online)
        {
            ((MultiGameScript)game).LeaveRoom();
        }
    }

    //public Player[] OrderTeamPlayers()
    //{
    //    if (!GameManager.Instance.IsTeam)
    //    {
    //        return game.Players.OrderBy(a => a.TotalScore).ToArray();
    //    }

    //    Player[] players = game.Players;

    //    int index = Array.FindIndex(players, a => a.Score == players.Max(b => b.Score));

    //    Player[] orderedPlayers = new Player[4];
    //    orderedPlayers[3] = players[index];
    //    orderedPlayers[2] = players[(index + 2) % 4];

    //    Player player1 = players[(index + 1) % 4];
    //    Player player2 = players[(index + 3) % 4];

    //    if (player1.Score > player2.Score)
    //    {
    //        orderedPlayers[0] = player2;
    //        orderedPlayers[1] = player1;
    //    }
    //    else
    //    {
    //        orderedPlayers[0] = player1;
    //        orderedPlayers[1] = player2;
    //    }

    //    return orderedPlayers;
    //}



    public void AddDebugWeight(int playerIndex, Card card, int Weight)
    {
        debugCards[playerIndex].ShowWeight(card, Weight);
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



    public int CorrectIndex(int index)
    {
        int correctedIndex = index - game.MainPlayerIndex;
        correctedIndex = (correctedIndex < 0 ? correctedIndex + 4 : correctedIndex);

        return correctedIndex;
    }

    public void SetPlayers(PlayerBase[] players,PlayerBase mainPlayer)
    {
        foreach (var player in players)
        {
            player.OnCardReady += Player_OnCardReady;
            player.OnPlayerTurn += Player_OnPlayerTurn;
        }

        this.mainPlayer= mainPlayer;
    }

    public void Player_OnCardReady(int playerIndex, Card card)
    {
        GameSFXManager.Instance.PlayClipRandom("Card");
        int index = CorrectIndex(playerIndex);

        if (GameManager.Instance.GameType != GameType.Single)
        {
            cardsUIManager.StopTimer(index);
        }
        if (playerIndex != game.MainPlayerIndex)
        {
            cardsUIManager.CardsPlayed(index, card);
        }
    }

    public void Player_OnPlayerTurn(int playerIndex, RoundInfo info)
    {
        //print("player Index:" + playerIndex);
        if (GameManager.Instance.GameType != GameType.Single)
        {
            int index = CorrectIndex(playerIndex);
            //print(index);
            cardsUIManager.WaitPlayer(index);
        }
        if (playerIndex == game.MainPlayerIndex)
        {
            cardsUIManager.SetPlayableCards(info, mainPlayer);
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

        if (!EmojiPanel.activeSelf)
        {
            EmojiPanel.SetActive(true);
        }
        else
        {
            EmojiPanel.SetActive(false);
        }
    }

    public void SendEmoji(int index)
    {
        ((MultiGameScript)game).SendMessageToOthers(index);

        ShowEmoji(0, index);

        EmojiPanel.SetActive(false);
    }

    private void OnDisable()
    {
        MultiGameScript.OnMessageRecieved -= MessageRecieved;
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

    public void MainPlayer_WaitOthers()
    {
        WaitingPanel.Show();
    }



    internal void Debug(string v)
    {
        debugText.text = v;
    }


    internal void HostLeft()
    {
        if (GameOver)
            return;

        hostLeftPopup.ShowWithCode("hostleftMessage", ()=>
        {
            GameManager.Instance.AddCoins(GameManager.Instance.Bet);
            GoToMainMenu();
        });
    }
}
