using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MenuScene
{
    [SerializeField]
    Image notification;
    [SerializeField]
    Color offColor;

    public void SetLanguage(string language)
    {
        LanguageManager.Instance.SetLanguage(language);

        LanguageSetter[] languageSetters = GameObject.FindObjectsOfType<LanguageSetter>();

        foreach (var item in languageSetters)
        {
            item.SetText();
        }
    }

    public void ToggleNotification()
    {
        notification.color = SFXManager.Instance.ToggleSFX() ? offColor : Color.white;
    }
}
