using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SingleEditableItem : EditableItem
{

    SpriteRenderer sprite;
    protected Sprite originalSprite;
    [SerializeField]
    Sprite[] varientSprites;

    public Sprite[] VarientSprites { get => varientSprites; }

    private void Awake()
    {
        Init();
    }

    public override void Init()
    {
        if (sprite == null)
        {
            sprite = GetComponent<SpriteRenderer>();
            originalSprite = sprite.sprite;
        }
    }

    public override void ResetToOriginal()
    {
        sprite.sprite = originalSprite;
    }

    public override void SetOriginal()
    {
        originalSprite = sprite.sprite;
    }

    public override void ChangeItem(int index)
    {
        SetModified();
        sprite.sprite = varientSprites[index];
    }

    public override void ChangeItem(int index,float time)
    {
        SetModified();
        transform.DOScale(0.7f, time * 2).SetEase(Ease.InOutCubic).OnComplete(() =>
        {
            sprite.sprite = varientSprites[index];
            transform.DOScale(1f, time).SetEase(Ease.Flash);
        });
    }
}
