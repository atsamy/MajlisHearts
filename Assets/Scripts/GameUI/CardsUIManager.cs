using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CardsUIManager : MonoBehaviour
{
    public int Spacing = 40;
    public GameObject playerCard;

    GameObject cardBack;

    public Transform[] CardsHolder;
    //[SerializeField]
    //PassCardsPanel passCardsPanel;

    //public Transform DoubleCardHolder;

    public Transform[] DeckCardsPosition;

    List<DeckCard> deckCards;
    List<CardUI> playableCards;
    List<CardUI> playerCardsUI;

    [SerializeField]
    PlayerDetails[] playersDetails;
    [SerializeField]
    PlayerCardsLayout playerCards;

    Dictionary<Card, Sprite> cardSprites;

    public void SetMainPlayer(MainPlayer mainPlayer)
    {
        mainPlayer.OnForcePlay += () => { playableCards[Random.Range(0, playableCards.Count)].Pressed(); };
    }

    private void Awake()
    {
        cardSprites = new Dictionary<Card, Sprite>();


        for (int i = 0; i < 13; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                CardShape shape = (CardShape)j;
                CardRank rank = (CardRank)i;

                cardSprites.Add(new Card(shape, rank), Resources.Load<Sprite>("Cards/" + shape + "_" + rank));
            }
        }

        deckCards = new List<DeckCard>();
    }

    public void ShowPlayerCards(MainPlayer mainPlayer, bool passCards)
    {
        playerCardsUI = new List<CardUI>();
        playableCards = new List<CardUI>();

        for (int i = 0; i < 13; i++)
        {
            GameObject newCard = Instantiate(playerCard, CardsHolder[0]);
            playerCardsUI.Add(newCard.GetComponent<CardUI>());

            Card card = mainPlayer.OwnedCards[i];

            playerCardsUI.Last().Set(cardSprites[card], card, (card) =>
            {
                 AddToPassCards(newCard.GetComponent<CardUI>());
            });

            playerCardsUI.Last().SetInteractable(passCards);
        }

        AddCards();
        OrganizeCards();
    }

    internal void SetCardBack(Sprite cardBack)
    {
        this.cardBack = new GameObject();
        this.cardBack.AddComponent<Image>().sprite = cardBack;
        this.cardBack.GetComponent<RectTransform>().sizeDelta = new Vector2(140, 186.7f);
    }

    public void OrganizeCards()
    {
        playerCardsUI = playerCardsUI.OrderBy(a => a.CardInfo.Shape).ThenByDescending(a => a.CardInfo.Rank).ToList();

        for (int i = 0; i < playerCardsUI.Count; i++)
        {
            playerCardsUI[i].transform.SetSiblingIndex(i);
        }

        playerCards.SetLocations();
    }

    public void SetCardLocations()
    {
        playerCards.SetLocations();
    }

    internal void AddDoubledCard(Card card, int index)
    {
        if (card.IsTenOfDiamonds)
            playersDetails[index].ShowDouble(0);
        else if (card.IsQueenOfSpades)
            playersDetails[index].ShowDouble(1);
    }

    internal IEnumerator UpdateCards(MainPlayer mainPlayer)
    {
        foreach (var item in playerCardsUI)
        {
            item.SetOnPressed((card) =>
            {
                mainPlayer.ChooseCard(card);
                MainPlayerCard(item);
            });

            //Debug.Log(item.CardInfo);

            //bug here
            item.SetInteractable(false);
        }

        for (int i = 0; i < 3; i++)
        {
            GameObject newCard = Instantiate(playerCard, CardsHolder[0]);
            newCard.transform.localPosition = new Vector3(i * 150 - 150,300,0);
            CardUI cardUI = newCard.GetComponent<CardUI>();
            playerCardsUI.Add(cardUI);

            Card card = mainPlayer.PassedCards[i];
            cardUI.Set(cardSprites[card], card, (card) =>
            {
                mainPlayer.ChooseCard(card);
                MainPlayerCard(cardUI);
            });

            //Debug.Log(card);
            //bug here
            cardUI.SetInteractable(false);
        }
        yield return new WaitForSeconds(1.7f);
        OrganizeCards();
    }

    public void AddToPassCards(CardUI cardUI)
    {
        if (UIManager.Instance.AddCard(cardUI))
        {
            playerCardsUI.Remove(cardUI);
            cardUI.SetOnPressed((card) =>
            {
                ReturnToStack(cardUI);
            });

            cardUI.transform.rotation = Quaternion.identity;
            playerCards.SetLocations();
        }
    }

    //void ShowWinningCard()
    //{
    //    int winningIndex = 0;

    //    for (int i = 1; i < deckCards.Count; i++)
    //    {
    //        if (deckCards[i].Card.Shape == deckCards[0].Card.Shape && deckCards[i].Card.Rank > deckCards[winningIndex].Card.Rank)
    //            winningIndex = i;
    //    }

    //    for (int i = 0; i < deckCards.Count; i++)
    //    {
    //        if(i != winningIndex)
    //            deckCards[i].Image.DOColor(Color.gray,0.1f);
    //    }

    //    //deckCards[winningIndex].Transform.SetAsLastSibling();
    //}

    public void ReturnToStack(CardUI cardUI)
    {
        playerCardsUI.Add(cardUI);
        cardUI.transform.SetParent(CardsHolder[0]);
        //selectedPassCards.Remove(cardUI.CardInfo);
        UIManager.Instance.RemoveCard(cardUI);

        cardUI.SetOnPressed((card) =>
        {
            AddToPassCards(cardUI);
        });

        OrganizeCards();
    }

    public void CardsPlayed(int playerIndex, Card card)
    {
        //bug here
        Transform playedCard = CardsHolder[playerIndex].GetChild(Random.Range(0, CardsHolder[playerIndex].childCount));

        playedCard.SetParent(DeckCardsPosition[playerIndex]);
        DeckCardsPosition[playerIndex].SetAsLastSibling();

        DeckCard deckCard = new DeckCard(playedCard.gameObject, card);

        playedCard.DOLocalMove(Vector3.zero, 0.5f);
        //.OnComplete(()=>
        //{
        //    if (deckCards.Count > 1)
        //    {
        //        ShowWinningCard();
        //    }
        //});

        playedCard.DOScaleX(0, 0.1f).OnComplete(() =>
        {
            playedCard.DOScaleX(1, 0.15f);
            deckCard.Image.sprite = cardSprites[card];

            if (playerIndex == 1 || playerIndex == 3)
            {
                playedCard.rotation = Quaternion.identity;
            }
        });

        deckCards.Add(deckCard);
        RemoveDoubleIcon(card, playerIndex);
    }

    private void RemoveDoubleIcon(Card card, int index)
    {
        if (card.IsTenOfDiamonds)
            playersDetails[index].HideDouble(0);
        else if (card.IsQueenOfSpades)
            playersDetails[index].HideDouble(1);
    }

    public void SetPlayers(int index, Player player)
    {
        playersDetails[index].SetPlayer(player.Avatar, player.Name, 0);
    }

    public void SetScore(int index, int score)
    {
        playersDetails[index].SetScore(score);
    }

    public void MainPlayerCard(CardUI cardUI)
    {
        cardUI.transform.SetParent(DeckCardsPosition[0]);
        DeckCardsPosition[0].SetAsLastSibling();

        cardUI.RectTransform.DOAnchorPos(Vector3.zero, 0.5f);
        //    .OnComplete(() =>
        //{
        //    if (deckCards.Count > 1)
        //    {
        //        ShowWinningCard();
        //    }
        //});

        cardUI.RectTransform.DORotate(Vector3.zero, 0.5f);
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

        RemoveDoubleIcon(cardUI.CardInfo, 0);
        playerCards.SetLocations();
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

    public void AddCards()
    {
        AddVerticalCards(CardsHolder[1], cardBack);
        AddVerticalCards(CardsHolder[3], cardBack);
        AddHorizontalCards(CardsHolder[2], cardBack);
    }

    void AddVerticalCards(Transform parent, GameObject CardBack)
    {
        for (int i = 0; i < 13; i++)
        {
            GameObject newCard = Instantiate(CardBack, parent);
            newCard.transform.localPosition = new Vector3(0, (i - 6) * Spacing);
            newCard.transform.eulerAngles = new Vector3(0, 0, 90);
        }
    }

    void AddHorizontalCards(Transform parent, GameObject CardBack)
    {
        for (int i = 0; i < 13; i++)
        {
            GameObject newCard = Instantiate(CardBack, parent);
            newCard.transform.localPosition = new Vector3((i - 6) * Spacing, 0);
        }
    }

    public void SetPlayableCards(DealInfo info, Player player)
    {
        playableCards.Clear();

        foreach (var item in playerCardsUI)
        {
            if (checkIfPlayable(item.CardInfo, info, player))
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

    bool checkIfPlayable(Card card, DealInfo trickInfo, Player player)
    {
        bool firstHand = trickInfo.CardsOntable.Count == 0;

        if (trickInfo.roundNumber == 0 && firstHand && !(card.Shape == CardShape.Club && card.Rank == CardRank.Two))
            return false;
        if (trickInfo.TrickShape != card.Shape && player.HasShape(trickInfo.TrickShape) && !firstHand)
            return false;
        if (trickInfo.roundNumber == 0)
        {
            if (card.Shape == CardShape.Heart)
                return false;
            if (card.IsQueenOfSpades || card.IsTenOfDiamonds)
                return false;
        }
        if (!trickInfo.heartBroken && card.Shape == CardShape.Heart && firstHand && !player.HasOnlyHearts())
            return false;

        return true;
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