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

    PurchasableItem selectedItem;
    string selectedCategory;
    int selectedIndex = 0;

    int currentIndex = 0;

    public Button BuyButton;

    public Action OnCancel;
    public Action OnConfirm;

    public Text BuyText;

    bool purchasable;
    public void Show(PurchaseCategory category)
    {
        BuyText.text = "Selected";
        BuyButton.interactable = false;

        gameObject.SetActive(true);
        selectedCategory = category.Code;
        foreach (Transform item in GridPanel)
        {
            Destroy(item.gameObject);
        }

        //this.currentIndex = currentIndex;

        int index = 0;

        foreach (var item in category.Items)
        {
            GameObject purchaseItem = Instantiate(ItemButton, GridPanel);

            purchaseItem.GetComponent<MajlisItem>().Set(category.Code,index, item);
            purchaseItem.GetComponent<MajlisItem>().OnPressed += MajlisItem_Pressed;

            index++;
        }
    }

    public void MajlisItem_Pressed(PurchasableItem item, string category, int index)
    {
        OnItemSelected?.Invoke(item.Model);

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
            purchasable = !GameManager.Instance.HasInInventory(category, index);

        if (purchasable)
        {
            BuyText.text = "Buy: " + item.Price;
            purchasable = true;
            selectedItem = item;
            selectedIndex = index;

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
            GameManager.Instance.Inventory.Add(new InventoryItem(selectedCategory, selectedIndex));
            GameManager.Instance.DeductCurrency(selectedItem.Price);
        }
    }
}
