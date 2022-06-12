using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;
using UnityEditor;

public class UIAnimation : MonoBehaviour
{
    [HideInInspector]
    public List<MoveAnimation> MoveAnimations;
    [HideInInspector]
    public List<ColorAnimation> ColorAnimation;

    public void AddMove()
    {
        MoveAnimations.Add(new MoveAnimation(transform));
    }
}

[System.Serializable]
public class AnimationBase
{
    public float Duration;
    public bool Reverse;

    public virtual void Play() { }
}
[System.Serializable]
public class MoveAnimation : AnimationBase
{
    public Vector3 Target;
    Transform transform;

    public MoveAnimation(Transform transform)
    {
        this.transform = transform;
    }

    public override void Play()
    {
        transform.DOMove(Target, Duration);
    }
}
[System.Serializable]
public class ColorAnimation : AnimationBase
{
    public Color Target;
    Image image;

    public ColorAnimation(Image image)
    {
        this.image = image;
    }

    public override void Play()
    {
        image.DOColor(Target, Duration);
    }
}
