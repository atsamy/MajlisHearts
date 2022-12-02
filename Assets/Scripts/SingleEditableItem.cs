using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SingleEditableItem : EditableItem
{
    SpriteRenderer sprite;
    protected Sprite originalSprite;
    protected Sprite modifiedSprite;
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

    public override void SetModified(int index,bool userModify)
    {
        SetEffect(userModify);
        modifiedSprite = sprite.sprite;

        base.SetModified(index, userModify);
    }

    public void SetEffect(bool userModify)
    {
        SetMaterial(userModify);
        ShowSparkles(userModify);
    }

    public void SetMaterial(bool userModify)
    {
        if (effectType != EffectType.None && userModify)
        {
            if (effectMaterial == null)
            {
                effectMaterial = new Material(MajlisScript.Instance.GlowMaterial);
                sprite.material = effectMaterial;
            }
        }

    }

    public void ShowSparkles(bool userModify)
    {
        if (userModify && effectType == EffectType.Sparkle)
        {
            var shape = MajlisScript.Instance.SparkleParticles.shape;
            shape.spriteRenderer = sprite;
            MajlisScript.Instance.SparkleParticles.Emit(15);
        }
    }

    public override void Reset()
    {
        sprite.sprite = modified?modifiedSprite:originalSprite;
    }

    public override void ChangeItem(int index,bool showEffect)
    {
        if (index >= varientSprites.Length)
            print(name);

        sprite.sprite = varientSprites[index];

        if (showEffect)
        {
            SetMaterial(true);
            ShowEffect();
            ShowSparkles(true);
        }
    }

    public override int GetVarientsCount()
    {
        return varientSprites.Length;
    }

    public override void ChangeItem(int index,float time)
    {
        if (disableAnimation)
        {
            sprite.sprite = varientSprites[index];
            return;
        }

        transform.DOMoveY(transform.position.y + 0.2f, time).SetLoops(2, LoopType.Yoyo).OnStepComplete(() =>
        {
            sprite.sprite = varientSprites[index];
        });
    }
}
