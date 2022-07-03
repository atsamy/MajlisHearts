using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
public class CategoryPanel : MonoBehaviour
{
    public GameObject ItemButton;

    public Transform GridPanel;

    public delegate void ItemSelected(string ID, GameObject Model);
    public ItemSelected OnItemSelected;

    CatalogueItem selectedItem;
    string selectedCategory;
    //int selectedIndex = 0;
    string selectedID;

    [SerializeField]
    RectTransform panel;
    //int currentIndex = 0;

    public Button BuyButton;

    public Action OnCancel;
    public Action OnConfirm;

    public TextMeshProUGUI BuyText;

    bool purchasable;
    public void Show(string categoryName, float itemPosition)
    {
        List<CatalogueItem> category = GameManager.Instance.Catalog[categoryName];

        BuyText.text = "Selected";
        BuyButton.interactable = false;

        gameObject.SetActive(true);
        selectedCategory = categoryName;
        //selectedID = editableItem.SelectedID;
        panel.anchoredPosition = new Vector2(Mathf.Abs(panel.anchoredPosition.x) * (itemPosition > 0.5 ? -1 : 1), panel.anchoredPosition.y);

        foreach (Transform item in GridPanel)
        {
            Destroy(item.gameObject);
        }

        int index = 0;

        foreach (var item in category)
        {
            MajlisItem purchaseItem = Instantiate(ItemButton, GridPanel).GetComponent<MajlisItem>();

            purchaseItem.Set(selectedCategory, index, item);
            purchaseItem.OnPressed += MajlisItem_Pressed;

            index++;
        }
    }

    public void MajlisItem_Pressed(CatalogueItem item)
    {
        OnItemSelected?.Invoke(item.ID, item.GetModel());

        if (item.Level > GameManager.Instance.MyPlayer.Level)
        {
            BuyText.text = "Unlock at Level " + item.Level;
            BuyButton.interactable = false;
            return;
        }
        if (item.ID == selectedID)
        {
            BuyText.text = "Selected";
            BuyButton.interactable = false;
            return;
        }


        purchasable = !GameManager.Instance.HasInInventory(item.ItemClass, item.ID);

        //selectedIndex = index;
        selectedItem = item;

        if (purchasable)
        {
            BuyText.text = "Buy: " + item.Price;
            purchasable = true;

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
        OnCancel?.Invoke();
    }

    public void ConfirmPressed()
    {
        gameObject.SetActive(false);
        OnConfirm?.Invoke();

        selectedID = selectedItem.ID;
        //currentIndex = selectedIndex;

        GameManager.Instance.SetCustomization(selectedCategory, selectedItem.ID);

        if (purchasable)
        {
            GameManager.Instance.Inventory.Add(new InventoryItem(selectedCategory, selectedItem.ID));
            GameManager.Instance.DeductCurrency(selectedItem.Price);

            PlayfabManager.instance.AddItemToInventory(selectedItem);
        }
    }
}
