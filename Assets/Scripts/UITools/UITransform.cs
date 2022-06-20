//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using System;
//using UnityEngine.UI;
//public enum AnimationType { Linear, Curve, Swing }

//public class UITransform : UIAnimation
//{
//    public TransformElement[] Elements;

//    MaskableGraphic image;

//    float time;
//    float MaxTime;

//    public Action Finished;

//    bool fadeout;

//    bool forcedReverse;
//    float reverseTime;
//    public bool Active
//    {
//        get
//        {
//            return active;
//        }
//    }

//    // Use this for initialization
//    //void Start()
//    //{
//    //    if (AutoStart)
//    //    {
//    //        Play();
//    //        //invoke()
//    //    }
//    //}

//    public void Play(Action OnFinish)
//    {
//        //yield return new WaitForEndOfFrame();
//        this.Finished = OnFinish;
//        Play();
//    }

//    public override void Play()
//    {
//        Started = true;

//        Reset();
//        active = true;
//    }

//    protected override void StartPlaying()
//    {
//        Started = true;

//        time = 0;
//        active = true;
//    }

//    protected override void Reset()
//    {
//        if (GetComponent<MaskableGraphic>())
//            image = GetComponent<MaskableGraphic>();

//        time = 0;
//        MaxTime = 0;

//        foreach (TransformElement item in Elements)
//        {
            
//            float totalTime = item.Duration + item.StartAfter;

//            if (item.Reverse)
//                totalTime += item.ReverseAfter + item.Duration;

//            if (MaxTime < totalTime)
//                MaxTime = totalTime;

//            switch (item.Type)
//            {
//                case TransformType.Postion:
//                    transform.localPosition = new Vector3(item.StartValue.x * 1920, item.StartValue.y * 1080, 0);
//                    break;
//                case TransformType.Scale:
//                    transform.localScale = item.StartValue;
//                    break;
//                case TransformType.Rotation:
//                    transform.rotation = Quaternion.Euler(item.StartValue);
//                    break;
//                case TransformType.Color:
//                    if (image)
//                        image.color = item.StartValue;
//                    break;
//                case TransformType.Fill:
//                    if (image)
//                        ((Image)image).fillAmount = item.StartValue.x;
//                    break;
//            }
//            item.Finished = false;
//        }
//    }

//    public void Reverse(bool reversechildren = false)
//    {
//        float maxTime = 0;
//        forcedReverse = true;
//        foreach (TransformElement item in Elements)
//        {
//            item.Reverse = true;
//            item.ReverseAfter = 0;

//            if (maxTime < item.Duration)
//                maxTime = item.Duration;
//        }

//        Started = true;

//        MaxTime = time + maxTime;
//        //verseTime = mi

//        if (reversechildren)
//        {
//            UITransform[] childrenTrans = transform.GetComponentsInChildren<UITransform>();

//            foreach (var item in childrenTrans)
//            {
//                item.Reverse();
//            }
//        }
//    }

//    public void Reverse(Action OnFinish, bool reversechildren = false)
//    {
//        Reverse(reversechildren);
//        this.Finished = OnFinish;
//    }

//    private void OnDisable()
//    {
//        if (forcedReverse)
//        {
//            foreach (var item in Elements)
//            {
//                item.Reverse = false;
//            }

//            forcedReverse = false;
//            Finished = null;
//        }
//    }

//    public void FadeOut()
//    {
//        //Loop = false;
//        Started = false;
//        fadeout = true;
//        //Elements = new TransformElement[1];
//        //Elements[0] = new TransformElement
//        //{
//        //    AnimationType = AnimationType.Curve,
//        //    Type = TransformType.Color,
//        //    StartValue = image.color,
//        //    FinishValue = new Vector4(1, 1, 1, 0),
//        //    Duration = duration,

//        //};
//    }
//    // Update is called once per frame
//    new void Update()
//    {
//        if (fadeout)
//        {
//            Color color = image.color;
//            color.a -= Time.deltaTime * 2;

//            image.color = color;

//            if (color.a <= 0)
//                fadeout = false;
//        }

//        base.Update();

