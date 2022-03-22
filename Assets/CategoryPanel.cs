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
    public void Show(PurchaseCategory category)
    {
        gameObject.SetActive(true);

        foreach (Transform item in GridPanel)
        {
            Destroy(item.gameObject);
        }

        foreach (var item in category.Items)
        {
            GameObject purchaseItem = Instantiate(ItemButton, GridPanel);
            purchaseItem.GetComponent<Image>().sprite = item.Sprite;

            purchaseItem.GetComponent<Button>().onClick.AddListener(() => 
            {
                OnItemSelected?.Invoke(item.Model);
            });
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
    }
}
