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

    //[SerializeField]
    //Sprite[] frameSprites;

    [SerializeField]
    Button[] frameImages;

    public void Show(ref EditableItem item, int selectedIndex, Action<int> confirmed, Action cancelled)
    {
        gameObject.SetActive(true);

        for (int i = 0; i < buttonIcons.Length; i++)
        {
            buttonIcons[i].sprite = item.VarientIcons[i];
        }

        editableItem = item;

        editableItem.ChangeItem(selectedIndex, 0.2f);
        for (int i = 0; i < 3; i++)
        {
            frameImages[i].interactable = (selectedIndex != i);
        }

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

        for (int i = 0; i < 3; i++)
        {
            frameImages[i].interactable = (index != i);
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
