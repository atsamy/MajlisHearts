using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UsernamePanel : MonoBehaviour
{
    public InputField NameInput;
    // Start is called before the first frame update
    public void Submit()
    {
        if (string.IsNullOrEmpty(NameInput.text))
            return;

        PlayerPrefs.SetString("userName", NameInput.text);
        gameObject.SetActive(false);
    }
}
