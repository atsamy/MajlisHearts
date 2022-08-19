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
    public RectTransform RectTransform;

    bool drag;
    bool dragged;
    Vector3 offSet;
    Vector3 originalPosition;
    // Start is called before the first frame update
    void Awake()
    {
        image = GetComponent<Image>();
        button = GetComponent<Button>();

        RectTransform = GetComponent<RectTransform>();
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
        //OnPressed?.Invoke(CardInfo);
    }

    public void PointerDown()
    {
        if (!button.interactable)
            return;

        offSet = transform.position - Input.mousePosition;
        originalPosition = transform.position;
        drag = true;
        StartCoroutine(Drag());
    }

    IEnumerator Drag()
    {
        while (drag)
        {
            transform.position = Input.mousePosition + offSet;

            if (Vector3.Distance(originalPosition, Input.mousePosition + offSet) > 5 && !dragged)
            {
                transform.rotation = Quaternion.identity;
                dragged = true;
            }

            yield return null;
        }
    }

    public void PointerUp()
    {
        if (!button.interactable)
            return;

        drag = false;

        if ((transform.localPosition.y > 50 && dragged) || !dragged)
        {
            OnPressed?.Invoke(CardInfo);
        }
        else
        {
            UIManager.Instance.SetCardLocations();
        }
        dragged = false;
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
