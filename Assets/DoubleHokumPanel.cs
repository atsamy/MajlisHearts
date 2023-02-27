using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DoubleHokumPanel : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI doubleText;

    public event Action<bool,int> OnDoublePressed;
    int value;
    public void Show(int value)
    {
        gameObject.SetActive(true);

        switch (value)
        {
            case 0:
                doubleText.text = "double";
                break;
            case 1:
                doubleText.text = "triple";
                break;
            case 2:
                doubleText.text = "quadruple";
                break;
            case 3:
                doubleText.text = "Qahwa";
                break;
            default:
                break;
        }

        this.value = value;
    }

    public void DoublePressed() 
    {
        gameObject.SetActive(false);
        OnDoublePressed?.Invoke(true, value);
    }
    public void PassPressed()
    {
        gameObject.SetActive(false);
        OnDoublePressed?.Invoke(false, value);
    }
}
