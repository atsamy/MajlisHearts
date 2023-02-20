using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectShapePanel : MonoBehaviour
{
    [SerializeField]
    public Button[] shapeButtons;

    public event Action<CardShape> OnShapeSelected;
    public void Show(CardShape hukomShape)
    {
        for (int i = 0; i < shapeButtons.Length; i++)
        {
            shapeButtons[i].interactable = (int)hukomShape != i;
        }

        gameObject.SetActive(true);
    }

    public void SelectShape(int index)
    {
        OnShapeSelected?.Invoke((CardShape)index);
        gameObject.SetActive(false);
    }

}
