using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ContentStoreItem : StoreItem
{
    [HideInInspector]
    public int Price;

    [SerializeField]
    Text itemName;

    Action<int> equipAction;
    public void Set(int cost,string itemName, Sprite itemSprite, int index,bool owned,bool equipped, Action<int> buyAction, Action<int> equipAction)
    {
        ((Image)ItemValue).sprite = itemSprite;
        this.index = index;
        this.itemName.text = itemName;

        if (equipped)
        {
            Equiped();
        }
        else if (owned || cost == 0)
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
        Cost.text = LanguageManager.Instance.GetString("owned");
    }

    public void Equiped()
    {
        Cost.text = LanguageManager.Instance.GetString("equipped");
        purchaseBtn.interactable = false;
    }
}
