using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class EditableItemUnit : MonoBehaviour
{
    SpriteRenderer sprite;
    Sprite originalSprite;
    [SerializeField]
    Sprite[] varientSprites;

    public int VarientCount => varientSprites.Length;

    private void Awake()
    {
        Init();
    }

    internal virtual void ResetToOriginal()
    {
        sprite.sprite = originalSprite;
    }

    internal virtual void ChangeItem(int index)
    {
        if (index >= varientSprites.Length)
            return;

        sprite.sprite = varientSprites[index];
    }

    public void ChangeItem(int index, float time)
    {
        //SetModified(index);

        //if (disableAnimation)
        //{
        //    sprite.sprite = varientSprites[index];
        //    return;
        //}

        transform.DOJump(transform.position, 0.2f, 1, time * 2).OnComplete(() =>
        {
            sprite.sprite = varientSprites[index];
        });
    }

    internal void Init()
    {
        if (sprite == null)
        {
            sprite = GetComponent<SpriteRenderer>();
            originalSprite = sprite.sprite;
        }
    }

    internal void SetOriginal()
    {
        originalSprite = sprite.sprite;
    }
}
