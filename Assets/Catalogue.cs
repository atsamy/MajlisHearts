using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Catalogue : MonoBehaviour
{
    public static Catalogue Instance;

    internal Dictionary<string,List<CatalogueItem>> AllItems;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        AllItems = new Dictionary<string, List<CatalogueItem>>();

        PlayfabManager.instance.GetCatalogueData((data) =>
        {
            foreach (var item in data)
            {
                if (AllItems.ContainsKey(item.ItemClass))
                {
                    AllItems[item.ItemClass].Add(new CatalogueItem(item));
                }
                else
                {
                    AllItems.Add(item.ItemClass, new List<CatalogueItem>());
                    AllItems[item.ItemClass].Add(new CatalogueItem(item));
                }
            }

            MajlisScript.Instance.SetItems(GameManager.Instance.Customization, AllItems);
        });
        PlayfabManager.instance.GetUserData();
    }
}

[System.Serializable]
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
