using UnityEngine;
using System.Collections;
using UnityEngine.Advertisements;
using System;

public enum AdsNetwork { Unity, AdMob }

public class AdsManager : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener, IUnityAdsInitializationListener
{
    public static AdsManager Instance;
    Action<ShowResult> RewardedVideoCallback;
    Action<ShowResult> InterstitialCallback;

    public Action<bool> FinishedVideo;

    bool isAddLoaded;
    [SerializeField] 
    bool testMode = true;    
    [SerializeField] 
    string androidGameId;
    [SerializeField] 
    string iOSGameId;

    string _adUnitId = null;

    public bool IsVideoReady
    {
        get
        {
            return isAddLoaded;
        }
    }

    private void Awake()
    {
        Instance = this;
        string appId = "";

#if UNITY_ANDROID
        appId = androidGameId;
#elif UNITY_IPHONE
        appId = iOSGameId;
#endif

        Advertisement.Initialize(appId, testMode, this);
    }

    public void ShowRewardedAd(Action<bool> HandleShowResult)
    {
        if (isAddLoaded)
        {
            Advertisement.Show(_adUnitId, this);
            FinishedVideo = HandleShowResult;

            isAddLoaded = false;
        }
    }


    public void OnUnityAdsDidError(string message)
    {

    }

    public void OnUnityAdsDidStart(string placementId)
    {

    }

    public void OnUnityAdsAdLoaded(string placementId)
    {
        Debug.Log("add loaded");
        if (placementId.Equals(_adUnitId))
        {
            isAddLoaded = true;
        }
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        if (FinishedVideo != null)
            FinishedVideo.Invoke(false);

        LoadVideo();
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        if (FinishedVideo != null)
            FinishedVideo.Invoke(false);

        LoadVideo();
    }

    public void OnUnityAdsShowStart(string placementId)
    {

    }

    public void OnUnityAdsShowClick(string placementId)
    {

    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        if (FinishedVideo != null)
            FinishedVideo.Invoke(true);

        LoadVideo();
    }

    public void OnInitializationComplete()
    {
        LoadVideo();
    }

    private void LoadVideo()
    {
#if UNITY_ANDROID
        _adUnitId = "Rewarded_Android";
#elif UNITY_IOS
        _adUnitId = "Rewarded_iOS";
#endif

        Advertisement.Load(_adUnitId, this);
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log(message);
    }
}
