using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[Serializable]
public class Profile {
    public string device_id;
    public int? localeID = null;
    public List<string> backgroundSounds;
    public List<string> sfxSounds;
    public string lastCity;
}

[Serializable]
public class City {
    public string id;
    public LocalizedString name;
    public Sprite sprite;
}

[Serializable]
public class Scraper {
    public string id;
    public LocalizedString name;
    public SoundManager.SF sfx;
    public Sprite sprite;
    public Texture2D particle;
    public string cityUnlock;
}


[Serializable]
public class BaseSound {
    public string id;
    public Type type;
    public Sprite icon;
    public LocalizedString name;

    public enum Type {
        background,
        sfx,
    }
}

[Serializable]
public class BackgroundSound : BaseSound {
    public SoundManager.SoundSource[] sources;
}

[Serializable]
public class SfxSound : BaseSound {
    public SoundManager.SF[] sources;
    public float minInterval; // seconds
    public float maxInterval; // seconds
}