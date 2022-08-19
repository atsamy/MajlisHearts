using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HandPointer : MonoBehaviour
{
    void Start()
    {
        transform.DOScale(0.8f, 0.75f).SetEase(Ease.InCubic).SetLoops(-1,LoopType.Yoyo);
    }

    int index = 0;

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.A))
    //    {
    //        transform.DOScale(0.6f, 1).SetEase((Ease)index).SetLoops(4,LoopType.Yoyo);
    //        print((Ease)index);
    //        index++;
    //    }
            
    //}
}
