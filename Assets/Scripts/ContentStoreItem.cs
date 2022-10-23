using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ContentStoreItem : StoreItem
{
    [HideInInspector]
    public int Price;
    [SerializeField]
    TextMeshProUGUI itemName;
    [SerializeField]
    GameObject costParent;
    [SerializeField]
    TextMeshProUGUI costText;

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
            Cost.gameObject.SetActive(false);
            costParent.SetActive(true);
            costText.text = cost.ToString();
            //this.Cost.text = cost.ToString();
            purchaseAction = buyAction;
        }

        this.equipAction = equipAction;
    }

    public void Owned()
    {
        Cost.gameObject.SetActive(true);
        purchaseAction = equipAction;
        purchaseBtn.interactable = true;
        Cost.text = LanguageManager.Instance.GetString("owned");
        costParent.SetActive(false);
    }

    public void Equiped()
    {
        Cost.gameObject.SetActive(true);
        Cost.text = LanguageManager.Instance.GetString("equipped");
        purchaseBtn.interactable = false;
        costParent.SetActive(false);
    }
}
