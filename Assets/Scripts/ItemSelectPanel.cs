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

    public void Show(ref EditableItem item, bool isNew, Action<int> confirmed, Action cancelled)
    {
        gameObject.SetActive(true);

        for (int i = 0; i < buttonIcons.Length; i++)
        {
            buttonIcons[i].sprite = item.VarientIcons[i];
        }

        editableItem = item;

        if (isNew)
            editableItem.ChangeItem(0, 0.2f);

        this.confirmed = confirmed;
        this.cancelled = cancelled;
    }

    public void Select(int index)
    {
        selectedIndex = index;

        if (editableItem is MultiEditableTypeItems)
        {
            ((MultiEditableTypeItems)editableItem).ChangeItem(index, 0.2f);
        }
        else
        {
            editableItem.ChangeItem(index, 0.2f);
        }

        SFXManager.Instance.PlayClip("Select");
    }

    public void Done()
    {
        confirmed?.Invoke(selectedIndex);
        gameObject.SetActive(false);
        editableItem.SetOriginal();
        SFXManager.Instance.PlayClip("Confirm");
    }

    public void Close()
    {
        MenuManager.Instance.CameraHover.Unlock();
        editableItem.ResetToOriginal();
        MenuManager.Instance.OpenMain();
        gameObject.SetActive(false);

        SFXManager.Instance.PlayClip("Close");
    }
}
