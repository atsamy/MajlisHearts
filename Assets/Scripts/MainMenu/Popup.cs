using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class Popup : MonoBehaviour
{
    public Action OKPressed;
    public Action ClosePressed;

    public Text Message;

    public Message[] Messages;

    public void ShowWithCode(string Code, Action OnOKPressed = null)
    {
        gameObject.SetActive(true);
        Message.text = Messages.Where(a => a.Code == Code).First().MessageText;

        OKPressed = OnOKPressed;
        ClosePressed = null;
    }

    public void ShowWithMessage(string message, Action OnOKPressed = null)
    {
        gameObject.SetActive(true);
        Message.text = message;

        OKPressed = OnOKPressed;
        ClosePressed = null;
    }

    public void ShowWithMessage(string message, Action OnOKPressed, Action OnClosePressed)
    {
        ShowWithMessage(message, OnOKPressed);
        ClosePressed = OnClosePressed;
    }

    public void Close()
    {
        if (ClosePressed != null)
            ClosePressed.Invoke();

        gameObject.SetActive(false);
    }

    public void OK()
    {
        if (OKPressed != null)
            OKPressed.Invoke();

        gameObject.SetActive(false);
    }

}

[Serializable]
public class Message
{
    public string Code;
    public string MessageText;
}