//        if (!Started)
//            return;

//        active = false;

//        foreach (TransformElement item in Elements)
//        {
//            switch (item.Type)
//            {
//                case TransformType.Postion:
//                    Vector4 resultVal;
//                    if (CalculateValue(item, out resultVal))
//                    {
//                        active = true;
//                        Vector3 modVal = new Vector3(resultVal.x * 1920, resultVal.y * 1080, 0);
//                        transform.localPosition = modVal;
//                    }

//                    break;
//                case TransformType.Scale:
//                    if (CalculateValue(item, out resultVal))
//                    {
//                        active = true;
//                        transform.localScale = resultVal;
//                    }

//                    break;
//                case TransformType.Rotation:
//                    if (CalculateValue(item, out resultVal))
//                    {
//                        active = true;
//                        transform.rotation = Quaternion.Euler(resultVal);
//                    }

//                    break;
//                case TransformType.Color:
//                    if (CalculateValue(item, out resultVal))
//                    {
//                        active = true;
//                        image.color = resultVal;
//                    }

//                    break;
//                case TransformType.Fill:
//                    if (CalculateValue(item, out resultVal))
//                    {
//                        active = true;
//                        ((Image)image).fillAmount = resultVal.x;
//                    }

//                    break;
//            }
//        }

//        if (Time.unscaledDeltaTime < 0.15f)
//            time += Time.unscaledDeltaTime;
//        //else
//        //    time += Time.deltaTime;

//        if (MaxTime < time)
//        {
//            if (Loop)
//            {
//                Play(); //StartCoroutine(playLoopAfter());// Play();
//            }
//            else
//            {
//                Started = false;

//                SetFinalValues();

//                if (Finished != null)
//                    Finished.Invoke();

//                if (forcedReverse)
//                {
//                    foreach (var item in Elements)
//                    {
//                        item.Reverse = false;
//                    }
//                    forcedReverse = false;
//                    Finished = null;
//                }

//                if (DisableAfterFinish)
//                    gameObject.SetActive(false);
//            }
//        }

//    }

//    //IEnumerator playLoopAfter()
//    //{
//    //    yield return new WaitForSeconds(LoopAfter);
//    //    Play();
//    //}

//    private bool CalculateValue(TransformElement item, out Vector4 returnValue)
//    {
//        returnValue = Vector3.zero;

//        if (time > item.StartAfter)
//        {
//            if ((time - item.StartAfter) / item.Duration < 1)
//            {
//                float currTime = (time - item.StartAfter) / item.Duration;

//                if (item.AnimationType == AnimationType.Curve)
//                {
//                    currTime = Mathf.Sin(currTime * Mathf.PI / 2);
//                }
//                else if (item.AnimationType == AnimationType.Swing)
//                {
//                    currTime = Mathf.Sin(currTime * Mathf.PI / 2) * (1 + Mathf.Sin(currTime * Mathf.PI) * 0.5f);
//                }

//                returnValue = Vector4.LerpUnclamped(item.StartValue, item.FinishValue, currTime);
//            }
//            else if (item.Reverse)
//            {
//                if (time < item.ReverseTime)
//                    returnValue = item.FinishValue;
//                else
//                {
//                    float currTime = (time - item.ReverseTime) / item.Duration;
//                    if (item.AnimationType == AnimationType.Curve)
//                    {
//                        currTime = Mathf.Sin(currTime * Mathf.PI / 2);
//                    }

//                    if (currTime < 1)
//                        returnValue = Vector4.Lerp(item.FinishValue, item.StartValue, currTime);
//                    else
//                    {
//                        returnValue = item.StartValue;

//                        if (!item.Finished)
//                        {
//                            item.Finished = true;
//                            return true;
//                        }
//                        return false;
//                    }
//                }
//            }
//            else
//            {
//                returnValue = item.FinishValue;
//                if (!item.Finished)
//                {
//                    item.Finished = true;
//                    return true;
//                }
//                return false;
//            }

//            return true;
//        }
//        returnValue = item.StartValue;
//        return false;
//    }

//    public void SaveValue(int Index)
//    {
//        if (Elements.Length == 0)
//            return;

