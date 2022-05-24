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

    List<StoreItem> initContent;
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
                    PlayfabManager.instance.AddCurrency(SCValues[index],(result)=> 
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
            initContent = new List<StoreItem>();

            for (int i = 0; i < CardBack.Count; i++)
            {
                StoreItem storeItem = Instantiate(ContentStoreItem, CardsContent).GetComponent<StoreItem>();
                storeItem.Set(CardBack[i].Price, Resources.Load<Sprite>("CardBack/" + CardBack[i].ID), i, (index) =>
                 {
                     PlayfabManager.instance.AddItemToInventory(CardBack[index]);
                     GameManager.Instance.DeductCurrency(CardBack[index].Price);
                     SetContentButtons();
                     GameManager.Instance.Inventory.Add(new InventoryItem(CardBack[index].ItemClass, CardBack[index].ID));
                 });

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

    private void SetContentButtons()
    {
        foreach (var item in initContent)
        {
            item.SetButton(GameManager.Instance.Currency >= item.Price);
        }
    }
}

