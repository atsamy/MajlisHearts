using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilableEditableItem : SingleEditableItem
{
    SpriteRenderer[] allTiles;

    public override void ChangeItem(int index)
    {
        //SetModified(index);
        foreach (var item in allTiles)
        {
            item.sprite = VarientSprites[index];
        }
    }

    public override void ChangeItem(int index, float time)
    {
        //SetModified(index);
        ChangeItem(index);
    }

    public override void ResetToOriginal()
    {
        foreach (var item in allTiles)
        {
            item.sprite = originalSprite;
        }
    }

    public override void Reset()
    {
        foreach (var item in allTiles)
        {
            item.sprite = modified?modifiedSprite:originalSprite;
        }
    }

    public override void SetModified(int index)
    {
        selectedIndex = index;
        modified = true;
        modifiedSprite = allTiles[0].sprite;
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
