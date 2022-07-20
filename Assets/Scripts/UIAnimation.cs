using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;
using UnityEditor;
using System.Threading.Tasks;

public class UIAnimation : MonoBehaviour
{
    [HideInInspector]
    public List<MoveAnimation> MoveAnimations;
    [HideInInspector]
    public List<ScaleAnimation> ScaleAnimation;
    [HideInInspector]
    public List<ColorAnimation> ColorAnimation;

    public bool PlayOnEnable;
    MaskableGraphic image;
    RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<MaskableGraphic>();
    }

    private void OnEnable()
    {
        if (PlayOnEnable)
        {
            Play();
        }
    }

    public void Play()
    {
        foreach (var item in MoveAnimations)
        {
            item.Set(rectTransform);
            item.Play();
        }

        foreach (var item in ScaleAnimation)
        {
            item.Set(transform);
            item.Play();
        }

        foreach (var item in ColorAnimation)
        {
            item.Set(image);
            item.Play();
        }
    }
}

[System.Serializable]
public class AnimationBase
{
    public float Duration;
    public bool Reverse;
    public float StartAfter;

    public async void Play()
    {
        await Task.Delay((int)(StartAfter * 100));

        if (Reverse)
            PlayReverse();
        else
            PlaySingle();
    }
    protected virtual void PlaySingle() { }
    protected virtual void PlayReverse() { }
}
[System.Serializable]
public abstract class TransformAnimationBase : AnimationBase
{
    public void Set(Transform transform)
    {
        this.transform = transform;
    }

    public Vector3 Start;
    protected Transform transform;
}

public abstract class RectTransformAnimationBase : AnimationBase
{
    public void Set(RectTransform transform)
    {
        this.transform = transform;
    }

    public Vector3 Start;
    protected RectTransform transform;
}

[System.Serializable]
public class MoveAnimation : RectTransformAnimationBase
{
    protected override void PlaySingle()
    {
        Vector3 Target = transform.anchoredPosition;
        transform.anchoredPosition = Start;
        transform.DOAnchorPos(Target, Duration).SetUpdate(true);
    }

    protected override void PlayReverse()
    {
        Vector3 Target = transform.anchoredPosition;
        transform.anchoredPosition = Start;
        transform.DOAnchorPos(Target, Duration).SetLoops(1, LoopType.Yoyo).SetUpdate(true);
    }
}

[System.Serializable]
public class ColorAnimation : AnimationBase
{
    public Color Start;
    MaskableGraphic image;

    public void Set(MaskableGraphic image)
    {
        this.image = image;
    }
    protected override void PlaySingle()
    {
        Debug.Log(image.name);
        Color Target = image.color;
        image.color = Start;
        image.DOColor(Target, Duration).SetUpdate(true);
    }
    protected override void PlayReverse()
    {
        Color Target = image.color;
        image.color = Start;
        image.DOColor(Target, Duration).SetLoops(1, LoopType.Yoyo).SetUpdate(true);
    }
}
[System.Serializable]
public class ScaleAnimation : TransformAnimationBase
{
    protected override void PlaySingle()
    {
        Vector3 Target = transform.localScale;
        transform.localScale = Start;
        transform.DOScale(Target, Duration).SetUpdate(true);
    }

    protected override void PlayReverse()
    {
        Vector3 Target = transform.localScale;
        transform.localScale = Start;
        transform.DOScale(Target, Duration).SetLoops(1, LoopType.Yoyo).SetUpdate(true);
    }
}
