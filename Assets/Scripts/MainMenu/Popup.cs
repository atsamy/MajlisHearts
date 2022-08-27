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

    public Text Message;

    //public Message[] Messages;

    void Show(Action OnOKPressed)
    {
        SFXManager.Instance.PlayClip("Popup");
        gameObject.SetActive(true);
        OKPressed = OnOKPressed;
        ClosePressed = null;
    }

    public void ShowWithCode(string code, Action OnOKPressed = null)
    {
        Show(OnOKPressed);
        Message.text = LanguageManager.Instance.GetString(code);
        Message.font = LanguageManager.Instance.GetFont();
    }

    public void ShowWithMessage(string message, Action OnOKPressed = null)
    {
        Show(OnOKPressed);
        Message.text = message;
    }

    public void ShowWithMessage(string message, Action OnOKPressed, Action OnClosePressed)
    {
        ShowWithMessage(message, OnOKPressed);
        ClosePressed = OnClosePressed;
    }

    public void ShowWithCode(string message, Action OnOKPressed, Action OnClosePressed)
    {
        ShowWithCode(message, OnOKPressed);
        ClosePressed = OnClosePressed;
    }

    public void Close()
    {
        ClosePressed?.Invoke();
        gameObject.SetActive(false);

        SFXManager.Instance.PlayClip("Close");
    }

    public void OK()
    {
        gameObject.SetActive(false);
        OKPressed?.Invoke();
        SFXManager.Instance.PlayClip("OK");
    }

}

//[Serializable]
//public class Message
//{
//    public string Code;
//    public string MessageText;
//}
