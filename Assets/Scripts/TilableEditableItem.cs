using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class TilableEditableItem : SingleEditableItem
{
    SpriteRenderer[] allTiles;
    public override void ChangeItem(int index,bool showEffect)
    {
        //SetModified(index);
        foreach (var item in allTiles)
        {
            item.sprite = VarientSprites[index];
        }

        if (showEffect)
        {
            SetEffect(true);
        }
    }

    public override void ChangeItem(int index, float time)
    {
        //SetModified(index);
        ChangeItem(index,false);
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

        SetEffect(userModify);
    }

    private new void SetEffect(bool userModify)
    {
        if (effectType != EffectType.None && userModify)
        {
            if (effectMaterial == null)
            {
                effectMaterial = new Material(MajlisScript.Instance.GlowMaterial);
                foreach (var item in allTiles)
                {
                    item.material = effectMaterial;

                    if (effectType == EffectType.Sparkle)
                    {
                        var shape = MajlisScript.Instance.SparkleParticles.shape;
                        shape.spriteRenderer = item;
                        MajlisScript.Instance.SparkleParticles.Emit(15);
                    }
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
