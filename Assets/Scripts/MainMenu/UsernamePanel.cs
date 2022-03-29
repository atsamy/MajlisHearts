using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UsernamePanel : MonoBehaviour
{
    public InputField NameInput;
    Action<string> onSubmit;
    // Start is called before the first frame update
    public void Show(Action<string> OnSubmit)
    {
        gameObject.SetActive(true);
        onSubmit = OnSubmit; 
    }

    public void Submit()
    {
        if (string.IsNullOrEmpty(NameInput.text))
            return;

        onSubmit?.Invoke(NameInput.text);
        PlayerPrefs.SetString("userName", NameInput.text);

        gameObject.SetActive(false);
    }
}
