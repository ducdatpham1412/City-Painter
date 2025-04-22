using Newtonsoft.Json;
using UnityEngine;

public class GameManager : Singleton<GameManager> {
    protected GameManager() { }
    public GameState gameState;
    public Profile profile;

    Sprite background;

    void Awake() {
        gameState = Storage.GETRef<GameState>(Storage.Key.gameState);
        gameState = gameState ?? new GameState { };

        profile = Storage.GETRef<Profile>(Storage.Key.profile);
        profile = profile ?? new Profile {
            device_id = SystemInfo.deviceUniqueIdentifier,
            localeID = null,
            music = true,
            sfx = true,
        };

        SoundManager.Instance.Initialize();
        FirebaseTracking.Instance.Initialize();
        if (profile.music) {
            SoundManager.Instance.PlayMusic(SoundManager.MusicSource.Kid);
        }
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

    public void Initialize() { }

    public void OnQuit() {
        Storage.SET(Storage.Key.profile, JsonConvert.SerializeObject(profile));
        Storage.SET(Storage.Key.gameState, JsonConvert.SerializeObject(gameState));
    }

    public Sprite GetBackground() {
        if (background != null) return background;
        Sprite[] bgSprites = Resources.LoadAll<Sprite>("Images/Background");
        background = bgSprites[Random.Range(0, bgSprites.Length)];
        return background;
    }
}

