using UnityEngine;
using System;

[Serializable]
public class CatalogueItem
{
    public string ID;
    public int Price;
    public int Level;
    public string ItemClass;
    public string Name;

    string description;

    public Sprite Sprite;
    GameObject model;

    public bool IsDefault { get => (description == "default"); }
    public bool IsCustomization { get; private set; }

    public CatalogueItem(PlayFab.ClientModels.CatalogItem item)
    {
        ID = item.ItemId;
        Price = (int)item.VirtualCurrencyPrices["SC"];
        int.TryParse(item.CustomData,out Level);
        ItemClass = item.ItemClass;

        Name = item.DisplayName;
        description = item.Description;

        Sprite = Resources.Load<Sprite>("Sprites/" + ID);
        IsCustomization = (description == "customization");
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