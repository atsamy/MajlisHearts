using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TilableEditableItem : SingleEditableItem
{
    SpriteRenderer[] allTiles;
    [SerializeField]
    Material floorMaterial;
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

        if (floorMaterial != null)
            StartCoroutine(ShowEffect());
            //floorMaterial?.DOFloat(0.5f, "_Intensity",0.25f).SetDelay(1).SetLoops(2,LoopType.Yoyo);//.SetFloat("_Intensity",).do
    }

    public override void Init()
    {
        if (allTiles == null)
        {
            allTiles = transform.GetComponentsInChildren<SpriteRenderer>();
            originalSprite = allTiles[0].sprite;
        }
    }

    IEnumerator ShowEffect()
    {
        float timer = 0;
        while (timer < 1)
        {
            floorMaterial.SetFloat("_Intensity",timer / 3);
            timer += Time.deltaTime * 4;
            yield return null;
        }

        yield return new WaitForSeconds(1.5f);

        while (timer > 0)
        {
            floorMaterial.SetFloat("_Intensity", timer / 3);
            timer -= Time.deltaTime * 4;
            yield return null;
        }

        floorMaterial.SetFloat("_Intensity", 0);
    }
}
