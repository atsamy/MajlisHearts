using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StoreScene : MenuScene
{
    public TabScript[] Tabs;
    public GameObject[] ContentPanels;

    public StoreItem[] CurrencyStoreItems;
    public StoreItem WatchVideoBtn;

    [SerializeField]
    Transform arrow;
    //public int[] SCValues;

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
                    GameManager.Instance.AddCoins(Purchaser.Instance.HCAmount[index]);
                }
            });
        });

        avatarContent.itemEquipped += (item) =>
        {
            GameManager.Instance.SaveAvatar(item);
            MenuManager.Instance.MainPanel.SetAvatar();
        };
    }

    public void Open(int index)
    {
        Open();
        MenuManager.Instance.HideMain(false, false);
        TabPressed(index);
    }

    public override void Open()
    {
        base.Open();
        MenuManager.Instance.HideMain(false, false);
        Purchaser.Instance.GetAllPrices((prices) =>
        {
            for (int i = 0; i < CurrencyStoreItems.Length; i++)
            {
                CurrencyStoreItems[i].Set(prices[i], Purchaser.Instance.HCAmount[i], i, (index) =>
                {
                    SFXManager.Instance.PlayClip("Coins");
                    GameManager.Instance.AddCoins(Purchaser.Instance.HCAmount[index]);
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

        arrow.transform.position = Tabs[index].transform.position + Vector3.down * 75;
    }
}

