using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StoreScene : MenuScene
{
    public TabScript[] Tabs;
    public GameObject[] ContentPanels;

    public StoreItem[] CurrencyStoreItems;
    public StoreItem[] GemsStoreItems;

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
        WatchVideoBtn.Set(LanguageManager.Instance.GetString("watchad"), 100, 0, (index) =>
        {
            AdsManager.Instance.ShowRewardedAd((result) =>
            {
                if (result)
                {
                    SFXManager.Instance.PlayClip("Coins");
                    GameManager.Instance.AddCoins(100);
                }
            });
        });

        for (int i = 0; i < GemsStoreItems.Length; i++)
        {
            int[] gemsPrices = Purchaser.Instance.GemsPrices;
            GemsStoreItems[i].Set(gemsPrices[i * 2].ToString(),
                gemsPrices[(i * 2) + 1], i, (index) =>
                {
                    if (GameManager.Instance.Coins >= gemsPrices[index * 2])
                    {
                        MenuManager.Instance.OpenPopup("buygems", false, false, () =>
                        {
                            GameManager.Instance.DeductCoins(gemsPrices[index * 2]);
                            GameManager.Instance.AddGems(gemsPrices[index * 2 + 1]);

                            SFXManager.Instance.PlayClip("Coins");
                        });


                    }
                    else
                    {
                        MenuManager.Instance.OpenPopup("nocoins", false, false, () =>
                        {
                            transform.GetComponentInParent<StoreScene>().TabPressed(0);
                        });
                    }
                });
        }

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
        Purchaser.Instance.GetAllPrices((prices, amounts) =>
        {
            for (int i = 0; i < CurrencyStoreItems.Length; i++)
            {
                //Debug.Log("item " + i + " price " + prices[i] + " amount " + amounts[i]);
                CurrencyStoreItems[i].Set(prices[i], amounts[i], i, (index) =>
                {
                    Purchaser.Instance.BuyCurrency(index, () =>
                    {
                        SFXManager.Instance.PlayClip("Coins");
                        GameManager.Instance.AddCoins(amounts[index]);
                    });
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

        arrow.transform.position = new Vector3(Tabs[index].transform.position.x, arrow.transform.position.y);
    }
}

