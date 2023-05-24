using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Threading.Tasks;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    PlayerBase mainPlayer;
    protected CardsUIManager CardsUI;

    [SerializeField]
    protected RoundResult DealFinishedPanel;

    public GameScriptBase Game { set; protected get; }
    internal bool GameOver;

    public UIElementsHolder UIElementsHolder { get; private set; }

    public void Init()
    {
        //cardsUIManager = GetComponentInChildren<CardsUIManager>();
        MultiGameScript.OnMessageRecieved += MessageRecieved;
        UIElementsHolder = GetComponent<UIElementsHolder>();

        Game.OnCardsReady += Game_OnCardsReady;
        Game.OnDealFinished += Game_OnDealFinished;
        Game.OnTrickFinished += Game_OnTrickFinished;
        Game.OnStartPlaying += Game_OnStartPlaying;
        Game.OnSetPlayEnvironment += Game_OnSetPlayEnvironment;
        Game.OnGameReady += Game_OnGameReady;
        //remove later
        CardsUI.SetCardBack(UIElementsHolder.CardBack);

        //uncomment later
        FadeScreen.Instance?.FadeOut(2);
    }

    protected virtual void Game_OnGameReady()
    {

    }

    public virtual async Task Game_OnCardsReady()
    {
        await Task.Yield();
    }

    public void SetCardManager(CardsUIManager cardsUIManager)
    {
        this.CardsUI = cardsUIManager;
        cardsUIManager.SetCardBack(UIElementsHolder.CardBack);
    }

    public void Game_OnSetPlayEnvironment(Sprite tableTop, Sprite cardBack)
    {
        UIElementsHolder.TableTop.sprite = tableTop;
        CardsUI.SetCardBack(cardBack);
    }

    public void Game_OnStartPlaying(bool isMulti)
    {
        UIElementsHolder.WaitingPanel.Hide();
        SetPlayer();

        UIElementsHolder.EmojiButton.SetActive(isMulti);
    }

    protected virtual void Game_OnTrickFinished(int winningHand)
    {
        SetScore();

        int index = CorrectIndex(winningHand);
        CardsUI.RemoveCards(index);
        GameSFXManager.Instance.PlayClipRandom("CardDraw");
    }

    public void PauseGame()
    {
        UIElementsHolder.PausePanel.Show();
        GameSFXManager.Instance.PlayClip("Click");
    }

    protected virtual void Game_OnDealFinished(bool hostPlayer, bool isGameOver)
    {
        UIElementsHolder.ScoresHolder.SetActive(false);
        UIElementsHolder.EmojiButton.SetActive(false);
        GameOver = isGameOver;

        if (isGameOver)
        {
            UIElementsHolder.GamePanel.SetActive(false);
            DealFinishedPanel.ShowRound(Game, false, true, (rank) =>
            {
                if (GameManager.Instance.GameType != GameType.Single)
                {
                    UIElementsHolder.LevelPanel.Open(rank, Game.Players[Game.MainPlayerIndex].TotalScore, () =>
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
        if (hostPlayer)
        {
            DealFinishedPanel.ShowRound(Game, false, false, (rank) =>
            {
                Game.StartNextDeal();
            });
        }
        else
        {
            DealFinishedPanel.ShowRound(Game, false, false, null);
        }

    }


    public void SetCardLocations()
    {
        CardsUI.SetCardLocations();
    }

    public void ShowScores()
    {
        DealFinishedPanel.ShowInGame(Game);
    }

    public void HideScores()
    {
        DealFinishedPanel.gameObject.SetActive(false);
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
            ((ILeaveRoom)Game).LeaveRoom();
        }
    }

    public virtual void SetScore()
    {
        
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



    //public void AddDebugWeight(int playerIndex, Card card, int Weight)
    //{
    //    UIElementsHolder.DebugCards[playerIndex].ShowWeight(card, Weight);
    //}

    public void SetPlayer()
    {
        UIElementsHolder.ScoresHolder.SetActive(true);

        for (int i = 1; i < Game.Players.Length; i++)
        {
            int correctIndex = i + Game.MainPlayerIndex;
            correctIndex %= 4;

            CardsUI.SetPlayers(i, Game.Players[correctIndex]);
        }
    }



    public int CorrectIndex(int index)
    {
        int correctedIndex = index - Game.MainPlayerIndex;
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

    public virtual void Player_OnCardReady(int playerIndex, Card card)
    {
        GameSFXManager.Instance.PlayClipRandom("Card");
        int index = CorrectIndex(playerIndex);

        if (GameManager.Instance.GameType != GameType.Single)
        {
            CardsUI.StopTimer(index);
        }
        if (playerIndex != Game.MainPlayerIndex)
        {
            CardsUI.CardsPlayed(index, card);
        }
    }

    public void Player_OnPlayerTurn(int playerIndex, RoundInfo info)
    {
        //print("player Index:" + playerIndex);
        if (GameManager.Instance.GameType != GameType.Single)
        {
            int index = CorrectIndex(playerIndex);
            //print(index);
            CardsUI.WaitPlayer(index);
        }
        if (playerIndex == Game.MainPlayerIndex)
        {
            CardsUI.SetPlayableCards(info, mainPlayer);
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

        if (!UIElementsHolder.EmojiPanel.activeSelf)
        {
            UIElementsHolder.EmojiPanel.SetActive(true);
        }
        else
        {
            UIElementsHolder.EmojiPanel.SetActive(false);
        }
    }

    public void SendEmoji(int index)
    {
        ((ISendMessage)Game).SendMessage(index);

        ShowEmoji(0, index);

        UIElementsHolder.EmojiPanel.SetActive(false);
    }

    protected void Disable()
    {
        MultiGameScript.OnMessageRecieved -= MessageRecieved;

        foreach (var player in Game.Players)
        {
            player.OnCardReady -= Player_OnCardReady;
            player.OnPlayerTurn -= Player_OnPlayerTurn;
        }

        Game.OnCardsReady -= Game_OnCardsReady;
        Game.OnTrickFinished -= Game_OnTrickFinished;
        Game.OnDealFinished -= Game_OnDealFinished;
        Game.OnStartPlaying -= Game_OnStartPlaying;
        Game.OnSetPlayEnvironment -= Game_OnSetPlayEnvironment;
    }

    public void ShowEmoji(int playerIndex, int index)
    {
        UIElementsHolder.EmojiImages[playerIndex].sprite = UIElementsHolder.Emojes[index];
        UIElementsHolder.EmojiImages[playerIndex].transform.parent.gameObject.SetActive(true);
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
        UIElementsHolder.EmojiImages[playerIndex].transform.parent.gameObject.SetActive(false);
    }

    public void MainPlayer_WaitOthers()
    {
        UIElementsHolder.WaitingPanel.Show();
    }



    //internal void Debug(string v)
    //{
    //    debugText.text = v;
    //}


    internal void HostLeft()
    {
        if (GameOver)
            return;

        UIElementsHolder.HostLeftPopup.ShowWithCode("hostleftMessage", ()=>
        {
            GameManager.Instance.AddCoins(GameManager.Instance.Bet);
            GoToMainMenu();
        });
    }
}
