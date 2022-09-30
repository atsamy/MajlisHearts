using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MenuScene
{
    [SerializeField]
    TabScript[] tabScript;

    [SerializeField]
    Image notification;

    [SerializeField]
    Sprite[] notificationSprites;


    private void Start()
    {
        tabScript[0].Pressed((int)LanguageManager.Instance.CurrentLanguage == 0);
        tabScript[1].Pressed((int)LanguageManager.Instance.CurrentLanguage == 1);
    }

    public void SetLanguage(int languageIndex)
    {
        LanguageManager.Instance.SetLanguage((Language)languageIndex);
        LanguageSetterTMP[] languageSetters = FindObjectsOfType<LanguageSetterTMP>();

        foreach (var item in languageSetters)
        {
            item.SetText();
        }

        SFXManager.Instance.PlayClip("Select");

        tabScript[0].Pressed(languageIndex == 0);
        tabScript[1].Pressed(languageIndex == 1);
    }

    public void ToggleNotification()
    {
        SFXManager.Instance.PlayClip("Toggle");

        int isOn = PlayerPrefs.GetInt("Notification", 1);
        isOn = Mathf.Abs(isOn - 1);

        PlayerPrefs.SetInt("Notification", isOn);

        notification.sprite = notificationSprites[isOn];
    }

    public override void Close()
    {
        MenuManager.Instance.ShowMain();
        base.Close();
    }
}
