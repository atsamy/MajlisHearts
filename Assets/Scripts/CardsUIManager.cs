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
    public Transform DeckCards;
    public Transform passCardsHolder;

    //Sprite[] cardSprites;

    List<CardUI> playerCardsUI;
    List<Card> selectedPassCards;

    public Text[] Scores;

    public void ShowPlayerCards(MainPlayer mainPlayer,bool passCards)
    {
        playerCardsUI = new List<CardUI>();
        selectedPassCards = new List<Card>();

        //cardSprites = Resources.LoadAll<Sprite>("Cards/classic-playing-cards");

        for (int i = 0; i < 13; i++)
        {
            GameObject newCard = Instantiate(playerCard, CardsHolder.GetChild(0));
            playerCardsUI.Add(newCard.GetComponent<CardUI>());

            Card card = mainPlayer.OwnedCards[i];

            Sprite sprite = Resources.Load<Sprite>("Cards/" + card.Shape + "_"+ card.Rank);

            playerCardsUI.Last().Set(sprite, card, (card) =>
             {
                 if (passCards)
                     AddToPassCards(newCard.GetComponent<CardUI>());
                 else
                    mainPlayer.ChooseCard(card);
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

    internal void UpdateCards(MainPlayer mainPlayer)
    {
        foreach (var item in playerCardsUI)
        {
            item.SetOnPressed((card) =>
            {
                mainPlayer.ChooseCard(card);
            });

            item.SetInteractable(false);
        }

        for (int i = 0; i < 3; i++)
        {
            GameObject newCard = Instantiate(playerCard, CardsHolder.GetChild(0));
            //newCard.transform.localPosition = new Vector3((i - 6) * 100, 0);

            playerCardsUI.Add(newCard.GetComponent<CardUI>());

            Card card = mainPlayer.PassedCards[i];

            Sprite sprite = Resources.Load<Sprite>("Cards/" + card.Shape + "_" + card.Rank);

            playerCardsUI.Last().Set(sprite, card, (card) =>
            {
                mainPlayer.ChooseCard(card);
            });

            playerCardsUI.Last().SetInteractable(false);
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
            cardUI.SetOnPressed((card)=>
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

        Sprite sprite = Resources.Load<Sprite>("Cards/" + card.Shape + "_" + card.Rank);
        playedCard.GetComponent<Image>().sprite = sprite;

        playedCard.parent = DeckCards.GetChild(playerIndex);
        //playedCard.localPosition = Vector3.zero;
        playedCard.DOLocalMove(Vector3.zero,0.5f);

    }

    public void SetScores(Player[] scores)
    {
        for (int i = 0; i < Scores.Length; i++)
        {
            Scores[i].text = scores[i].Score.ToString();
        }
    }

    public void ResetScores()
    {
        for (int i = 0; i < Scores.Length; i++)
        {
            Scores[i].text = "0";
        }
    }

    public void MainPlayerCard(Card card)
    {
        CardUI cardUI = playerCardsUI.Find(a => a.CardInfo == card);
        cardUI.transform.parent = DeckCards.GetChild(0);
        cardUI.transform.DOLocalMove(Vector3.zero, 0.5f);

        playerCardsUI.Remove(cardUI);
        Destroy(cardUI.GetComponent<Button>());

        foreach (var item in playerCardsUI)
        {
            item.SetInteractable(false);
        }
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

        foreach (Transform item in DeckCards)
        {
            item.GetChild(0).DOMove(item.GetChild(0).position + moveDirections[winningHand] * 1500, 0.5f).OnComplete(() =>
            {
                Destroy(item.GetChild(0).gameObject);
            });
        }
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
            newCard.transform.localPosition = new Vector3(0,(i - 6) * 60 );
            newCard.transform.eulerAngles = new Vector3(0,0,90);
        }
    }

    void AddHorizontalCards(Transform parent)
    {
        for (int i = 0; i < 13; i++)
        {
            GameObject newCard = Instantiate(CardBack, parent);
            newCard.transform.localPosition = new Vector3((i - 6) * 60,0);
        }
    }

    public void SetPlayableCards(DealInfo info, Player player,bool firstHand)
    {
        foreach (var item in playerCardsUI)
        {
            print(item.CardInfo.Rank + " " + item.CardInfo.Shape);
            item.SetInteractable(checkIfPlayable(item.CardInfo, info,player, firstHand));
        }
    }

    public void DisableAllCards()
    {
        foreach (var item in playerCardsUI)
        {
            item.SetInteractable(false);
        }
    }

    bool checkIfPlayable(Card card, DealInfo trickInfo, Player player,bool firstHand)
    {
        if (trickInfo.roundNumber == 0 && firstHand && !(card.Shape == CardShape.Club && card.Rank == CardRank.Two))
            return false;
        if (trickInfo.TrickShape != card.Shape && player.HasShape(trickInfo.TrickShape) && !firstHand)
            return false;
        if (trickInfo.roundNumber == 0)
        {
            if (card.Shape == CardShape.Heart)
                return false;
            if (card.Shape == CardShape.Spade && card.Rank == CardRank.Queen)
                return false;
        }
        if (!trickInfo.heartBroken && card.Shape == CardShape.Heart && firstHand)
            return false;

        return true;
    }
}

