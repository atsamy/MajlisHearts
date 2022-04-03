using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CategoryPanel : MonoBehaviour
{
    public GameObject ItemButton;

    public Transform GridPanel;

    public Action<GameObject> OnItemSelected;

    CatalogueItem selectedItem;
    string selectedCategory;
    int selectedIndex = 0;
    //string selectedID;

    int currentIndex = 0;

    public Button BuyButton;

    public Action OnCancel;
    public Action OnConfirm;

    public Text BuyText;

    bool purchasable;
    public void Show(string categoryClass)
    {
        BuyText.text = "Selected";
        BuyButton.interactable = false;

        gameObject.SetActive(true);
        selectedCategory = categoryClass;

        foreach (Transform item in GridPanel)
        {
            Destroy(item.gameObject);
        }

        int index = 0;

        List<CatalogueItem> category = Catalogue.Instance.AllItems[categoryClass];

        foreach (var item in category)
        {
            MajlisItem purchaseItem = Instantiate(ItemButton, GridPanel).GetComponent<MajlisItem>();

            purchaseItem.Set(categoryClass, index, item);
            purchaseItem.OnPressed += MajlisItem_Pressed;

            index++;
        }
    }

    public void MajlisItem_Pressed(CatalogueItem item, int index)
    {
        OnItemSelected?.Invoke(item.GetModel());

        if (item.Level > GameManager.Instance.MyPlayer.Level)
        {
            BuyText.text = "Unlock at Level " + item.Level;
            BuyButton.interactable = false;
            return;
        }
        if (currentIndex == index)
        {
            BuyText.text = "Selected";
            BuyButton.interactable = false;
            return;
        }

        if (index == 0)
            purchasable = false;
        else
            purchasable = !GameManager.Instance.HasInInventory(item.ItemClass, item.ID);

        selectedIndex = index;

        if (purchasable)
        {
            BuyText.text = "Buy: " + item.Price;
            purchasable = true;
            selectedItem = item;

            BuyButton.interactable = (item.Price <= GameManager.Instance.Currency);
        }
        else
        {
            BuyText.text = "Select";
            BuyButton.interactable = true;
        }
    }

    public void CancelPressed()
    {
        gameObject.SetActive(false);
        OnCancel?.Invoke();
    }

    public void ConfirmPressed()
    {
        gameObject.SetActive(false);
        OnConfirm?.Invoke();

        currentIndex = selectedIndex;

        if (purchasable)
        {
            GameManager.Instance.Inventory.Add(new InventoryItem(selectedCategory, selectedItem.ID));
            GameManager.Instance.DeductCurrency(selectedItem.Price);

            PlayfabManager.instance.AddItemToInventory(selectedItem);
        }
    }
}
