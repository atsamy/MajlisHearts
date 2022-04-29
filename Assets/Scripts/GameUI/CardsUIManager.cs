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
    public GameObject CardBack;

    public Transform CardsHolder;
    public Transform passCardsHolder;
    public Transform DoubleCardHolder;

    public Transform[] DeckCardsPosition;

    List<Transform> DeckCards;

    List<CardUI> playableCards;

    List<CardUI> playerCardsUI;
    List<Card> selectedPassCards;

    [SerializeField]
    GameObject emojiPanel;
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

                cardSprites.Add(new Card(shape,rank), Resources.Load<Sprite>("Cards/" + shape + "_" + rank));
            }
        }

        DeckCards = new List<Transform>();
    }

    public void ShowPlayerCards(MainPlayer mainPlayer, bool passCards)
    {
        playerCardsUI = new List<CardUI>();
        selectedPassCards = new List<Card>();
        playableCards = new List<CardUI>();

        for (int i = 0; i < 13; i++)
        {
            GameObject newCard = Instantiate(playerCard, CardsHolder.GetChild(0));
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

    void OrganizeCards()
    {
        playerCardsUI = playerCardsUI.OrderBy(a => a.CardInfo.Shape).ThenByDescending(a => a.CardInfo.Rank).ToList();

        for (int i = 0; i < playerCardsUI.Count; i++)
        {
            playerCardsUI[i].transform.SetSiblingIndex(i);
        }

        playerCards.SetLocations();
    }

    internal void AddDoubledCard(Card card, int index)
    {
        if (card.IsTenOfDiamonds)
            playersDetails[index].ShowDouble(0);
        else if (card.IsQueenOfSpades)
            playersDetails[index].ShowDouble(1);
    }

    internal void UpdateCards(MainPlayer mainPlayer)
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

        for (int i = 0; i < 3; i++)
        {
            GameObject newCard = Instantiate(playerCard, CardsHolder.GetChild(0));
            //newCard.transform.localPosition = new Vector3((i - 6) * 100, 0);
            CardUI cardUI = newCard.GetComponent<CardUI>();
            playerCardsUI.Add(cardUI);

            Card card = mainPlayer.PassedCards[i];

            cardUI.Set(cardSprites[card], card, (card) =>
            {
                mainPlayer.ChooseCard(card);
                MainPlayerCard(cardUI);
            });

            cardUI.SetInteractable(false);
        }

        OrganizeCards();
    }

    public void AddToPassCards(CardUI cardUI)
    {
        if (passCardsHolder.childCount < 3)
        {
            playerCardsUI.Remove(cardUI);
            cardUI.transform.SetParent(passCardsHolder);
            selectedPassCards.Add(cardUI.CardInfo);
            cardUI.SetOnPressed((card) =>
            {
                ReturnToStack(cardUI);
            });
            cardUI.transform.rotation = Quaternion.identity;

            playerCards.SetLocations();
        }
    }

    public void ReturnToStack(CardUI cardUI)
    {
        playerCardsUI.Add(cardUI);
        cardUI.transform.SetParent(CardsHolder.GetChild(0));
        selectedPassCards.Remove(cardUI.CardInfo);
        cardUI.SetOnPressed((card) =>
        {
            AddToPassCards(cardUI);
        });

        OrganizeCards();
    }

    public void SelectPassCard()
    {
        if (selectedPassCards.Count == 3)
        {
            UIManager.Instance.PassCards(selectedPassCards);
        }
    }

    public void CardsPlayed(int playerIndex, Card card)
    {
        Transform playedCard = CardsHolder.GetChild(playerIndex).GetChild(Random.Range(0, CardsHolder.GetChild(playerIndex).childCount));

        
        playedCard.SetParent(DeckCardsPosition[playerIndex]);
        DeckCardsPosition[playerIndex].SetAsLastSibling();

        playedCard.GetComponent<RectTransform>().DOAnchorPos(Vector3.zero, 0.5f);

        playedCard.DOScaleX(0, 0.1f).OnComplete(()=> 
        {
            playedCard.DOScaleX(1, 0.15f);
            playedCard.GetComponent<Image>().sprite = cardSprites[card];

            if (playerIndex == 1 || playerIndex == 3)
            {
                playedCard.rotation = Quaternion.identity;
            }
        });

        DeckCards.Add(playedCard);

        RemoveDoubleIcon(card,playerIndex);
    }

    private void RemoveDoubleIcon(Card card,int index)
    {
        if (card.IsTenOfDiamonds)
            playersDetails[index].HideDouble(0);
        else if (card.IsQueenOfSpades)
            playersDetails[index].HideDouble(1);
    }

    public void SetPlayers(int index, Player player,string avatar)
    {
        playersDetails[index].SetPlayer(avatar, player.Name,0);
    }

    public void SetScore(int index, int score)
    {
        playersDetails[index].SetScore(score);
    }

    public void MainPlayerCard(CardUI cardUI)
    {
        cardUI.transform.parent = DeckCardsPosition[0];
        DeckCardsPosition[0].SetAsLastSibling();

        cardUI.RectTransform.DOAnchorPos(Vector3.zero, 0.5f);
        cardUI.RectTransform.DORotate(Vector3.zero, 0.5f);

        cardUI.RectTransform.anchorMin = new Vector2(0.5f,0.5f);
        cardUI.RectTransform.anchorMax = new Vector2(0.5f, 0.5f);

        playerCardsUI.Remove(cardUI);
        cardUI.DisableButton();

        DeckCards.Add(cardUI.transform);

        foreach (var item in playerCardsUI)
        {
            item.SetInteractable(false);
        }

        RemoveDoubleIcon(cardUI.CardInfo,0);

        playerCards.SetLocations();
    }

    public void RemoveCards(int winningHand)
    {
        Vector3[] moveDirections = new Vector3[]
        {
            Vector3.down,
            Vector3.right,
            Vector3.up,
            Vector3.left
        };

        foreach (RectTransform item in DeckCards)
        {
            //LeanTween.moveLocal(item.gameObject, moveDirections[winningHand] * 1500, 0.5f).setDestroyOnComplete(true);
            item.DOAnchorPos(moveDirections[winningHand] * 1500, 0.5f).OnComplete(() =>
            {
                Destroy(item.gameObject);
            });
        }

        DeckCards.Clear();
    }

    public void RemovePassedCards()
    {
        foreach (Transform item in passCardsHolder)
        {
            Destroy(item.gameObject);
        }
    }

    public void AddCards()
    {
        AddVerticalCards(CardsHolder.GetChild(1));
        AddVerticalCards(CardsHolder.GetChild(3));

        AddHorizontalCards(CardsHolder.GetChild(2));
    }

    void AddVerticalCards(Transform parent)
    {
        for (int i = 0; i < 13; i++)
        {
            GameObject newCard = Instantiate(CardBack, parent);
            newCard.transform.localPosition = new Vector3(0, (i - 6) * Spacing);
            newCard.transform.eulerAngles = new Vector3(0, 0, 90);
        }
    }

    void AddHorizontalCards(Transform parent)
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
            //print(item.CardInfo.Rank + " " + item.CardInfo.Shape);

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

