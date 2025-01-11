using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class IndexedButton : MonoBehaviour
{
    public Action<int> OnClicked;
    [SerializeField]
    int index;

    public void Clicked()
    {
        OnClicked?.Invoke(index);
    }
}
