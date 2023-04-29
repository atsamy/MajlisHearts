using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Threading.Tasks;

public class WordEffectScript : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI wordsText;

    [SerializeField]
    Image icon;

    [SerializeField]
    Sprite[] iconsprite;

    public async void ShowWord(string wordCode)
    {
        gameObject.SetActive(true);
        icon.gameObject.SetActive(false);
        wordsText.text = LanguageManager.Instance.GetString(wordCode);
        await Task.Delay(1000);
        gameObject.SetActive(false);
    }

    public async void ShowWordAndIcon(string wordCode,CardShape cardShape)
    {
        gameObject.SetActive(true);
        icon.gameObject.SetActive(true);
        icon.sprite = iconsprite[(int)cardShape];
        wordsText.text = LanguageManager.Instance.GetString(wordCode);
        await Task.Delay(1000);
        gameObject.SetActive(false);
    }

}
