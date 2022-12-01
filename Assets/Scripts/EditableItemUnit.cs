using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using PlayFab.ProfilesModels;
using UnityEngine;

public class EditableItemUnit : MonoBehaviour
{
    SpriteRenderer sprite;
    Sprite originalSprite;
    Sprite modifiedSprite;
    [SerializeField]
    Sprite[] varientSprites;
    bool modified;

    public int VarientCount => varientSprites.Length;

    private void Awake()
    {
        Init();
    }

    internal virtual void ResetToOriginal()
    {
        sprite.sprite = originalSprite;
    }

    internal virtual void Reset()
    {
        sprite.sprite = modified?modifiedSprite:originalSprite;
    }

    internal virtual void ChangeItem(int index)
    {
        if (index >= varientSprites.Length)
            return;

        sprite.sprite = varientSprites[index];
    }

    public void ChangeItem(int index, float time)
    {
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

    internal void SetModified(bool userModify,EffectType effectType)
    {
        modified = true;
        modifiedSprite = sprite.sprite;

        ShowParticles(userModify, effectType);
    }

    public void ShowParticles(bool userModify, EffectType effectType)
    {
        if (effectType != EffectType.None && userModify)
        {
            var shape = MajlisScript.Instance.SparkleParticles.shape;
            shape.spriteRenderer = sprite;
            MajlisScript.Instance.SparkleParticles.Emit(7);
        }
    }
}
