using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Catalogue : MonoBehaviour
{
    Dictionary<string,List<CatalogueItem>> allItems;


    private void Start()
    {
        PlayfabManager.instance.GetCatalogueData((data) =>
        {
            foreach (var item in data)
            {
                if (allItems.ContainsKey(item.ItemClass))
                {
                    allItems[item.ItemClass].Add(new CatalogueItem(item));
                }
                else
                {
                    allItems.Add(item.ItemClass, new List<CatalogueItem>());
                    allItems[item.ItemClass].Add(new CatalogueItem(item));
                }
            }
        });
    }
}

//[System.Serializable]
//public class PurchaseCategory
//{
//    public string Code;
//    public PurchasableItem[] Items;
//}

[System.Serializable]
public class CatalogueItem
{
    public string ID;
    public int Price;
    public int Level;

    public Sprite Sprite;
    GameObject model;

    public CatalogueItem(PlayFab.ClientModels.CatalogItem item)
    {
        ID = item.ItemId;
        Price = (int)item.VirtualCurrencyPrices["SC"];
        Level = int.Parse(item.CustomData);

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
