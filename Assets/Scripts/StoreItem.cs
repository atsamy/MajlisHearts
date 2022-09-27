using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StoreItem : MonoBehaviour
{
    public TextMeshProUGUI Cost;
    public MaskableGraphic ItemValue;
    public Button purchaseBtn;

    protected Action<int> purchaseAction;
    protected int index;

    public void Set(string cost, int quantity, int index, Action<int> action)
    {
        this.Cost.text = cost;
        ((Text)ItemValue).text = quantity.ToString();

        this.index = index;
        purchaseAction = action;
    }


    public void SetButton(bool value)
    {
        purchaseBtn.interactable = value;
    }

    public void Purchase()
    {
        purchaseAction?.Invoke(index);
    }
}
