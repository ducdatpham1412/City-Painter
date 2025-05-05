using System;
using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using UnityEngine;

public class GoogleAds : Singleton<GoogleAds> {
    BannerView bannerView;
    RewardedAd rewardedAd;
    InterstitialAd interstitialAd;
    Queue<Action> mainThreadQueue = new Queue<Action>();

    void Start() {
        MobileAds.Initialize((InitializationStatus status) => {
            Dictionary<string, AdapterStatus> map = status.getAdapterStatusMap();

            bool allReady = true;

            foreach (KeyValuePair<string, AdapterStatus> entry in map) {
                string adapterClass = entry.Key;
                AdapterStatus adapterStatus = entry.Value;

                Debug.Log($"[AdMob Init] Adapter: {adapterClass} | Status: {adapterStatus.InitializationState}");

                if (adapterStatus.InitializationState != AdapterState.Ready) {
                    allReady = false;
                    Debug.LogWarning($"{entry.Key} not ready: {adapterStatus.Description}");
                }
            }

            if (allReady) {
                // LoadInterstitialAd(); // Disable here because we init InterstitialAd when user coming app
                LoadRewardAd();
                ShowBanner();
            }
        });
    }

    void Update() {
        while (mainThreadQueue.Count > 0) {
            mainThreadQueue.Dequeue()?.Invoke();
        }
    }

    IEnumerator LoadBanner(int delay) {
        yield return new WaitForSeconds(delay);
        var adRequest = new AdRequest();
        bannerView.LoadAd(adRequest);
    }

    void ShowBanner() {
        if (bannerView == null) {
            CreateBannerView();
        }
        int retry = 0;
        void LoadRetry(LoadAdError err) {
            Debug.Log($"Load banner error: {err.GetMessage()}");
            if (retry <= 3) {
                retry++;
                mainThreadQueue.Enqueue(() => StartCoroutine(LoadBanner(2)));
            }
        }
        bannerView.OnBannerAdLoadFailed += LoadRetry;
        mainThreadQueue.Enqueue(() => StartCoroutine(LoadBanner(0)));
    }

    public void ShowInterstitial(Action success = null, Action error = null) {
        void ShowAndLoadForNext() {
            interstitialAd.OnAdFullScreenContentClosed += () => {
                success?.Invoke();
                LoadInterstitialAd();
            };
            interstitialAd.Show();
        }

        if (interstitialAd != null && interstitialAd.CanShowAd()) {
            ShowAndLoadForNext();
        }
        else {
            void Success() {
                if (interstitialAd != null && interstitialAd.CanShowAd()) {
                    ShowAndLoadForNext();
                }
            }
            LoadInterstitialAd(success: Success, error: error);
        }
    }

    public void ShowReward(Action success = null, Action error = null) {
        if (rewardedAd != null && rewardedAd.CanShowAd()) {
            rewardedAd.Show((Reward reward) => {
                success?.Invoke();
                LoadRewardAd();
            });
        }
        else {
            LoadRewardAd();
            error?.Invoke();
        }
    }

    public void Initialize() { }

    void LoadInterstitialAd(int retry = 0, Action success = null, Action error = null) {
        if (interstitialAd != null) {
            interstitialAd.Destroy();
            interstitialAd = null;
        }
        var adRequest = new AdRequest();
        InterstitialAd.Load(Configs.Env.INTERSTITIAL_ID, adRequest,
            (InterstitialAd ad, LoadAdError err) => {
                if (err != null || ad == null) {
                    Debug.LogError("Interstitial ad failed to load an ad " +
                                   "with error : " + err);
                    if (retry <= 3) {
                        //TODO: Make delay 2s
                        LoadInterstitialAd(retry + 1, success, error);
                    }
                    else {
                        error?.Invoke();
                    }
                    return;
                }

                Debug.Log("Interstitial ad loaded with response : "
                          + ad.GetResponseInfo());
                interstitialAd = ad;
                success?.Invoke();
            });
    }

    void LoadRewardAd(int retry = 0) {
        if (rewardedAd != null) {
            rewardedAd.Destroy();
            rewardedAd = null;
        }

        var adRequest = new AdRequest();

        RewardedAd.Load(Configs.Env.REWARD_ID, adRequest,
            (RewardedAd ad, LoadAdError error) => {
                if (error != null || ad == null) {
                    Debug.LogError("Rewarded ad failed to load an ad " +
                                   "with error : " + error);
                    if (retry <= 3) {
                        //TODO: Make delay 2s
                        LoadRewardAd(retry + 1);
                    }
                    return;
                }

                Debug.Log("Rewarded ad loaded with response : "
                          + ad.GetResponseInfo());
                rewardedAd = ad;
            });
    }

    void CreateBannerView() {
        if (bannerView != null) {
            bannerView.Destroy();
            bannerView = null;
        }
        // float scale = MobileAds.Utils.GetDeviceScale();
        // float width = scale == 0 ? Screen.width : Screen.width / scale;
        float width = MobileAds.Utils.GetDeviceSafeWidth() / 2;
        AdSize size = AdSize.Banner;
        if (width >= AdSize.Leaderboard.Width) {
            size = AdSize.Leaderboard;
        }
        else if (width >= AdSize.IABBanner.Width) {
            size = AdSize.IABBanner;
        }
        bannerView = new BannerView(Configs.Env.BANNER_ID, size, AdPosition.Top);
    }
}
