using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StoreScene : MenuScene
{
    public TabScript[] Tabs;
    public GameObject[] ContentPanels;

    public StoreItem[] CurrencyStoreItems;
    public StoreItem WatchVideoBtn;

    [SerializeField]
    Transform arrow;
    [SerializeField]
    ScrollRect[] storesItems;

    ScrollRect currentScrollRect;
    public GameObject Loading;

    [SerializeField]
    StoreContentLoader avatarContent;

    private void OnEnable()
    {
        currentScrollRect.content.localPosition = new Vector3(-2, 0);
    }

    void Start()
    {
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
        currentScrollRect = storesItems[index];
        Open();
        MenuManager.Instance.HideMain(false, false);
        TabPressed(index);
    }

    public override void Open()
    {
        base.Open();
        MenuManager.Instance.HideMain(false, false);
        Purchaser.Instance.GetAllPrices((prices,amounts) =>
        {
            for (int i = 0; i < CurrencyStoreItems.Length; i++)
            {
                //Debug.Log("item " + i + " price " + prices[i] + " amount " + amounts[i]);
                CurrencyStoreItems[i].Set(prices[i], amounts[i], i, (index) =>
                {
                    SFXManager.Instance.PlayClip("Coins");
                    GameManager.Instance.AddCoins(amounts[index]);
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

