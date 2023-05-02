using GoogleMobileAds.Api;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GoogleMobileAdsScript : MonoBehaviour
{

    public static GoogleMobileAdsScript Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
        // When true all events raised by GoogleMobileAds will be raised
        // on the Unity main thread. The default value is false.
        MobileAds.RaiseAdEventsOnUnityMainThread = true;

        // Initialize the Mobile Ads SDK.
        MobileAds.Initialize((initStatus) =>
        {
            Dictionary<string, AdapterStatus> map = initStatus.getAdapterStatusMap();
            foreach (KeyValuePair<string, AdapterStatus> keyValuePair in map)
            {
                string className = keyValuePair.Key;
                AdapterStatus status = keyValuePair.Value;
                switch (status.InitializationState)
                {
                    case AdapterState.NotReady:
                        // The adapter initialization did not complete.
                        print("Adapter: " + className + " not ready.");
                        break;
                    case AdapterState.Ready:
                        // The adapter was successfully initialized.
                        print("Adapter: " + className + " is initialized.");



                        break;
                }
            }
        });

        LoadRewardedAd();
    }

    // These ad units are configured to always serve test ads.
#if UNITY_ANDROID
    const string adUnitId = "ca-app-pub-4234006813055244/7294376287";
#elif UNITY_IPHONE
const string adUnitId = "ca-app-pub-3940256099942544/1712485313";
#else
const string adUnitId = "unused";
#endif

    private RewardedAd _rewardedAd;

    private void LoadRewardedAd()
    {
        // create our request used to load the ad.
        var adRequest = new AdRequest.Builder().Build();

        // Load a rewarded ad
        RewardedAd.Load(adUnitId, adRequest,
            (RewardedAd ad, LoadAdError loadError) =>
            {
                if (loadError != null)
                {
                    Debug.Log("Rewarded ad failed to load with error: " +
                               loadError.GetMessage());
                    return;
                }
                else if (ad == null)
                {
                    Debug.Log("Rewarded ad failed to load.");
                    return;
                }

                Debug.Log("Rewarded ad loaded.");
                _rewardedAd = ad;
            });

    }

    public void ShowRewardedAd(Action AdFinished)
    {
        if (_rewardedAd.CanShowAd())
        {
            _rewardedAd.Show((result) => 
            {
                AdFinished?.Invoke();
                //if(result.)
            });
        }
    }
}
