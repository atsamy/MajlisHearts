using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsToggle : MonoBehaviour
{

    public string PrefsID;
    public Sprite OnState;
    public Sprite OffState;
    internal bool IsOn;

    public delegate void Toggled(bool state);
    public Toggled OnToggled;

    public Image Image;
    public Transform Knob;

    void Awake()
    {
        IsOn = PlayerPrefs.GetInt(PrefsID, 1) == 1;


        Image.sprite = IsOn ? OnState : OffState;
        Knob.localPosition = IsOn ? Vector3.right * 25 : Vector3.left * 25;
    }

    public void Toggle()
    {
        IsOn = !IsOn;
        PlayerPrefs.SetInt(PrefsID, IsOn ? 1 : 0);

        Image.sprite = IsOn ? OnState : OffState;
        Knob.localPosition = IsOn ? Vector3.right * 25 : Vector3.left * 25;

        if (OnToggled != null)
            OnToggled(IsOn);

        SFXManager.Instance.PlayClip("Toggle");
    }
}
