using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class MajlisItem : MonoBehaviour
{
    string category;
    int index;

    PurchasableItem item;

    public delegate void Pressed(PurchasableItem item, string category,int index);
    public Pressed OnPressed;

    public void Set(string category, int index,PurchasableItem item)
    {
        this.category = category;
        this.index = index;

        GetComponent<Image>().sprite = item.Sprite;

        this.item = item;
    }

    public void Select()
    {
        OnPressed?.Invoke(item,category, index);
    }
}
