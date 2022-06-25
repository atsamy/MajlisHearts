using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class MajlisItem : MonoBehaviour
{
    string category;
    int index;

    CatalogueItem item;

    [SerializeField]
    GameObject locked;

    [SerializeField]
    GameObject equipped;

    [SerializeField]
    Image itemImage;

    public delegate void Pressed(CatalogueItem item);
    public Pressed OnPressed;

    public void Set(string category, int index, CatalogueItem item)
    {
        this.category = category;
        this.index = index;

        itemImage.sprite = item.Sprite;

        this.item = item;
    }

    public void Select()
    {
        OnPressed?.Invoke(item);
    }
}
