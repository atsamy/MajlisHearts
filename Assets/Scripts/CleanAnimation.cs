using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class CleanAnimation : MonoBehaviour
{
    public void Clean()
    {
        StartCoroutine(CleanRoutine());
    }

    IEnumerator CleanRoutine()
    {
        foreach (Transform item in transform)
        {
            item.DOScale(0, 0.5f).SetEase(Ease.InOutCubic);
            yield return new WaitForSeconds(0.2f);
        }

        yield return new WaitForSeconds(0.3f);

        gameObject.SetActive(false);
    }

    internal void Reset()
    {
        SFXManager.Instance.PlayClip("Clean");

        foreach (Transform item in transform)
        {
            item.localScale = Vector3.one;
        }

        gameObject.SetActive(true);
    }
}
