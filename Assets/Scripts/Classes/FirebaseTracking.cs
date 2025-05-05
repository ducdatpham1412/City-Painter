using System;
using Firebase;
using Firebase.Analytics;
using Firebase.Extensions;
using UnityEngine;

public class FirebaseTracking : Singleton<FirebaseTracking> {
    static bool isReady = false;
    static Action action;

    public void Initialize() {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread((task) => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available) {
                FirebaseApp app = FirebaseApp.DefaultInstance;
                FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
                isReady = true;
                OpenApp();
                action?.Invoke();
                action = null;
                Debug.Log("Firebase initialized successfully ✅");
            }
            else {
                Debug.LogWarning($"Could not resolve all Firebase dependencies {dependencyStatus}");
            }
        });
    }

    void OpenApp() {
        FirebaseAnalytics.LogEvent("custom_app_open", new Parameter[] {
            new Parameter("device_id", GameManager.Instance.profile.device_id),
            new Parameter("version", Application.version),
            new Parameter("ts", Helper.TimeStamp().ToString())
        });
    }

    public void OpenCity(string cityID) {
        CheckLog(() => {
            FirebaseAnalytics.LogEvent("open_city", new Parameter[] {
                new Parameter("city_id", cityID),
                new Parameter("device_id", GameManager.Instance.profile.device_id),
                new Parameter("version", Application.version),
                new Parameter("ts",  Helper.TimeStamp().ToString())
            });
        });
    }

    public void FinishCity(string cityID) {
        CheckLog(() => {
            FirebaseAnalytics.LogEvent("finish_city", new Parameter[] {
                new Parameter("city_id", cityID),
                new Parameter("device_id", GameManager.Instance.profile.device_id),
                new Parameter("version", Application.version),
                new Parameter("ts",  Helper.TimeStamp().ToString())
            });
        });
    }

    public void UseScraper(string scraperID) {
        CheckLog(() => {
            FirebaseAnalytics.LogEvent("use_scraper", new Parameter[] {
                new Parameter("scraper_id", scraperID),
                new Parameter("device_id", GameManager.Instance.profile.device_id),
                new Parameter("version", Application.version),
                new Parameter("ts", Helper.TimeStamp().ToString())
            });
        });
    }

    void CheckLog(Action call) {
        if (isReady) {
            call.Invoke();
        }
        else {
            action += call;
        }
    }
}