//        if (GetComponent<MaskableGraphic>())
//            image = GetComponent<MaskableGraphic>();

//        switch (Elements[Elements.Length - 1].Type)
//        {
//            case TransformType.Postion:
//                Vector3 currPosition = transform.localPosition;

//                currPosition.x /= 1920;
//                currPosition.y /= 1080;

//                if (Index == 0)
//                    Elements[Elements.Length - 1].StartValue = currPosition;
//                else
//                    Elements[Elements.Length - 1].FinishValue = currPosition;


//                break;
//            case TransformType.Scale:

//                if (Index == 0)
//                    Elements[Elements.Length - 1].StartValue = transform.localScale;
//                else
//                    Elements[Elements.Length - 1].FinishValue = transform.localScale;

//                break;
//            case TransformType.Rotation:
//                if (Index == 0)
//                    Elements[Elements.Length - 1].StartValue = transform.eulerAngles;
//                else
//                    Elements[Elements.Length - 1].FinishValue = transform.eulerAngles;

//                break;
//            case TransformType.Color:
//                if (Index == 0)
//                {
//                    if (image)
//                        Elements[Elements.Length - 1].StartValue = image.color;
//                }
//                else
//                {
//                    if (image)
//                        Elements[Elements.Length - 1].FinishValue = image.color;
//                }

//                break;
//        }
//    }

//    public void ResetValues()
//    {
//        foreach (TransformElement item in Elements)
//        {
//            switch (item.Type)
//            {
//                case TransformType.Postion:
//                    transform.localPosition = new Vector3(item.StartValue.x * 1920, item.StartValue.y * 1080, 0);
//                    break;
//                case TransformType.Scale:
//                    transform.localScale = item.StartValue;
//                    break;
//                case TransformType.Rotation:
//                    transform.rotation = Quaternion.Euler(item.StartValue);
//                    break;
//                case TransformType.Color:
//                    if (image)
//                        image.color = item.StartValue;
//                    break;
//                case TransformType.Fill:
//                    if (image)
//                        ((Image)image).fillAmount = item.StartValue.x;
//                    break;
//            }
//        }
//    }

//    public void SetFinalValues()
//    {
//        foreach (TransformElement item in Elements)
//        {
//            switch (item.Type)
//            {
//                case TransformType.Postion:
//                    if (!item.Reverse)
//                        transform.localPosition = new Vector3(item.FinishValue.x * 1920, item.FinishValue.y * 1080, 0);
//                    else
//                        transform.localPosition = new Vector3(item.StartValue.x * 1920, item.StartValue.y * 1080, 0);
//                    break;
//                case TransformType.Scale:
//                    if (!item.Reverse)
//                        transform.localScale = item.FinishValue;
//                    else
//                        transform.localScale = item.StartValue;
//                    break;
//                case TransformType.Rotation:
//                    if (!item.Reverse)
//                        transform.rotation = Quaternion.Euler(item.FinishValue);
//                    else
//                        transform.rotation = Quaternion.Euler(item.StartValue);
//                    break;
//                case TransformType.Color:
//                    if (image)
//                    {
//                        if (!item.Reverse)
//                            image.color = item.FinishValue;
//                        else
//                            image.color = item.StartValue;
//                    }
//                    break;
//                case TransformType.Fill:
//                    if (image)
//                    {
//                        if (!item.Reverse)
//                            ((Image)image).fillAmount = item.FinishValue.x;
//                        else
//                            ((Image)image).fillAmount = item.StartValue.x;
//                    }
//                    break;
//            }
//        }
//    }
//}



//public enum TransformType { Postion, Scale, Rotation, Color, Fill }

//[Serializable]
//public class TransformElement
//{
//    public TransformType Type;
//    public AnimationType AnimationType;
//    public Vector4 StartValue;
//    public Vector4 FinishValue;
//    public float StartAfter;
//    public float Duration;
//    public bool Reverse;
//    public float ReverseAfter;
//    internal bool Finished;
//    internal float ReverseTime { get { return ReverseAfter + StartAfter + Duration; } }
//}
