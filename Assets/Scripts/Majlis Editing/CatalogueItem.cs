using UnityEngine;
using System;

[Serializable]
public class CatalogueItem
{
    public string ID;
    public int Price;
    public int Level;
    public string ItemClass;

    public Sprite Sprite;
    GameObject model;

    public CatalogueItem(PlayFab.ClientModels.CatalogItem item)
    {
        ID = item.ItemId;
        Price = (int)item.VirtualCurrencyPrices["SC"];
        Level = int.Parse(item.CustomData);
        ItemClass = item.ItemClass;

        Sprite = Resources.Load<Sprite>("Sprites/" + ID);
    }

    public GameObject GetModel()
    {
        if (model != null)
        {
            return model;
        }
        else
        {
            model = Resources.Load<GameObject>("Models/" + ID);
            return model;
        }
    }
}