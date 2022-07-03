using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MenuScene
{
    [SerializeField]
    Image notification;

    [SerializeField]
    Sprite[] notificationSprites;
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

        int isOn = PlayerPrefs.GetInt("Notification", 1);
        isOn = Mathf.Abs(isOn - 1);

        PlayerPrefs.SetInt("Notification", isOn);

        notification.sprite = notificationSprites[isOn];
    }
}
