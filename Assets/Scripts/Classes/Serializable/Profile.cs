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
    public SoundManager.SF sound;
    public string cityUnlock;
}


[Serializable]
public class BackgroundSound {
    public string id;
    public Type type;
    public Sprite icon;
    public LocalizedString name;
    public SoundManager.SoundSource[] sources;

    public enum Type {
        background,
        sfx,
    }
}

[Serializable]
public class SfxSound : BackgroundSound {
    public float minInterval; // seconds
    public float maxInterval; // seconds
}