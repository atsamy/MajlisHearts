using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using DG.Tweening;
public class PassCardsPanel : MonoBehaviour
{
    [SerializeField]
    Transform[] cardHolder;
    [SerializeField]
    Button passButton;

    Action<List<Card>> passAction;
    List<Card> selectedPassCards;

    List<GameObject> cardsObject;
    public bool AddCard(CardUI card)
    {
        bool ready = true;
        bool cardAdded = false;

        cardsObject.Add(card.gameObject);

        foreach (var item in cardHolder)
        {
            if (item.childCount == 0 && !cardAdded)
            {
                selectedPassCards.Add(card.CardInfo);
                card.transform.SetParent(item);
                card.transform.DOLocalMove(Vector3.zero, 0.25f);
                cardAdded = true;
            }
            else if (item.childCount == 0)
            {
                ready = false;
            }
        }

        passButton.interactable = ready;
        return cardAdded;
    }

    public void Show(Action<List<Card>> OnPass)
    {
        cardsObject = new List<GameObject>();

        selectedPassCards = new List<Card>();

        passAction = OnPass;
        gameObject.SetActive(true);
    }

    public void RemoveCard(CardUI card)
    {
        cardsObject.Remove(card.gameObject);
        selectedPassCards.Remove(card.CardInfo);
        passButton.interactable = false;
    }

    public void SwapPressed()
    {
        gameObject.SetActive(false);
        passAction?.Invoke(selectedPassCards);

        foreach (var item in cardsObject)
        {
            Destroy(item);
        }

        cardsObject.Clear();
    }
}
