using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditableItemUnit : MonoBehaviour
{
    SpriteRenderer sprite;
    Sprite originalSprite;
    [SerializeField]
    Sprite[] varientSprites;

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
        sprite.sprite = varientSprites[index];
    }

    internal void Init()
    {
        if (sprite == null)
        {
            sprite = GetComponent<SpriteRenderer>();
            originalSprite = sprite.sprite;
        }
    }
}
