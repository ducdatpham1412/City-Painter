using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Profile {
    public string device_id;
    public int? localeID = null;
    public bool music;
    public bool sfx;
}


[Serializable]
public class Name {
    public string key;
    public string value;
}

[Serializable]
public class City {
    public string id;
    public List<Name> name;
    public Sprite sprite;


}

[Serializable]
public class Scraper {
    public string id;
    public List<Name> name;
    public SoundManager.SF sfx;
    public Sprite sprite;
    public Sprite sound;
}

