using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

public class Utilities {
    [MenuItem("Tools/Get env")]
    public static void GetEnv() {
        TextAsset envText = Resources.Load<TextAsset>("env");
        var envDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(envText.text);
        foreach (var (key, value) in envDict) {
            Debug.Log($"{key} - {value}");
        }
    }

    [MenuItem("Tools/Set storage")]
    public static void Test() {
        var profile = Storage.GETRef<Profile>(Storage.Key.profile);
        profile.lastCity = "london";
        var gameState = Storage.GETRef<GameState>(Storage.Key.gameState);
        gameState.city = "london";
        Storage.SET(Storage.Key.profile, JsonConvert.SerializeObject(profile));
        Storage.SET(Storage.Key.gameState, JsonConvert.SerializeObject(gameState));
    }
}

