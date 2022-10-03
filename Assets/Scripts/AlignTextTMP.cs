using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AlignTextTMP : MonoBehaviour
{
    [SerializeField]
    TextAlignmentOptions textAnchor;

    void Start()
    {
        if (LanguageManager.Instance.CurrentLanguage == Language.Arabic)
        {
            GetComponent<TextMeshProUGUI>().alignment = textAnchor;
        }
    }
}