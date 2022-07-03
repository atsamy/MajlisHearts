using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class MajlisScript : MonoBehaviour
{
    public EditableItem[] EditableItems;
    public static MajlisScript Instance;
    //internal Dictionary<string, List<CatalogueItem>> AllItems;

    GameManager gameManager;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {

        gameManager = GameManager.Instance;
        SetItems(gameManager.Customization);
        //PlayfabManager.instance.GetUserData();
    }

    public void SetItems(List<InventoryItem> customization)
    {
        foreach (var item in EditableItems)
        {
            if (customization.Any(a => a.Category == item.Code))
            {
                string id = customization.Find(a => a.Category == item.Code).ID;
                CatalogueItem catalogueItem = gameManager.Catalog[item.Code].Find(a => a.ID == id);

                GameObject obj = Instantiate(catalogueItem.GetModel(), item.transform);

                //Destroy(item.Model);

                item.SelectedID = id;
                //item.Model = obj;
                obj.transform.localPosition = Vector3.zero;
            }
            else
            {
                item.SelectedID = gameManager.Catalog[item.Code].First().ID;
            }
        }
    }

}


