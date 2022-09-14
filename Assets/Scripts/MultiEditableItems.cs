using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiEditableItems : EditableItem
{
    [SerializeField]
    EditableItemUnit[] AllItems;

    public override void ResetToOriginal()
    {
        foreach (var item in AllItems)
        {
            item.ResetToOriginal();
        }
    }
    public override int GetVarientsCount()
    {
        if (AllItems.Length == 0)
            return 0;

        return AllItems[0].VarientCount;
    }

    public override void ChangeItem(int index)
    {
        SetModified();
        foreach (var item in AllItems)
        {
            item.ChangeItem(index);
        }
    }

    public override void ChangeItem(int index, float time)
    {
        SetModified();
        foreach (var item in AllItems)
        {
            item.ChangeItem(index);
        }
    }

    public override void Init()
    {
        foreach (var item in AllItems)
        {
            if(item != null)
            item.Init();
        }
    }

    public override void SetOriginal()
    {
        foreach (var item in AllItems)
        {
            item.SetOriginal();
        }
    }
}
