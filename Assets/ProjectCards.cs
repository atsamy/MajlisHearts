using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine;

public class ProjectCards : MonoBehaviour
{
    public Transform[] CardsHolder;
    public TextMeshProUGUI[] NameText;

    public void RemoveAll()
    {
        foreach (var holder in CardsHolder)
        {
            foreach (Transform item in holder)
            {
                Destroy(item.gameObject);
            }
        }

        foreach (var item in NameText)
        {
            item.text = "";
        }
    }
}
