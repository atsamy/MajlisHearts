using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArabicSupport;

public class FriendRequestPopup : Popup
{
    public void Show(string friendName,Action OnAccept,Action Ondeny)
    {
        ShowWithMessage(string.Format(LanguageManager.Instance.GetString("friendrquest"), ArabicFixer.Fix(friendName)),OnAccept,Ondeny);
    }
}
