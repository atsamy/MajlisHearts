using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UsernamePanel : MonoBehaviour
{
    public InputField NameInput;
    Action<string> onSubmit;
    [SerializeField]
    GameObject error;
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


        PlayfabManager.instance.SetDisplayName(NameInput.text, (result) =>
         {
             if (result)
             {
                 onSubmit?.Invoke(NameInput.text);
                 gameObject.SetActive(false);
             }
             else
             {
                 error.SetActive(false);
             }
         });
    }

    public void Close()
    {
        SFXManager.Instance.PlayClip("Close");
        error.SetActive(false);
    }
}
