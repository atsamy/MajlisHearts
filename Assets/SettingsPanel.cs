using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsPanel : MenuScene
{
    public void SetLanguage(string language)
    {
        LanguageManager.Instance.SetLanguage(language);

        LanguageSetter[] languageSetters = GameObject.FindObjectsOfType<LanguageSetter>();

        foreach (var item in languageSetters)
        {
            item.SetText();
        }
    }
}
