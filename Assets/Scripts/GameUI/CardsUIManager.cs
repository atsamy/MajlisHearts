using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CardsUIManager : MonoBehaviour
{
    protected GameObject cardBack;
    protected List<DeckCard> deckCards;
    protected List<CardUI> playableCards;
    protected List<CardUI> playerCardsUI;

    protected CardElementsHolder cardElementsHolder;

    public void OnEnable()
    {
        deckCards = new List<DeckCard>();
        cardElementsHolder = GetComponent<CardElementsHolder>();
    }

    public void SetMainPlayer(PlayerBase mainPlayer)
    {
        mainPlayer.OnForcePlay += () =>
        {
            playableCards[Random.Range(0, playableCards.Count)].Pressed();
        };

        cardElementsHolder.playersDetails[0].SetPlayer(mainPlayer.Avatar,mainPlayer.Name,0);
    }

    public void WaitPlayer(int index)
    {
        cardElementsHolder.playersDetails[index].StartTimer(10);
    }

    public void StopTimer(int index)
    {
        cardElementsHolder.playersDetails[index].StopTimer();
    }

    public void SetPlayerCards(PlayerBase mainPlayer)
    {
        foreach (var item in playerCardsUI)
        {
            item.SetOnPressed((card) =>
            {
                mainPlayer.ChooseCard(card);
                MainPlayerCard(item);
            });

            item.SetInteractable(false);
        }
    }

    public virtual void ShowPlayerCards(PlayerBase mainPlayer, bool setInteractable,int count)
    {
        //playerCardsUI = new List<CardUI>();
        //playableCards = new List<CardUI>();

        //for (int i = 0; i < count; i++)
        //{
        //    GameObject newCard = Instantiate(playerCard, CardsHolder[0].transform);
        //    playerCardsUI.Add(newCard.GetComponent<CardUI>());

        //    Card card = mainPlayer.OwnedCards[i];

        //    playerCardsUI.Last().Set(cardShapeSprites[(int)card.Shape].Sprites[(int)card.Rank], card, (card) =>
        //    {
        //         AddToPassCards(newCard.GetComponent<CardUI>());
        //    });

        //    playerCardsUI.Last().SetInteractable(setInteractable);
        //}

        //AddCards(count);
        //OrganizeCards();
    }

    internal void SetCardBack(Sprite cardBack)
    {
        this.cardBack = new GameObject();
        this.cardBack.AddComponent<Image>().sprite = cardBack;
        this.cardBack.GetComponent<RectTransform>().sizeDelta = cardElementsHolder.CardsStartResolution;
    }

    public void OrganizeCards()
    {
        playerCardsUI = playerCardsUI.OrderBy(a => a.CardInfo.Shape).ThenByDescending(a => a.CardInfo.Rank).ToList();

        for (int i = 0; i < playerCardsUI.Count; i++)
        {
            playerCardsUI[i].transform.SetSiblingIndex(i);
        }

        cardElementsHolder.CardsHolder[0].SetLocations();
    }

    public void SetCardLocations()
    {
        cardElementsHolder.CardsHolder[0].SetLocations();
    }

    public virtual void CardsPlayed(int playerIndex, Card card)
    {
        //bug here
        Transform playedCard = cardElementsHolder.CardsHolder[playerIndex].transform.GetChild(
            Random.Range(0, cardElementsHolder.CardsHolder[playerIndex].transform.childCount));

        cardElementsHolder.CardsHolder[playerIndex].SetLocations();

        playedCard.SetParent(cardElementsHolder.DeckCardsPosition[playerIndex]);
        cardElementsHolder.DeckCardsPosition[playerIndex].SetAsLastSibling();

        DeckCard deckCard = new DeckCard(playedCard.gameObject, card);

        int offset = (playerIndex == 1 || playerIndex == 3) ?90:0;

        playedCard.DOLocalMove(Vector3.zero + new Vector3(Random.Range(-10,10), Random.Range(-10, 10),0), 0.5f);
        playedCard.DORotate(new Vector3(0,0,Random.Range(-40,40) + offset), 0.5f);
        playedCard.DOScaleX(0, 0.1f).OnComplete(() =>
        {
            playedCard.DOScaleX(1, 0.15f);
            deckCard.Image.sprite = cardElementsHolder.cardShapeSprites[(int)card.Shape].Sprites[(int)card.Rank];

            if (playerIndex == 1 || playerIndex == 3)
            {
                playedCard.rotation = Quaternion.identity;
            }
        });

        playedCard.GetComponent<RectTransform>().DOSizeDelta(cardElementsHolder.CardsOnTableResolution, 0.5f);

        deckCards.Add(deckCard);
    }

    public void SetPlayers(int index, PlayerBase player)
    {
        cardElementsHolder.playersDetails[index].SetPlayer(player.Avatar, player.Name, 0);
    }

    public void SetScore(int index, int score)
    {
        cardElementsHolder.playersDetails[index].SetScore(score);
    }

    public virtual void MainPlayerCard(CardUI cardUI)
    {
        cardUI.transform.SetParent(cardElementsHolder.DeckCardsPosition[0]);
        cardElementsHolder.DeckCardsPosition[0].SetAsLastSibling();

        cardUI.RectTransform.DOAnchorPos(Vector3.zero + new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), 0), 0.5f);
        cardUI.RectTransform.DOSizeDelta(cardElementsHolder.CardsOnTableResolution, 0.5f);
        cardUI.RectTransform.DORotate(new Vector3(0, 0, Random.Range(-40, 40)), 0.5f);
        cardUI.RectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        cardUI.RectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        cardUI.DisableButton();

        playerCardsUI.Remove(cardUI);

        DeckCard card = new DeckCard(cardUI.gameObject, cardUI.CardInfo);
        deckCards.Add(card);

        foreach (var item in playerCardsUI)
        {
            item.SetInteractable(false);
        }

        cardElementsHolder.CardsHolder[0].SetLocations();
    }

    public void RemoveCards(int winningHand)
    {
        Vector3[] moveDirections = new Vector3[]
        {
            new Vector3(Screen.width/2,-30f),
            new Vector3(Screen.width + 15,Screen.height/2),
            new Vector3(Screen.width/2,Screen.height + 30f),
            new Vector3(-15,Screen.height/2)
        };

        foreach (var item in deckCards)
        {
            item.Transform.DOMove(moveDirections[winningHand], 0.5f).OnComplete(() =>
            {
                Destroy(item.Transform.gameObject);
            });
        }

        deckCards.Clear();
    }

    public void AddCards(int count)
    {
        AddVerticalCards(cardElementsHolder.CardsHolder[1], cardBack,90, count);
        AddVerticalCards(cardElementsHolder.CardsHolder[3], cardBack,-90, count);
        AddHorizontalCards(cardElementsHolder.CardsHolder[2], cardBack, count);
    }

    void AddVerticalCards(PlayerCardsLayout parent, GameObject CardBack,int rotation,int count)
    {
        parent.IsVertical = true;
        for (int i = 0; i < count; i++)
        {
            GameObject newCard = Instantiate(CardBack, parent.transform);
            newCard.transform.localPosition = new Vector3(0, (i - 6) * cardElementsHolder.Spacing);
            newCard.transform.eulerAngles = new Vector3(0, 0, rotation);
        }

        parent.SetLocations();
    }

    void AddHorizontalCards(PlayerCardsLayout parent, GameObject CardBack, int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject newCard = Instantiate(CardBack, parent.transform);
            newCard.transform.localPosition = new Vector3((i - 6) * cardElementsHolder.Spacing, 0);
            newCard.transform.eulerAngles = new Vector3(0, 0, 180);
        }

        parent.SetLocations();
    }

    public void SetPlayableCards(RoundInfo info, PlayerBase player)
    {
        playableCards.Clear();

        foreach (var item in playerCardsUI)
        {
            if (CheckIfPlayable(item.CardInfo, info, player))
            {
                playableCards.Add(item);
                item.SetInteractable(true);
            }
            else
            {
                item.SetInteractable(false);
            }
        }
    }

    public void DisableAllCards()
    {
        foreach (var item in playerCardsUI)
        {
            item.SetInteractable(false);
        }
    }

    protected virtual bool CheckIfPlayable(Card card, RoundInfo trickInfo, PlayerBase player)
    {
        return false;
    }
}

public class DeckCard
{
    public Image Image;
    public Transform Transform;
    public Card Card;

    public DeckCard(GameObject gameObject, Card card)
    {
        Card = card;
        Transform = gameObject.transform;
        Image = gameObject.GetComponent<Image>();
    }
}

[System.Serializable]
public class CardShapeSprites
{
    public CardShape ShapeName;
    public Sprite[] Sprites;
}