using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using static UnityEngine.UIElements.UxmlAttributeDescription;

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
            item.sprite = modified ? modifiedSprite : originalSprite;
        }
    }

    public override void SetModified(int index, bool userModify)
    {
        selectedIndex = index;
        modified = true;
        modifiedSprite = allTiles[0].sprite;

        if (effectType != EffectType.None && userModify)
        {
            if (effectMaterial == null)
            {
                effectMaterial = new Material(Shader.Find("Shader Graphs/" + effectType));
                foreach (var item in allTiles)
                {
                    item.material = effectMaterial;
                }
            }

            ShowEffect();
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
