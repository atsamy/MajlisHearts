using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTypePanel : MonoBehaviour
{
    public event Action<BalootGameType> OnGameTypeSelected;

    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void ChooseType(int type)
    {
        OnGameTypeSelected?.Invoke((BalootGameType)type);
        gameObject.SetActive(false);
    }
}

public enum BalootGameType
{
    Sun = 0,
    Hukom = 1,
    Ashkal = 2,
    Pass = 3
}
