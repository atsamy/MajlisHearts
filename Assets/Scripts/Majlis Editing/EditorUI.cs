using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class EditorUI : MonoBehaviour
{
    EditableItem[] AllItems;
    //public GameObject EditButton;
    //public Transform EditButtonsParent;

    //GameObject[] EditButtons;
    Camera myCamera;

    string selectedID;
    EditableItem selectedItem;
    public CategoryPanel CategoryPanel;
    CameraHover cameraHover;

    public static EditorUI Instance;

    void Start()
    {
        Instance = this;
        //AllItems = FindObjectsOfType<EditableItem>();
        //EditButtons = new GameObject[AllItems.Length];

        //int index = 0;

        //foreach (var item in AllItems)
        //{
        //    EditButtons[index] = Instantiate(EditButton, EditButtonsParent);

        //    EditButtons[index].GetComponent<Button>().onClick.AddListener(() =>
        //    {
        //        
        //        selectedItem = item;
        //        EditButtonsParent.gameObject.SetActive(false);

        //        cameraHover.GoToLocation(item.CameraLocation);
        //    });

        //    index++;
        //}

        myCamera = Camera.main;
        cameraHover = myCamera.GetComponent<CameraHover>();

        CategoryPanel.OnItemSelected = CategoryPanel_OnItemSelected;
        CategoryPanel.OnCancel = CategoryPanel_OnCancel;
        CategoryPanel.OnConfirm = CategoryPanel_OnConfirm;
    }

    private void CategoryPanel_OnConfirm()
    {
        selectedItem.SelectedID = selectedID;
        cameraHover.GoBack();
    }

    public void CategoryPanel_OnCancel()
    {
        if (selectedItem == null)
            return;

        CategoryPanel.gameObject.SetActive(false);
        selectedItem.ResetToOriginal();
        cameraHover.GoBack();
    }

    private void CategoryPanel_OnItemSelected(string ID,GameObject obj)
    {
        selectedItem.ChangeItem(ID);
        selectedID = ID;
    }

    // Update is called once per frame
    //void Update()
    //{
    //    //for (int i = 0; i < EditButtons.Length; i++)
    //    //{
    //    //    EditButtons[i].transform.position = myCamera.WorldToScreenPoint(AllItems[i].transform.position);
    //    //}
    //}

    public void ShowItems(string item,float itemPosition)
    {
        MenuManager.Instance.CloseMain();
        CategoryPanel.Show(item, itemPosition);
    }
}
