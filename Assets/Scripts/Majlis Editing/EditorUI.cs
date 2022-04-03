using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class EditorUI : MonoBehaviour
{
    EditableItem[] AllItems;
    public GameObject EditButton;
    public Transform EditButtonsParent;

    GameObject[] EditButtons;
    Camera myCamera;

    //GameObject selectedItem;
    GameObject initItem;

    EditableItem selectedItem;

    public CategoryPanel CategoryPanel;
    CameraHover cameraHover;
    // Start is called before the first frame update
    void Start()
    {
        AllItems = FindObjectsOfType<EditableItem>();
        EditButtons = new GameObject[AllItems.Length];

        int index = 0;

        foreach (var item in AllItems)
        {
            EditButtons[index] = Instantiate(EditButton, EditButtonsParent);

            EditButtons[index].GetComponent<Button>().onClick.AddListener(() =>
            {
                ShowItems(item.Code);
                selectedItem = item;
                EditButtonsParent.gameObject.SetActive(false);

                cameraHover.GoToLocation(item.CameraLocation);
            });

            index++;
        }

        myCamera = Camera.main;
        cameraHover = myCamera.GetComponent<CameraHover>();

        CategoryPanel.OnItemSelected = CategoryPanel_OnItemSelected;
        CategoryPanel.OnCancel = CategoryPanel_OnCancel;
        CategoryPanel.OnConfirm = CategoryPanel_OnConfirm;
    }

    private void CategoryPanel_OnConfirm()
    {
        //Destroy(selectedItem);
        selectedItem.Model = initItem;
        initItem = null;

        EditButtonsParent.gameObject.SetActive(true);
        cameraHover.GoBack();
    }

    private void CategoryPanel_OnCancel()
    {
        selectedItem.Model.SetActive(true);
        Destroy(initItem);

        EditButtonsParent.gameObject.SetActive(true);
        cameraHover.GoBack();
    }

    private void CategoryPanel_OnItemSelected(GameObject obj)
    {
        selectedItem.Model.SetActive(false);

        if (initItem != null)
        {
            Destroy(initItem);
        }

        initItem = Instantiate(obj, selectedItem.Model.transform.position, selectedItem.Model.transform.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < EditButtons.Length; i++)
        {
            EditButtons[i].transform.position = myCamera.WorldToScreenPoint(AllItems[i].transform.position);
        }
    }

    public void ShowItems(string code)
    {
        //List<CatalogueItem> current = Catalogue.Instance.AllItems[code];
        CategoryPanel.Show(code);
    }
}
