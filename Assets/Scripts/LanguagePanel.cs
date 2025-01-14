using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguagePanel : MonoBehaviour
{
    Action onDone;
    public void Open(Action Done)
    {
        onDone = Done;
        gameObject.SetActive(true);
    }

    public void SetEnglish()
    {
        LanguageManager.Instance.SetLanguage(Language.English);

        onDone?.Invoke();
        gameObject.SetActive(false);
    }

    public void SetArabic()
    {
        LanguageManager.Instance.SetLanguage(Language.Arabic);

        onDone?.Invoke();
        gameObject.SetActive(false);
    }
}
