using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArabicSupport;

public class FriendRequestPopup : Popup
{
    List<string> requests;

    public void Show(string friendName,Action OnAccept,Action Ondeny)
    {
        if (requests == null)
        {
            requests = new List<string>();
        }
        else
        {
            if (requests.Contains(friendName))
                return;
        }

        requests.Add(friendName);
        ShowWithMessage(string.Format(LanguageManager.Instance.GetString("friendrquest"), ArabicFixer.Fix(friendName)),OnAccept,Ondeny);
    }
}
