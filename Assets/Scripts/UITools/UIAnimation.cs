//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public abstract class UIAnimation : MonoBehaviour
//{
//    public bool AutoStart = true;
//    public bool StartOnEnable;
//    public bool Loop;
//    //public float LoopAfter;
//    internal bool active;
//    internal bool Started;

//    public bool DisableAfterFinish;

//    float timer = 0;

//    bool start;

//    public float StartAnimationAfter;
//    public abstract void Play();

//    private void Start()
//    {
//        if (AutoStart)
//        {
//            Play();
//        }
//    }

//    private void OnEnable()
//    {
//        if (StartOnEnable)
//        {
//            Reset();
//            //StartCoroutine(StartAfter());
//            start = true;
//        }
//    }

//    protected virtual void Reset()
//    { }

//    protected virtual void StartPlaying()
//    { }

//    protected void Update()
//    {
//        if (start && Time.unscaledDeltaTime < 0.2f)
//        {
//            timer += Time.unscaledDeltaTime;

//            if (timer >= StartAnimationAfter)
//            {
//                StartPlaying();
//                start = false;
//            }
//        }
//    }
//}
