using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using TMPro;

public class Popup : MonoBehaviour
{
    public Action OKPressed;
    public Action ClosePressed;

    public TextMeshProUGUI Message;

    void Show(Action OnOKPressed = null,Action OnCanelPressed = null)
    {
        gameObject.SetActive(true);
        OKPressed = OnOKPressed;
        ClosePressed = OnCanelPressed;

        if (LanguageManager.Instance.CurrentLanguage == Language.Arabic)
        {
            Message.lineSpacing = -330;
        }
        else
        {
            Message.lineSpacing = 0;
        }
    }

    public void ShowWithMessage(string message, Action OnOKPressed = null, Action OnClosePressed = null)
    {
        Show(OnOKPressed,OnClosePressed);
        Message.text = message;
    }

    public void ShowWithCode(string code, Action OnOKPressed = null, Action OnClosePressed = null)
    {
        Show(OnOKPressed, OnClosePressed);
        Message.text = LanguageManager.Instance.GetString(code);
    }

    public void Close()
    {
        ClosePressed?.Invoke();
        gameObject.SetActive(false);
    }

    public void OK()
    {
        gameObject.SetActive(false);
        OKPressed?.Invoke();
    }

}

//[Serializable]
//public class Message
//{
//    public string Code;
//    public string MessageText;
//}
