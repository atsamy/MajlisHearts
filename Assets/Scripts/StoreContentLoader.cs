using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StoreContentLoader : MonoBehaviour
{
    public string Category;

    [SerializeField]
    GameObject contentStoreItem;
    [SerializeField]
    Transform content;

    List<ContentStoreItem> initContent;
    Dictionary<string, ContentStoreItem> equippedCatalogueItem;

    void Start()
    {
        equippedCatalogueItem = new Dictionary<string, ContentStoreItem>();
        LoadCategory(Category);
    }

    private void LoadCategory(string category)
    {
        List<CatalogueItem> catalogueItems = GameManager.Instance.Catalog.First(a => a.Key == category).Value;
        initContent = new List<ContentStoreItem>();

        for (int i = 0; i < catalogueItems.Count; i++)
        {
            ContentStoreItem storeItem = Instantiate(contentStoreItem, content).GetComponent<ContentStoreItem>();
            bool equipped = false;

            if (GameManager.Instance.EquippedItem.ContainsKey(category))
                equipped = (GameManager.Instance.EquippedItem[category] == catalogueItems[i].ID);

            bool owned = GameManager.Instance.Inventory.Any(a => a.ID == catalogueItems[i].ID);

            if (equipped)
                equippedCatalogueItem.Add(category, storeItem);

            storeItem.Set(catalogueItems[i].Price, catalogueItems[i].Name, Resources.Load<Sprite>(category + "/" + catalogueItems[i].ID + "_Icon"), i, owned, equipped, (index) =>
            {
                print(catalogueItems[index].Price);

                if (catalogueItems[index].Price > GameManager.Instance.Currency)
                {
                    MenuManager.Instance.Popup.ShowWithMessage("you dont have enough money, buy more coins?", () =>
                    {
                        transform.GetComponentInParent<StoreScene>().TabPressed(0);
                    });
                    return;
                }
                MenuManager.Instance.Popup.ShowWithMessage("are you sure you want to buy this item", () =>
                {
                    SFXManager.Instance.PlayClip("Buy");

                    PlayfabManager.instance.AddItemToInventory(catalogueItems[index]);
                    GameManager.Instance.DeductCurrency(catalogueItems[index].Price);
                    //SetContentButtons();
                    GameManager.Instance.Inventory.Add(new InventoryItem(catalogueItems[index].ItemClass, catalogueItems[index].ID));
                    EquibNewItem(category, storeItem, catalogueItems[index].ID);
                });

            }, (index) =>
            {
                SFXManager.Instance.PlayClip("Equip");
                EquibNewItem(category, storeItem, catalogueItems[index].ID);
            });

            //if (!equipped && !owned)
            //    storeItem.SetButton(GameManager.Instance.Currency >= catalogueItems[i].Price);

            initContent.Add(storeItem);
        }
    }

    private void EquibNewItem(string category, ContentStoreItem storeItem, string ID)
    {
        GameManager.Instance.EquipItem(category, ID);

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

}
