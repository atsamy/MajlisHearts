using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LanguageSetter : MonoBehaviour
{

    // Use this for initialization
    Language prevLanguage;
    bool started;

    public bool FixLines;
    public bool TextMeshPro;

    void Start()
    {
        SetText();
        started = true;
    }

    public void SetText()
    {
        if (!TextMeshPro)
        {
            Text text = GetComponent<Text>();
            if (FixLines && LanguageManager.Instance.CurrentLanguage == Language.Arabic)
            {
                ArabicLineFixer.Instance.FixLines(text, LanguageManager.Instance.GetRawString(name));
            }
            else
            {
                text.text = LanguageManager.Instance.GetString(name);
            }
            text.font = LanguageManager.Instance.GetFont();
        }
        else
        {
            TextMeshProUGUI text = GetComponent<TextMeshProUGUI>();
            text.text = LanguageManager.Instance.GetString(name);
            text.font = LanguageManager.Instance.GetTMPFont();
        }

        prevLanguage = LanguageManager.Instance.CurrentLanguage;
    }
    // Update is called once per frame

    void OnEnable()
    {
        if (started && prevLanguage != LanguageManager.Instance.CurrentLanguage)
            SetText();
    }
}
