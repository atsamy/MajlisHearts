using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InvitePopup : Popup
{
    [SerializeField]
    TextMeshProUGUI Cost;

    public void Show(string message,int cost,Action OKPressed)
    {
        ShowWithMessage(message, OKPressed);
        this.Cost.text = cost.ToString();
    }
}
