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
            //prices = new string[] { "1", "2", "3", "4", "5", "6" };
            for (int i = 0; i < CurrencyStoreItems.Length; i++)
            {
                CurrencyStoreItems[i].Set(prices[i], SCValues[i], i, (index) =>
                {
                    SFXManager.Instance.PlayClip("Buy");
                });
            }
            Loading.SetActive(false);
        });

        //AdjustCashButtons();
    }

    //private void AdjustCashButtons(int currency)
    //{
    //    for (int i = 0; i < CurrencyStoreItems.Length; i++)
    //    {
    //        CurrencyStoreItems[i].SetButton(SCCost[i] <= currency);
    //    }
    //}

    //public void BuySC(int index)
    //{
    //    GameManager.Instance.SubtractHardCurrency(SCCost[index], (result) =>
    //    {
    //        if (result)
    //        {
    //            GameManager.Instance.AddSoftcurrency(SCValues[index]);
    //            //SFXManager.Instance.PlayClip("Buy");
    //        }
    //        else
    //        {
    //            MenuManager.Instance.ShowConnectionError(() => { BuySC(index); });
    //        }
    //    });
    //    AdjustCashButtons();
    //}

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

            for (int i = 0; i < CardBack.Count; i++)
            {
                StoreItem storeItem = Instantiate(ContentStoreItem, CardsContent).GetComponent<StoreItem>();
                storeItem.Set(CardBack[i].Price.ToString(), Resources.Load<Sprite>("CardBack/" + CardBack[i].ID),i, (index) => 
                {

                });
            }

            contentLoaded = true;
        }
    }
}

