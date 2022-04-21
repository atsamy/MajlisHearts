//using UnityEngine;
//using System.Collections;
////using GoogleMobileAds.Api;
//using UnityEngine.Advertisements;
//using System;

//public enum AdsNetwork { Unity, AdMob }

//public class AdsManager : MonoBehaviour, IUnityAdsListener
//{
//    //InterstitialAd interstitial;
//    public static AdsManager Instance;
//    //private RewardBasedVideoAd rewardBasedVideo;
//    Action<ShowResult> RewardedVideoCallback;
//    Action<ShowResult> InterstitialCallback;

//    public Action<bool> FinishedVideo;

//    //public bool VideoReady { get { return Advertisement.IsReady("rewardedVideo"); } }

//    public AdsNetwork[] VideoPriority;
//    public AdsNetwork[] InterstitialPriority;

//    public bool IsVideoReady
//    {
//        get
//        {
//            return Advertisement.IsReady("rewardedVideo");
//        }
//    }

//    private void Awake()
//    {
//        Instance = this;
//    }

//    // Use this for initialization
//    void Start()
//    {
//        //#if UNITY_ANDROID
//        //        string appId = "ca-app-pub-4234006813055244~3003294487";
//        //#elif UNITY_IPHONE
//        //            string appId = "ca-app-pub-3940256099942544~1458002511";
//        //#else
//        //            string appId = "unexpected_platform";
//        //#endif

//        //        // Initialize the Google Mobile Ads SDK.
//        //        MobileAds.Initialize(appId);

//        //        rewardBasedVideo = RewardBasedVideoAd.Instance;

//        //        rewardBasedVideo.OnAdRewarded += HandleRewardBasedVideoRewarded;

//        //        rewardBasedVideo.OnAdLoaded += RewardBasedVideo_OnAdLoaded;
//#if UNITY_ANDROID
//        Advertisement.Initialize("3921385");
//#elif UNITY_IOS
//        Advertisement.Initialize("3921384");
//#endif

//        Advertisement.AddListener(this);
//        //RequestInterstitial();

//        //requestVideo();
//    }

//    //    private void RequestInterstitial()
//    //    {
//    //#if UNITY_ANDROID
//    //        string adUnitId = "ca-app-pub-4234006813055244/9319035791";
//    //#elif UNITY_IPHONE
//    //        string adUnitId = "ca-app-pub-3940256099942544/4411468910";
//    //#else
//    //        string adUnitId = "unexpected_platform";
//    //#endif

//    //        // Initialize an InterstitialAd.
//    //        interstitial = new InterstitialAd(adUnitId);
//    //        // Create an empty ad request.
//    //        AdRequest request = new AdRequest.Builder().Build();
//    //        // Load the interstitial with the request.
//    //        interstitial.LoadAd(request);
//    //        interstitial.OnAdClosed += interstitialclosed;
//    //        interstitial.OnAdFailedToLoad += interstitialFailed;

//    //    }

//    //private void interstitialclosed(object sender, EventArgs e)
//    //{
//    //    InterstitialCallback.Invoke(ShowResult.Finished);
//    //}

//    //private void interstitialFailed(object sender, EventArgs e)
//    //{
//    //    InterstitialCallback.Invoke(ShowResult.Failed);
//    //}

//    //    private void requestVideo()
//    //    {

//    //#if UNITY_ANDROID
//    //        string adUnitId = "ca-app-pub-4234006813055244/9865830706";
//    //#elif UNITY_IPHONE
//    //            string adUnitId = "ca-app-pub-3940256099942544/1712485313";
//    //#else
//    //            string adUnitId = "unexpected_platform";
//    //#endif

//    //        // Create an empty ad request.
//    //        AdRequest request = new AdRequest.Builder().Build();
//    //        // Load the rewarded video ad with the request.
//    //        this.rewardBasedVideo.LoadAd(request, adUnitId);
//    //    }

//    //private void RewardBasedVideo_OnAdLoaded(object sender, EventArgs e)
//    //{
//    //    print("VideoLoaded");
//    //}

//    //public void ShowInterstitial(Action<ShowResult> HandleShowResult)
//    //{
//    //    bool adPlayed = false;

//    //    foreach (var item in InterstitialPriority)
//    //    {
//    //        if (item == AdsNetwork.Unity)
//    //        {
//    //            if (Advertisement.IsReady("video"))
//    //            {
//    //                var options = new ShowOptions { resultCallback = HandleShowResult };
//    //                Advertisement.Show("video", options);
//    //                adPlayed = true;
//    //                break;
//    //            }
//    //        }
//    //        //else if (item == AdsNetwork.AdMob)
//    //        //{
//    //        //    if (interstitial.IsLoaded())
//    //        //    {
//    //        //        interstitial.Show();
//    //        //        InterstitialCallback = HandleShowResult;
//    //        //        adPlayed = true;
//    //        //        break;
//    //        //    }
//    //        //}

//    //        if (!adPlayed)
//    //        {
//    //            HandleShowResult.Invoke(ShowResult.Failed);
//    //        }
//    //    }
//    //}

//    public void ShowRewardedAd(Action<bool> HandleShowResult)
//    {
//        //prin("unity ads" + Advertisement.IsReady());
//        //print("admob ads" + rewardBasedVideo.IsLoaded());

//        foreach (var item in VideoPriority)
//        {
//            if (item == AdsNetwork.Unity)
//            {
//                if (Advertisement.IsReady("rewardedVideo"))
//                {
//                    //var options = new ShowOptions { resultCallback = HandleShowResult };
//                    FinishedVideo = HandleShowResult;
//                    Advertisement.Show("rewardedVideo");
//                    break;
//                }
//            }
//            //else if (item == AdsNetwork.AdMob)
//            //{
//            //    if (rewardBasedVideo.IsLoaded())
//            //    {
//            //        rewardBasedVideo.Show();
//            //        RewardedVideoCallback = HandleShowResult;
//            //        break;
//            //    }
//            //}
//        }
//    }

//    //public void HandleRewardBasedVideoRewarded(object sender, Reward args)
//    //{
//    //    requestVideo();

//    //    string type = args.Type;
//    //    double amount = args.Amount;

//    //    print(
//    //        "HandleRewardBasedVideoRewarded event received for "
//    //                    + amount.ToString() + " " + type);

//    //    RewardedVideoCallback.Invoke(ShowResult.Finished);
//    //}

//    public void OnUnityAdsReady(string placementId)
//    {
//        
//        //VideoReady = true;
//    }

//    public void OnUnityAdsDidError(string message)
//    {
//        
//    }

//    public void OnUnityAdsDidStart(string placementId)
//    {
//        
//    }

//    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
//    {
//        

//        switch (showResult)
//        {
//            case ShowResult.Failed:

//                if (FinishedVideo != null)
//                    FinishedVideo.Invoke(false);

//                break;
//            case ShowResult.Skipped:

//                if (FinishedVideo != null)
//                    FinishedVideo.Invoke(false);

//                break;
//            case ShowResult.Finished:

//                if (FinishedVideo != null)
//                    FinishedVideo.Invoke(true);

//                break;
//            default:
//                break;
//        }
//    }
//}
