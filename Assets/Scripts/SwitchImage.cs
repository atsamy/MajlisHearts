using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchImage : MonoBehaviour
{
    [SerializeField]
    GameObject[] Images;

    private void OnEnable()
    {
        for (int i = 0; i < 2; i++)
        {
            Images[i].SetActive((int)LanguageManager.Instance.CurrentLanguage == i);
        }
    }
}
