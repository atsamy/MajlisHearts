using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilableEditableItem : SingleEditableItem
{
    SpriteRenderer[] allTiles;

    public override void ChangeItem(int index)
    {
        foreach (var item in allTiles)
        {
            item.sprite = VarientSprites[index];
        }
    }

    public override void ResetToOriginal()
    {
        foreach (var item in allTiles)
        {
            item.sprite = originalSprite;
        }
    }

    public override void Init()
    {
        if (allTiles == null)
        {
            allTiles = transform.GetComponentsInChildren<SpriteRenderer>();
            originalSprite = allTiles[0].sprite;
        }
    }
}