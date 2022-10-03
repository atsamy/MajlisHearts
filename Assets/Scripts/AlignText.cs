using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AlignText : MonoBehaviour
{
    [SerializeField]
    TextAnchor textAnchor;

    void Start()
    {
        if (LanguageManager.Instance.CurrentLanguage == Language.Arabic)
        {
            GetComponent<Text>().alignment = textAnchor;
        }
    }
}
