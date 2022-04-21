using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleButton : MonoBehaviour
{
    public Sprite[] IconSprites;
    public Image Icon;

    public Text State;

    public string Key;
    bool isOn;
    int value;

    public Action Toggled;
    // Start is called before the first frame update
    void Start()
    {
        SetValues();
    }

    private void SetValues()
    {
        value = PlayerPrefs.GetInt(Key, 1);
        isOn = value == 1;
        Icon.sprite = IconSprites[value];

        State.text = Key + " " + (value == 1 ? "ON" : "OFF");
    }

    public void Pressed()
    {
        //        SetValues();value = Mathf.Abs(value - 1);
        //PlayerPrefs.SetInt(Key, value);


        if (Toggled != null)
            Toggled.Invoke();

        SetValues();
    }
}
