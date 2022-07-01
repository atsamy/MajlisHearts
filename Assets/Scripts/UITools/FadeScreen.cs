using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum FadeType { FadeIn, FadeOut, flash }
public class FadeScreen : MonoBehaviour
{

    Image image;
    float timer;
    float multiplier;
    //FadeController controller;
    public static FadeScreen Instance;


    FadeType currentType;
    internal bool active;

    public delegate void FadeComplete();
    public FadeComplete OnFadeComplete;

    bool once;
    // Use this for initialization
    void Awake()
    {
        image = GetComponent<Image>();
        Instance = this;
    }

    public void FadeIn(float speed, FadeComplete OnComplete)
    {

        if (!active)
        {
            OnFadeComplete = OnComplete;
            Enable(speed, FadeType.FadeIn, new Color(0, 0, 0, 0));
        }
        else
        {
            if (OnComplete != null)
                OnComplete();
        }
    }

    public void FadeIn(float speed, FadeComplete OnComplete, Color startColor)
    {
        image.sprite = null;

        if (!active)
        {
            OnFadeComplete = OnComplete;
            Enable(speed, FadeType.FadeIn, startColor);
        }
        else
        {
            if (OnComplete != null)
                OnComplete();
        }
    }

    public void FadeOut(float speed, FadeComplete OnComplete)
    {
        OnFadeComplete = OnComplete;
        Enable(speed, FadeType.FadeOut);
    }

    public void FadeOut(float speed)
    {
        Enable(speed, FadeType.FadeOut);
        OnFadeComplete = null;
    }

    public void FadeOut(float speed, bool splash)
    {
        Enable(speed, FadeType.FadeOut, splash ? Color.white : new Color(0, 0, 0, 1));
        OnFadeComplete = null;
    }
    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            if (Time.unscaledDeltaTime > 0.2)
                return;

            timer += Time.unscaledDeltaTime * (Mathf.PI / 2) * multiplier;



            if (currentType == FadeType.flash && timer > Math.PI / 2 && !once)
            {
                if (OnFadeComplete != null)
                    OnFadeComplete();

                once = true;
            }

            Color color = image.color;
            color.a = currentType == FadeType.FadeOut ? Mathf.Cos(timer) : Mathf.Sin(timer);
            image.color = color;

            float finishTime = currentType == FadeType.flash ? Mathf.PI : Mathf.PI / 2;

            if (timer > finishTime)
            {
                active = false;

                Color finalcolor = image.color;
                finalcolor.a = currentType == FadeType.FadeIn ? 1 : 0;
                image.color = finalcolor;

                if (currentType != FadeType.flash)
                {
                    if (OnFadeComplete != null)
                        OnFadeComplete();
                }

                if (currentType == FadeType.FadeOut || currentType == FadeType.flash)
                {
                    image.enabled = false;
                }
            }

        }
    }

    public void Enable(float speed, FadeType type, Color startColor)
    {
        Enable(speed, type);
        image.color = startColor;
    }

    public void Enable(float speed, FadeType type)
    {
        if (!image.enabled)
            image.enabled = true;

        multiplier = speed;
        timer = 0;
        active = true;

        currentType = type;
    }

    internal void Flash(float speed, FadeComplete OnComplete)
    {
        Flash(speed, new Color(1, 1, 1, 0), OnComplete);
    }

    internal void Flash(float speed, Color color, FadeComplete OnComplete)
    {
        once = false;

        OnFadeComplete = OnComplete;
        Enable(speed, FadeType.flash, color);
    }
}
