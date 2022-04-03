using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class MajlisScript : MonoBehaviour
{
    public EditableItem[] EditableItems;
    public static MajlisScript Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void SetItems(List<InventoryItem> Customization, Dictionary<string, List<CatalogueItem>> AllItems)
    {
        foreach (var item in EditableItems)
        {
            if (Customization.Any(a => a.Category == item.Code))
            {
                string id = Customization.Find(a => a.Category == item.Code).ID;
                CatalogueItem catalogueItem = AllItems[item.Code].Find(a => a.ID == id);

                GameObject obj = Instantiate(catalogueItem.GetModel(), item.transform);

                Destroy(item.Model);

                item.Model = obj;
                obj.transform.localPosition = Vector3.zero;

            }
        }
    }
}
