using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreItem : MonoBehaviour
{
    public Text Cost;
    public Text Quantity;

    public Button purchaseBtn;

    Action<int> purchaseAction;

    int index;

    public void Set(string cost, int quantity, int index, Action<int> action)
    {
        this.Cost.text = cost.ToString();
        this.Quantity.text = quantity.ToString();

        this.index = index;

        purchaseAction = action;
    }


    public void SetButton(bool value)
    {
        purchaseBtn.interactable = value;
    }

    public void Purchase()
    {
        purchaseAction.Invoke(index);
    }
}
