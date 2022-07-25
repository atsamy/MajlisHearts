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

    [SerializeField]
    CharacterSelectTab[] characterSelectTabs;

    int selectedIndex;

    public void Show(Action<string> OnSubmit)
    {
        gameObject.SetActive(true);
        onSubmit = OnSubmit; 
    }

    public void Submit()
    {
        if (string.IsNullOrEmpty(NameInput.text))
            return;

        GameManager.Instance.SaveAvatar("Avatar" + selectedIndex);

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
        gameObject.SetActive(false);
    }

    public void ChooseCharacters(int index)
    {
        for (int i = 0; i < characterSelectTabs.Length; i++)
        {
            characterSelectTabs[i].Pressed(i == index);
        }

        selectedIndex = index;
    }
}
