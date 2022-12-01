using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipleEditableObject : EditableItem
{
    public GameObject[] Objects;
    int originalIndex = -1;
    public override void ChangeItem(int index,bool showEffect)
    {
        //SetModified(index);
        for (int i = 0; i < Objects.Length; i++)
        {
            Objects[i].SetActive(i == index);
        }
    }

    public override void ChangeItem(int index, float time)
    {
        ChangeItem(index,false);
    }

    public override void ResetToOriginal()
    {
        for (int i = 0; i < Objects.Length; i++)
        {
            Objects[i].SetActive(false);
        }
    }

    public override void Reset()
    {
        for (int i = 0; i < Objects.Length; i++)
        {
            Objects[i].SetActive(i == originalIndex);
        }
    }

    public override int GetVarientsCount()
    {
        return Objects.Length;
    }

    public override void Init()
    {
        originalIndex = -1;
    }

    public override void SetModified(int index,bool userModify)
    {
        originalIndex = selectedIndex;
        base.SetModified(index,userModify);
    }
}
