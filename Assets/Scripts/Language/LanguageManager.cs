using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml;
using ArabicSupport;
using System;
using TMPro;

public class LanguageManager : MonoBehaviour
{
    public LanguageFont[] fonts;

    private Hashtable Strings;
    public TextAsset LanguageFile;

    public static LanguageManager Instance;

    public string CurrentLanguage
    {
        get
        {
            string language = "English";

            if (Application.systemLanguage == SystemLanguage.Arabic)
            {
                language = "Arabic";
            }

            return PlayerPrefs.GetString("Language", language);
        }
        set { PlayerPrefs.SetString("Language", value); }
    }

    void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

        SetLanguage(CurrentLanguage);
    }

    public void SetLanguage(string language)
    {
        CurrentLanguage = language;

        XmlDocument xml = new XmlDocument();
        xml.LoadXml(LanguageFile.text);

        Strings = new Hashtable();
        var element = xml.DocumentElement[language];
        if (element != null)
        {
            var elemEnum = element.GetEnumerator();
            while (elemEnum.MoveNext())
            {
                if (elemEnum.Current is XmlElement)
                {
                    XmlElement xmlItem = (XmlElement)elemEnum.Current;
                    Strings.Add(xmlItem.GetAttribute("name"), xmlItem.InnerText);
                }
            }
        }
        else
        {
            Debug.LogError("The specified language does not exist: " + language);
        }
    }

    public void AddFile(TextAsset file)
    {
        XmlDocument xml = new XmlDocument();
        xml.LoadXml(file.text);

        var element = xml.DocumentElement[CurrentLanguage];
        if (element != null)
        {
            var elemEnum = element.GetEnumerator();
            while (elemEnum.MoveNext())
            {
                if (elemEnum.Current is XmlElement)
                {
                    XmlElement xmlItem = (XmlElement)elemEnum.Current;

                    if (Strings.ContainsKey(xmlItem.GetAttribute("name")))
                        break;

                    Strings.Add(xmlItem.GetAttribute("name"), xmlItem.InnerText);
                }
            }
        }
    }

    public string GetString(string name)
    {
        if (!Strings.ContainsKey(name))
        {
            Debug.LogError("The specified string does not exist: " + name);

            return "";
        }

        string text = (string)Strings[name];

        if (CurrentLanguage == "Arabic")
            text = ArabicFixer.Fix(text, false, false);

        return text;
    }

    public string GetRawString(string name)
    {
        if (!Strings.ContainsKey(name))
        {
            Debug.LogError("The specified string does not exist: " + name);

            return "";
        }

        return (string)Strings[name];
    }

    public Font GetFont()
    {
        for (int i = 0; i < fonts.Length; i++)
        {
            if (fonts[i].name == CurrentLanguage)
                return fonts[i].font;
        }

        return null;
    }

    public TMP_FontAsset GetTMPFont()
    {
        for (int i = 0; i < fonts.Length; i++)
        {
            if (fonts[i].name == CurrentLanguage)
                return fonts[i].TMP_font;
        }

        return null;
    }
}

[Serializable]
public struct LanguageFont
{
    public string name;
    public Font font;
    public TMP_FontAsset TMP_font;
}
