using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CardsUIManager : MonoBehaviour
{
    public GameObject playerCard;
    public GameObject CardBack;

    public Transform CardsHolder;
    public Transform passCardsHolder;
    public Transform DoubleCardHolder;

    public Transform[] DeckCardsPosition;

    List<Transform> DeckCards;

    List<CardUI> playableCards;
    //Sprite[] cardSprites;

    List<CardUI> playerCardsUI;
    List<Card> selectedPassCards;

    GameObject TenOfDiamondIcon;
    GameObject QueenOfSpadeIcon;

    public Text[] Scores;

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

        
        //cardSprites = Resources.LoadAll<Sprite>("Cards/classic-playing-cards");

        for (int i = 0; i < 13; i++)
        {
            GameObject newCard = Instantiate(playerCard, CardsHolder.GetChild(0));
            playerCardsUI.Add(newCard.GetComponent<CardUI>());

            Card card = mainPlayer.OwnedCards[i];

            //Sprite sprite = Resources.Load<Sprite>("Cards/" + card.Shape + "_" + card.Rank);

            playerCardsUI.Last().Set(cardSprites[card], card, (card) =>
             {
                 //if (passCards)
                 AddToPassCards(newCard.GetComponent<CardUI>());
                 //else
                 //    mainPlayer.ChooseCard(card);
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
    }

    internal void AddDoubledCard(Card card, int index)
    {
        GameObject doubleCard = new GameObject();
        doubleCard.AddComponent<Image>().sprite = cardSprites[card];
        //GameObject newCard = Instantiate(playerCard, DoubleCardHolder.GetChild(index));
        //newCard.GetComponent<Image>().sprite = Resources.Load<Sprite>("Cards/" + card.Shape + "_" + card.Rank);
        doubleCard.transform.parent = DoubleCardHolder.GetChild(index);

        if (card.IsTenOfDiamonds)
            TenOfDiamondIcon = doubleCard;
        else if (card.IsQueenOfSpades)
            QueenOfSpadeIcon = doubleCard;
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
        Transform playedCard = (CardsHolder.GetChild(playerIndex).GetChild(Random.Range(0, CardsHolder.GetChild(playerIndex).childCount)));

        //Sprite sprite = Resources.Load<Sprite>("Cards/" + card.Shape + "_" + card.Rank);
        playedCard.GetComponent<Image>().sprite = cardSprites[card];

        playedCard.parent = DeckCardsPosition[playerIndex];
        //playedCard.localPosition = Vector3.zero;
        playedCard.GetComponent<RectTransform>().DOAnchorPos(Vector3.zero, 0.5f);

        DeckCards.Add(playedCard);

        RemoveDoubleIcon(card);
    }

    private void RemoveDoubleIcon(Card card)
    {
        if (card.IsQueenOfSpades && QueenOfSpadeIcon != null)
        {
            Destroy(QueenOfSpadeIcon);
        }
        else if (card.IsTenOfDiamonds && TenOfDiamondIcon != null)
        {
            Destroy(TenOfDiamondIcon);
        }
    }

    public void SetScore(int index, Player player)
    {
        Scores[index].text = player.Name + " " + player.Score.ToString();
    }

    public void MainPlayerCard(CardUI cardUI)
    {
        //CardUI cardUI = playerCardsUI.Find(a => a.CardInfo == card);
        cardUI.transform.parent = DeckCardsPosition[0];
        //cardUI.transform.parent = null;
        cardUI.GetComponent<RectTransform>().DOAnchorPos(Vector3.zero, 0.5f);

        playerCardsUI.Remove(cardUI);
        cardUI.DisableButton();

        DeckCards.Add(cardUI.transform);

        foreach (var item in playerCardsUI)
        {
            item.SetInteractable(false);
        }

        RemoveDoubleIcon(cardUI.CardInfo);
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
            newCard.transform.localPosition = new Vector3(0, (i - 6) * 60);
            newCard.transform.eulerAngles = new Vector3(0, 0, 90);
        }
    }

    void AddHorizontalCards(Transform parent)
    {
        for (int i = 0; i < 13; i++)
        {
            GameObject newCard = Instantiate(CardBack, parent);
            newCard.transform.localPosition = new Vector3((i - 6) * 60, 0);
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

