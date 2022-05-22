using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreScene : MenuScene
{
    public TabScript Nickel;
    public TabScript Pill;

    public GameObject NickelContent;
    public GameObject PillContent;

    public int[] ChestPrices;
    public Text[] ChestsPricesText;
    //public BoxType[] ChestsNames;

    public GameObject EpicChestLock;
    public GameObject LegendaryChestLock;

    //public OpenChestScene ChestScene;

    //public int[] HCValues;
    public StoreItem[] HCStoreItems;

    public int[] SCValues;
    public int[] SCCost;
    public StoreItem[] SCStoreItems;

    public GameObject Loading;
    // Start is called before the first frame update
    void Start()
    {
        Nickel.Pressed(true);
        Pill.Pressed(false);

        for (int i = 0; i < ChestPrices.Length; i++)
        {
            ChestsPricesText[i].text = ChestPrices[i].ToString();
        }

        //if (GameManager.Instance.Me.RankIndex >= 4)
        //{
        //    LegendaryChestLock.SetActive(false);
        //}
        //if (GameManager.Instance.Me.RankIndex >= 2)
        //{
        //    EpicChestLock.SetActive(false);
        //}


        for (int i = 0; i < SCStoreItems.Length; i++)
        {
            SCStoreItems[i].Set(SCCost[i].ToString(), SCValues[i], i, (index) =>
            {
                BuySC(index);
                SFXManager.Instance.PlayClip("Buy");
            });
        }
        //print("rank " + GameManager.Instance.Me.RankIndex);
        //"Arena" + (PlayerRank + 1) + "DropRates";
    }

    public override void Open()
    {
        base.Open();

        Purchaser.Instance.GetAllPrices((prices) =>
        {
            for (int i = 0; i < HCStoreItems.Length; i++)
            {
                HCStoreItems[i].Set(prices[i], Purchaser.Instance.HCAmount[i], i, (index) =>
                  {
                      SFXManager.Instance.PlayClip("Buy");
                      Purchaser.Instance.BuyHC(index, () =>
                       {
                       });
                  });
            }

            Loading.SetActive(false);

        });



        AdjustCashButtons();
    }

    private void AdjustCashButtons()
    {
        for (int i = 0; i < SCStoreItems.Length; i++)
        {
            SCStoreItems[i].SetButton(SCCost[i] <= GameManager.Instance.HardCurrency);
        }
    }

    public void BuySC(int index)
    {

        GameManager.Instance.SubtractHardCurrency(SCCost[index], (result) =>
        {
            if (result)
            {
                GameManager.Instance.AddSoftcurrency(SCValues[index]);
                //SFXManager.Instance.PlayClip("Buy");
            }
            else
            {
                MenuManager.Instance.ShowConnectionError(() => { BuySC(index); });
            }
        });

        AdjustCashButtons();
    }

    public void NickelPressed()
    {
        Nickel.Pressed(true);
        Pill.Pressed(false);

        NickelContent.SetActive(true);
        PillContent.SetActive(false);
    }

    public void PillPressed()
    {
        Nickel.Pressed(false);
        Pill.Pressed(true);

        NickelContent.SetActive(false);
        PillContent.SetActive(true);

        AdjustCashButtons();
    }

    public void BuyChest(int boxType)
    {
        string[] types = new string[] { "_Attack_Chest", "_Defense_Chest", "_Versus_Chest" };
        if (GameManager.Instance.HardCurrency >= ChestPrices[boxType])
        {
            MainMenu.Instance.Popup.ShowWithMessage("You are going to buy the " +
                ChestsNames[boxType] + " Chest for " + ChestPrices[boxType] + " Gems", () =>
                 {
                     string ID = "Arena_" + (GameManager.Instance.Me.RankIndex + 1).ToString("00") + "_" + ChestsNames[boxType] + types[Random.Range(0, types.Length)];

                     //SFXManager.Instance.PlayClip("BuyChest");
                     SFXManager.Instance.PlayClip("Buy");
                     Loading.SetActive(true);
                     PlayfabManager.instance.BuyContainer(ID, ChestPrices[boxType], (boxItems) =>
                          {
                              Loading.SetActive(false);
                              ChestScene.OpenChest(boxItems, ChestsNames[boxType]);
                          });


                 });
        }
        else
        {
            MenuManager.Instance.ShowPopup("NoHC", () =>
            {
                //MenusController.Instance.ToggleStore();
            });
        }
    }
}

//[System.Serializable]
//public class BuyChestData
//{

//}
