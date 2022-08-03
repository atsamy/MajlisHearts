using System.Collections.Generic;
using UnityEngine;

public class StoreScene : MenuScene
{
    public TabScript[] Tabs;
    public GameObject[] ContentPanels;

    public StoreItem[] CurrencyStoreItems;
    public StoreItem WatchVideoBtn;

    public int[] SCValues;
    
    public GameObject Loading;

    [SerializeField]
    StoreContentLoader avatarContent;

    void Start()
    {
        //for (int i = 0; i < Tabs.Length; i++)
        //{
        //    Tabs[i].Pressed(i == 0);
        //}

        WatchVideoBtn.Set(LanguageManager.Instance.GetString("watchad"), 50, 0, (index) =>
        {
            AdsManager.Instance.ShowRewardedAd((result)=>
            {
                if (result)
                {
                    SFXManager.Instance.PlayClip("Coins");
                    PlayfabManager.instance.AddCurrency(50, (result) =>
                    {
                        print("add currency: " + result);
                    });
                    GameManager.Instance.AddCurrency(SCValues[index]);
                }
            });
        });

        avatarContent.itemEquipped += (item) =>
        {
            GameManager.Instance.SaveAvatar(item);
            MenuManager.Instance.Header.SetAvatar();
        };
    }

    public void Open(int index)
    {
        Open();
        TabPressed(index);
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
                    SFXManager.Instance.PlayClip("Coins");
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

    public void TabPressed(int index)
    {
        for (int i = 0; i < Tabs.Length; i++)
        {
            Tabs[i].Pressed(index == i);
        }
        for (int i = 0; i < ContentPanels.Length; i++)
        {
            ContentPanels[i].SetActive(index == i);
        }
    }
}

