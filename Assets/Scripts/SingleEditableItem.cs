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
        SetMaterial(userModify);

        base.SetModified(index, userModify);
        modifiedSprite = sprite.sprite;
    }

    private void SetMaterial(bool userModify)
    {
        if (effectType != EffectType.None && userModify)
        {
            if (effectMaterial == null)
            {
                if (effectType == EffectType.Glow)
                {
                    effectMaterial = new Material(Shader.Find("Shader Graphs/" + effectType));
                }
                else
                {
                    effectMaterial = new Material(Shader.Find("AllIn1SpriteShader/AllIn1Urp2dRenderer"));
                    var shape = MajlisScript.Instance.SparkleParticles.shape;
                    shape.spriteRenderer = sprite;
                    MajlisScript.Instance.SparkleParticles.Emit(15);
                }
                sprite.material = effectMaterial;
            }
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
        
        transform.DOJump(transform.position,0.2f,1, time * 2).OnComplete(() =>
        {
            sprite.sprite = varientSprites[index];
        });
    }
}
