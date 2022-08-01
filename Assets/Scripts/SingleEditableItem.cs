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

    public override void ChangeItem(int index)
    {
        transform.DOScale(0.8f, 0.25f).SetLoops(1, LoopType.Yoyo).SetEase(Ease.InOutCubic).OnComplete(()=>
        {
            sprite.sprite = varientSprites[index];
        });
    }
}
