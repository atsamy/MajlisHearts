using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiEditableTypeItems : EditableItem
{
    [SerializeField]
    SingleEditableItem[] AllItems;

    //public int VarientsCount => AllItems[0].VarientsCount;

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

        return AllItems[0].GetVarientsCount();
    }

    public override void ChangeItem(int index)
    {
        //SetModified(index);
        foreach (var item in AllItems)
        {
            item.ChangeItem(index);
        }
    }

    public override void ChangeItem(int index, float time)
    {
        if (disableAnimation)
        {
            ChangeItem(index);
            return;
        }

        //SetModified(index);
        foreach (var item in AllItems)
        {
            item.ChangeItem(index, time);
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

    public override void SetModified(int index,bool userModify)
    {
        base.SetModified(index, userModify);
        foreach (var item in AllItems)
        {
            item.SetModified(index, userModify);
        }
    }

    public override void Reset()
    {
        foreach (var item in AllItems)
        {
            item.Reset();
        }
    }
}
