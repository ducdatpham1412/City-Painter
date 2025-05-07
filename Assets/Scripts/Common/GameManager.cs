using System.Collections.Generic;
using Newtonsoft.Json;
using Unity.Notifications.Android;
using UnityEngine;
using System;

public class GameManager : Singleton<GameManager> {
    protected GameManager() { }
    public GameState gameState;
    public Profile profile;
    public GameResources resources;
    public List<ItemSound> ItemSounds = new();
    public List<ItemScraper> ItemScrapers = new();
    public Dictionary<string, Sprite> citySprites = new();
    public bool firstTimeOpenApp = false;
    Sprite background;

    void Awake() {
        resources = new GameResources {
            backgroundSounds = Resources.Load<BackgroundSoundsObject>("Objects/SoundsBackground"),
            sfxSounds = Resources.Load<SfxSoundsObject>("Objects/SoundsSfx"),
            scrapeSounds = Resources.Load<ScrapeSoundsObject>("Objects/SoundsScrape"),
            scrapers = Resources.Load<ScrapersObject>("Objects/Scrapers"),
            cities = Resources.Load<CitiesObject>("Objects/Cities")
        };

        gameState = Storage.GETRef<GameState>(Storage.Key.gameState);
        gameState = gameState ?? new GameState {
            city = resources.cities.data[0].id,
            scraper = resources.scrapers.data[0].id,
            scape_sound = resources.scrapeSounds.data[0].id,
        };

        profile = Storage.GETRef<Profile>(Storage.Key.profile);
        if (profile == null) {
            firstTimeOpenApp = true;
            profile = new Profile {
                device_id = SystemInfo.deviceUniqueIdentifier,
                localeID = null,
                backgroundSounds = new List<string> { resources.backgroundSounds.data[0].id },
                sfxSounds = new List<string> { resources.sfxSounds.data[0].id },
                lastCity = resources.cities.data[0].id,
                items = new List<string>()
            };
            FirebaseTracking.Instance.UseScraper(resources.scrapers.data[0].id);
        }

        SoundManager.Instance.Initialize();
        FirebaseTracking.Instance.Initialize();

        if (profile.localeID != null) {
            LocalizationManager.Instance.SetLocale((int)profile.localeID);
        }
    }

    void Start() {
        if (Application.isEditor) {
            Application.targetFrameRate = 30;
        }
    }

    void OnApplicationQuit() {
        OnQuit();
    }

    void OnApplicationPause(bool pauseStatus) {
        if (pauseStatus) {
            OnQuit();
        }
    }

    public void OnQuit() {
        Storage.SET(Storage.Key.profile, JsonConvert.SerializeObject(profile));
        Storage.SET(Storage.Key.gameState, JsonConvert.SerializeObject(gameState));
        GameController.Instance.SaveCityWireFrame();
    }

    public Sprite GetBackground() {
        if (background != null) return background;
        Sprite[] bgSprites = Resources.LoadAll<Sprite>("Images/Background");
        background = bgSprites[UnityEngine.Random.Range(0, bgSprites.Length)];
        return background;
    }

    public bool IsCityUnlocked(string cityID) {
        var cities = resources.cities.data;
        int index = cities.FindIndex(c => c.id == cityID);
        int lastIndex = cities.FindIndex(c => c.id == profile.lastCity);
        bool unlocked = index <= lastIndex;
        return unlocked;
    }

    public bool IsLatestCity(string cityID) {
        return cityID == profile.lastCity;
    }

    public bool ShouldCityPlayAgain(string cityID) {
        bool isPlayed = IsCityUnlocked(cityID) && !IsLatestCity(cityID);
        return isPlayed && Storage.GET_TEXTURE(cityID) == null;
    }

    public void RescheduleDailyNotification() {
        int? notiID = Storage.GETStruct<int>(Storage.Key.dailyNoti);
        if (notiID != null) {
            AndroidNotificationCenter.CancelNotification((int)notiID);
            Storage.DELETE(Storage.Key.dailyNoti);
        }
        ScheduleDailyNotification();
    }

    void ScheduleDailyNotification() {
        if (Storage.GETStruct<int>(Storage.Key.dailyNoti) != null) return;

        var channel = new AndroidNotificationChannel() {
            Id = "daily_reminder",
            Name = "Daily Reminder",
            Importance = Importance.Default,
            Description = "Daily reminder notification",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);

        var notification = new AndroidNotification {
            Title = "City Painter 🧠 🧘‍♀️",
            Text = Helper.GetLocalizedValue("timeToRelaxYourMind"),
            FireTime = DateTime.Today.AddHours(20),
            RepeatInterval = TimeSpan.FromDays(1),
        };

        if (notification.FireTime <= DateTime.Now)
            notification.FireTime = notification.FireTime.AddDays(1);

        int notiID = AndroidNotificationCenter.SendNotification(notification, channel.Id);

        Storage.SET(Storage.Key.dailyNoti, notiID.ToString());
    }

    [Serializable]
    public class GameResources {
        public BackgroundSoundsObject backgroundSounds;
        public SfxSoundsObject sfxSounds;
        public ScrapeSoundsObject scrapeSounds;
        public ScrapersObject scrapers;
        public CitiesObject cities;
    }
}

