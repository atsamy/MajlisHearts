using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class ItemSelectPanel : MonoBehaviour
{
    Action<int> confirmed;
    Action cancelled;

    [SerializeField]
    Image[] buttonIcons;
    int selectedIndex = 0;
    EditableItem editableItem;

    public void Show(ref EditableItem item,Action<int> confirmed,Action cancelled)
    {
        gameObject.SetActive(true);

        for (int i = 0; i < buttonIcons.Length; i++)
        {
            buttonIcons[i].sprite = item.VarientIcons[i];
        }

        editableItem = item;
        editableItem.ChangeItem(0);

        this.confirmed = confirmed;
        this.cancelled = cancelled;
    }

    public void Select(int index)
    {
        selectedIndex = index;
        editableItem.ChangeItem(index);
    }

    public void Done()
    {
        confirmed?.Invoke(selectedIndex);
        gameObject.SetActive(false);
    }

    public void Close()
    {
        editableItem.ResetToOriginal();
        gameObject.SetActive(false);
    }
}
