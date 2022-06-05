using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class StoreScene : MenuScene
{
    public TabScript CoinsTab;
    public TabScript ContentTab;

    public GameObject CoinsContent;
    public GameObject ContentContent;
    public Transform CardsContent;
    public StoreItem[] CurrencyStoreItems;
    public int[] SCValues;
    public GameObject ContentStoreItem;

    bool contentLoaded;

    public GameObject Loading;
    Dictionary<string, ContentStoreItem> equippedCatalogueItem;
    List<ContentStoreItem> initContent;
    void Start()
    {
        CoinsTab.Pressed(true);
        ContentTab.Pressed(false);
    }

    public override void Open()
    {
        base.Open();

        Purchaser.Instance.GetAllPrices((prices) =>
        {
            for (int i = 0; i < CurrencyStoreItems.Length; i++)
            {
                CurrencyStoreItems[i].Set(prices[i], SCValues[i], i, (index) =>
                {
                    PlayfabManager.instance.AddCurrency(SCValues[index], (result) =>
                     {
                         print("add currency: " + result);
                     });
                    GameManager.Instance.AddCurrency(SCValues[index]);
                    //SFXManager.Instance.PlayClip("Buy");
                });
            }
            Loading.SetActive(false);
        });
    }

    public void CoinsPressed()
    {
        CoinsTab.Pressed(true);
        ContentTab.Pressed(false);

        CoinsContent.SetActive(true);
        ContentContent.SetActive(false);
    }

    public void ContentPressed()
    {
        CoinsTab.Pressed(false);
        ContentTab.Pressed(true);

        CoinsContent.SetActive(false);
        ContentContent.SetActive(true);

        if (!contentLoaded)
        {
            equippedCatalogueItem = new Dictionary<string, ContentStoreItem>();
            LoadCategory("CardBack");
            LoadCategory("TableTop");
            contentLoaded = true;
        }
        else
        {
            SetContentButtons();
        }
    }

    private void LoadCategory(string category)
    {
        List<CatalogueItem> catalogueItems = GameManager.Instance.Catalog.First(a => a.Key == category).Value;
        initContent = new List<ContentStoreItem>();

        for (int i = 0; i < catalogueItems.Count; i++)
        {
            ContentStoreItem storeItem = Instantiate(ContentStoreItem, CardsContent).GetComponent<ContentStoreItem>();
            bool equipped = false;

            if (GameManager.Instance.EquippedItem.ContainsKey(category))
                equipped = (GameManager.Instance.EquippedItem[category] == catalogueItems[i].ID);

            bool owned = GameManager.Instance.Inventory.Any(a => a.ID == catalogueItems[i].ID);

            if (equipped)
                equippedCatalogueItem.Add(category, storeItem);

            storeItem.Set(catalogueItems[i].Price, Resources.Load<Sprite>(category + "/" + catalogueItems[i].ID), i, owned, equipped, (index) =>
            {
                MenuManager.Instance.Popup.ShowWithMessage("are you sure you want to buy this item",()=> 
                {
                    PlayfabManager.instance.AddItemToInventory(catalogueItems[index]);
                    GameManager.Instance.DeductCurrency(catalogueItems[index].Price);
                    SetContentButtons();
                    GameManager.Instance.Inventory.Add(new InventoryItem(catalogueItems[index].ItemClass, catalogueItems[index].ID));
                    EquibNewItem(category, storeItem);
                });

            }, (index) =>
            {
                GameManager.Instance.EquipItem(category, catalogueItems[index].ID);
                EquibNewItem(category, storeItem);
            });

            if (!equipped && !owned)
                storeItem.SetButton(GameManager.Instance.Currency >= catalogueItems[i].Price);

            initContent.Add(storeItem);
        }
    }

    private void EquibNewItem(string category, ContentStoreItem storeItem)
    {
        if (equippedCatalogueItem.ContainsKey(category))
        {
            equippedCatalogueItem[category].Owned();
            equippedCatalogueItem[category] = storeItem;
        }
        else
        {
            equippedCatalogueItem.Add(category, storeItem);
        }

        storeItem.Equiped();
    }

    private void SetContentButtons()
    {
        foreach (var item in initContent)
        {
            item.SetButton(GameManager.Instance.Currency >= item.Price);
        }
    }
}

