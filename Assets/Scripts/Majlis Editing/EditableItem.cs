using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditableItem : MonoBehaviour
{
    public string Code;
    SpriteRenderer sprite;
    Sprite originalSprite;
    [SerializeField]
    Sprite[] varientSprites;

    public Sprite[] VarientSprites { get => varientSprites; }

    int counter = 0;

    string room;
    //public Transform CameraLocation;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        originalSprite = sprite.sprite;
    }

    [HideInInspector]
    public string SelectedID;

    private void OnMouseDown()
    {
        if (counter == 1)
        {
            float itemPos = Camera.main.WorldToScreenPoint(transform.position).x / Screen.width;
            print(itemPos);
            //EditorUI.Instance.ShowItems(Code, itemPos);
        }
        else
            counter = 1;

        StartCoroutine(resetTimer());
    }

    IEnumerator resetTimer()
    {
        yield return new WaitForSeconds(0.5f);
        counter = 0;
    }

    internal void ResetToOriginal()
    {
        sprite.sprite = originalSprite;
    }

    internal void ChangeItem(int index)
    {
        sprite.sprite = varientSprites[index];
    }
}
