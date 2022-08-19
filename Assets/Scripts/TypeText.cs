using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TypeText : MonoBehaviour
{
    TextMeshProUGUI text;
    string content;
    int index;

    AudioSource source;
    Action onDone;

    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        source = GetComponent<AudioSource>();
    }

    public void Play(string textString)
    {
        content = textString;
        text.text = "";
        index = 0;

        onDone = null;
        StartCoroutine(WriteText());
    }

    public void Play(string textString, Action done)
    {
        Play(textString);
        onDone = done;
    }

    public void Set(string textString)
    {
        index = content.Length;
        text.text = textString;
    }

    internal void Clear()
    {
        text.text = "";
    }

    public void Finish()
    {
        text.text = content;
        source.Stop();
        index = content.Length;
    }

    IEnumerator WriteText()
    {
        source.Play();
        while (index < content.Length)
        {
            index++;
            text.text = content.Substring(0, index);


            yield return new WaitForSeconds(0.02f);
        }
        source.Stop();

        if (onDone != null)
            onDone.Invoke();
    }
}