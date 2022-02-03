using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    Card cardInfo;
    Image image;
    Button button;

    public Action<Card> OnPressed;
    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
        button = GetComponent<Button>();
    }

    public void Set(Card card,Action<Card> Pressed)
    {
        cardInfo = card;

        image.sprite = Resources.Load<Sprite>("Cards/" + card.Shape + card.Rank);

        OnPressed = Pressed;
    }

    public void Pressed()
    {
        OnPressed?.Invoke(cardInfo);
        //move to position
    }

    public void SetInteractable(bool value)
    {
        button.interactable = value;
    }
}
