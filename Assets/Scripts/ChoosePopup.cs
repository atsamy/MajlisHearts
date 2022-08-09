using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoosePopup : MonoBehaviour
{
    [SerializeField]
    Text messageText;

    Action<int> choice;
    public void Show(string message,Action<int> Choice)
    {
        gameObject.SetActive(true);
        choice = Choice;

        messageText.text = message;
    }

    public void Select(int value)
    {
        gameObject.SetActive(false);
        choice?.Invoke(value);
    }

    public void Close()
    {
        gameObject.SetActive(false);
        MenuManager.Instance.OpenMain();
    }
}
