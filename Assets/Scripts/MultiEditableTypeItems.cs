using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiEditableTypeItems : EditableItem
{
    [SerializeField]
    SingleEditableItem[] AllItems;

    public override void ResetToOriginal()
    {
        foreach (var item in AllItems)
        {
            item.ResetToOriginal();
        }
    }

    public override void ChangeItem(int index)
    {
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
}