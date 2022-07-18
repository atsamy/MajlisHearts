using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml;
using ArabicSupport;
using System;


public enum Language
{
    English = 0,
    Arabic = 1
}

public class LanguageManager : MonoBehaviour
{
    public LanguageFont[] fonts;

    private Hashtable Strings;
    public TextAsset LanguageFile;

    public static LanguageManager Instance;

    Language currentLanguage;

    public Language CurrentLanguage
    {
        get
        {
            return currentLanguage;
            //if (Application.systemLanguage == SystemLanguage.Arabic)
            //{
            //    return Language.Arabic;
            //}

            //return (Language)PlayerPrefs.GetInt("Language", (int)Language.English);
        }
        set
        {
            currentLanguage = value;
            PlayerPrefs.SetInt("Language", (int)value);
        }
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

        currentLanguage = (Language)PlayerPrefs.GetInt("Language", 0);
        SetLanguage(currentLanguage);
    }

    public void SetLanguage(Language language)
    {
        CurrentLanguage = language;

        XmlDocument xml = new XmlDocument();
        xml.LoadXml(LanguageFile.text);

        Strings = new Hashtable();
        var element = xml.DocumentElement[language.ToString()];
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

        var element = xml.DocumentElement[CurrentLanguage.ToString()];
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

        if (CurrentLanguage == Language.Arabic)
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
            if (fonts[i].language == CurrentLanguage)
                return fonts[i].font;
        }

        return null;
    }

    //public TMP_FontAsset GetTMPFont()
    //{
    //    for (int i = 0; i < fonts.Length; i++)
    //    {
    //        if (fonts[i].language == CurrentLanguage)
    //            return fonts[i].TMP_font;
    //    }

    //    return null;
    //}
}

[Serializable]
public struct LanguageFont
{
    public Language language;
    public Font font;
}
