using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleEditableItem : EditableItem
{

    SpriteRenderer sprite;
    Sprite originalSprite;
    [SerializeField]
    Sprite[] varientSprites;

    public Sprite[] VarientSprites { get => varientSprites; }

    private void Awake()
    {
        Init();
    }

    public void Init()
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
        sprite.sprite = varientSprites[index];
    }
}
