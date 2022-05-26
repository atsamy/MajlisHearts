using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContentStoreItem : StoreItem
{
    [HideInInspector]
    public int Price;

    Action<int> equipAction;
    public void Set(int cost, Sprite itemSprite, int index,bool owned,bool equipped, Action<int> buyAction, Action<int> equipAction)
    {
        ((Image)ItemValue).sprite = itemSprite;
        this.index = index;

        if (equipped)
        {
            Equiped();
        }
        else if (owned)
        {
            Owned();
            purchaseAction = equipAction;
        }
        else
        {
            Price = cost;
            this.Cost.text = cost.ToString();
            purchaseAction = buyAction;
        }

        this.equipAction = equipAction;
    }

    public void Owned()
    {
        purchaseAction = equipAction;
        purchaseBtn.interactable = true;
        Cost.text = "Owned";
    }

    public void Equiped()
    {
        Cost.text = "Equipped";
        purchaseBtn.interactable = false;
    }
}
