using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    public Card CardInfo;
    Image image;
    Button button;

    public Action<Card> OnPressed;
    // Start is called before the first frame update
    void Awake()
    {
        image = GetComponent<Image>();
        button = GetComponent<Button>();
    }

    public void Set(Sprite sprite, Card card, Action<Card> Pressed)
    {
        CardInfo = card;
        image.sprite = sprite;
        OnPressed = Pressed;
    }

    public void SetOnPressed(Action<Card> Pressed)
    {
        OnPressed = Pressed;
    }

    public void Pressed()
    {
        OnPressed?.Invoke(CardInfo);
    }

    public void SetInteractable(bool value)
    {
        button.interactable = value;
    }

    public void DisableButton()
    {
        button.enabled = false;
    }
}
