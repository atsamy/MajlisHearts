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

        SFXManager.Instance.PlayClip("Select");
    }

    public void ToggleNotification()
    {
        SFXManager.Instance.PlayClip("Toggle");

        bool isOn = (PlayerPrefs.GetInt("Notification", 1) != 1);

        PlayerPrefs.SetInt("Notification", isOn ? 1 : 0);

        notification.color = isOn ? Color.white : offColor;
    }
}
