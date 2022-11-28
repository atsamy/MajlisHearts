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

    public override void Reset()
    {
        foreach (var item in AllItems)
        {
            item.Reset();
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
            item.ChangeItem(index,time);
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
    public override void SetModified(int index, bool userModify)
    {
        if (enableEffect && userModify)
        {
            if (effectMaterial == null)
            {
                effectMaterial = new Material(Shader.Find("Shader Graphs/EffectMaterial"));

                foreach (var item in AllItems)
                {
                    item.GetComponent<SpriteRenderer>().material = effectMaterial;
                }
            }
        }

        base.SetModified(index,userModify);

        foreach (var item in AllItems)
        {
            item.SetModified();
        }
    }

    //public override void SetOriginal()
    //{

    //}
}
