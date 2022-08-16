using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class CleanAnimation : MonoBehaviour
{
    public void Clean(Action taskFinished)
    {
        SFXManager.Instance.PlayClip("Clean");
        StartCoroutine(CleanRoutine(taskFinished));
    }

    IEnumerator CleanRoutine(Action taskFinished)
    {
        foreach (Transform item in transform)
        {
            item.DOScale(0, 0.5f).SetEase(Ease.InOutCubic);
            yield return new WaitForSeconds(0.2f);
        }

        yield return new WaitForSeconds(0.3f);
        taskFinished?.Invoke();
        gameObject.SetActive(false);
    }

    internal void Reset()
    {
        foreach (Transform item in transform)
        {
            item.localScale = Vector3.one;
        }

        gameObject.SetActive(true);
    }
}
