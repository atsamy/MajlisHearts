using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LanguageSetterTMP : MonoBehaviour
{

    // Use this for initialization
    Language prevLanguage;
    bool started;

    void Start()
    {
        SetText();
        started = true;
    }

    public void SetText()
    {
        TextMeshProUGUI text = GetComponent<TextMeshProUGUI>();
        text.text = LanguageManager.Instance.GetString(name);
        //text.font = LanguageManager.Instance.GetFont();

        prevLanguage = LanguageManager.Instance.CurrentLanguage;
    }
    // Update is called once per frame

    void OnEnable()
    {
        if (started && prevLanguage != LanguageManager.Instance.CurrentLanguage)
            SetText();
    }
}
