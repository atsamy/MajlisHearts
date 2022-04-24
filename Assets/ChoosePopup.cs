using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoosePopup : MonoBehaviour
{
    Action<int> choice;
    public void Show(Action<int> Choice)
    {
        gameObject.SetActive(true);
        choice = Choice;
    }

    public void Select(int value)
    {
        gameObject.SetActive(false);
        choice?.Invoke(value);
    }
}
