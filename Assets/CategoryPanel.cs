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
    
    public Action OnCancel;
    public Action OnConfirm;

    public Text BuyText;

    bool purchasable;
    public void Show(PurchaseCategory category)
    {
        gameObject.SetActive(true);

        foreach (Transform item in GridPanel)
        {
            Destroy(item.gameObject);
        }

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

        purchasable = !GameManager.Instance.HasInInventory(category, index);

        if (purchasable)
        {
            BuyText.text = "Buy: " + item.Price;
            purchasable = true;
        }
        else
        {
            BuyText.text = "Select";
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

        if (purchasable)
        {
            
        }
    }
}
