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

    [MenuItem("Tools/Test")]
    public static void Test() {
    }
}

