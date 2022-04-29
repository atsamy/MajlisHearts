using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectChair : MonoBehaviour
{
    public Action<int> OnSelected;
    public int Index;
    [SerializeField]
    SpriteRenderer player;
    private void OnMouseDown()
    {
        OnSelected?.Invoke(Index);
    }

    internal void SetPlayer(Sprite sprite)
    {
        player.sprite = sprite;
        player.gameObject.SetActive(true);
    }

    internal void Hide()
    {
        player.gameObject.SetActive(false);
    }
}
