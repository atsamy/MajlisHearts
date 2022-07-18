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

    public Message[] Messages;

    void Show(Action OnOKPressed)
    {
        SFXManager.Instance.PlayClip("Popup");
        gameObject.SetActive(true);
        OKPressed = OnOKPressed;
        ClosePressed = null;
    }

    public void ShowWithCode(string Code, Action OnOKPressed = null)
    {
        Show(OnOKPressed);
        Message.text = Messages.Where(a => a.Code == Code).First().MessageText;
    }

    public void ShowWithMessage(string message, Action OnOKPressed = null)
    {
        Show(OnOKPressed);
        Message.text = message;
    }

    public void ShowWithMessage(string message, Action OnOKPressed, Action OnClosePressed)
    {
        Show(OnOKPressed);
        ShowWithMessage(message, OnOKPressed);
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
        OKPressed?.Invoke();
        gameObject.SetActive(false);

        SFXManager.Instance.PlayClip("OK");
    }

}

[Serializable]
public class Message
{
    public string Code;
    public string MessageText;
}
