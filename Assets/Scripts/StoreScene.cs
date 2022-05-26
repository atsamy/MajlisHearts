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
    ContentStoreItem equippedCardBack;
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
            List<CatalogueItem> CardBack = GameManager.Instance.Catalog.First(a => a.Key == "CardBack").Value;
            initContent = new List<ContentStoreItem>();

            for (int i = 0; i < CardBack.Count; i++)
            {
                ContentStoreItem storeItem = Instantiate(ContentStoreItem, CardsContent).GetComponent<ContentStoreItem>();
                bool equipped = false;

                if (GameManager.Instance.EquippedItem.ContainsKey("CardBack"))
                    equipped = (GameManager.Instance.EquippedItem["CardBack"] == CardBack[i].ID);

                bool owned = GameManager.Instance.Inventory.Any(a => a.ID == CardBack[i].ID);

                if (equipped)
                    equippedCardBack = storeItem;

                storeItem.Set(CardBack[i].Price, Resources.Load<Sprite>("CardBack/" + CardBack[i].ID), i, owned, equipped, (index) =>
                   {
                       PlayfabManager.instance.AddItemToInventory(CardBack[index]);
                       GameManager.Instance.DeductCurrency(CardBack[index].Price);
                       SetContentButtons();
                       GameManager.Instance.Inventory.Add(new InventoryItem(CardBack[index].ItemClass, CardBack[index].ID));

                       equippedCardBack?.Owned();
                       equippedCardBack = storeItem;
                       storeItem.Equiped();

                   }, (index) =>
                  {
                       GameManager.Instance.EquipItem("CardBack", CardBack[index].ID);

                       equippedCardBack?.Owned();
                       equippedCardBack = storeItem;
                       storeItem.Equiped();
                   });

                if (!equipped && !owned)
                    storeItem.SetButton(GameManager.Instance.Currency >= CardBack[i].Price);

                initContent.Add(storeItem);
            }

            contentLoaded = true;
        }
        else
        {
            SetContentButtons();
        }
    }

    void UnEquipCardBack()
    {

    }

    private void SetContentButtons()
    {
        foreach (var item in initContent)
        {
            item.SetButton(GameManager.Instance.Currency >= item.Price);
        }
    }
}

