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
    public bool AddCard(CardUI card)
    {
        bool ready = true;
        bool cardAdded = false;

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
        selectedPassCards = new List<Card>();

        passAction = OnPass;
        gameObject.SetActive(true);
    }

    public void RemoveCard(CardUI card)
    {
        selectedPassCards.Remove(card.CardInfo);
        passButton.interactable = false;
    }

    public void SwapPressed()
    {
        //foreach (var item in cardHolder)
        //{
        //    Destroy(item.GetChild(0));
        //}

        gameObject.SetActive(false);
        passAction?.Invoke(selectedPassCards);
    }
}
