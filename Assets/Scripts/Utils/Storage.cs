using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public static class Storage {
    public enum Key {
        profile,
        gameState,
    }

    public static void SET(Key key, string value) {
        PlayerPrefs.SetString(key.ToString(), value);
        PlayerPrefs.Save();
    }

    public static T? GETStruct<T>(Key key) where T : struct {
        string res = PlayerPrefs.GetString(key.ToString());
        if (string.IsNullOrEmpty(res)) return null;

        try {
            return JsonConvert.DeserializeObject<T>(res);
        }
        catch (JsonException jsonEx) {
            Debug.LogWarning($"Deserialization failed: {jsonEx.Message}");
            return null;
        }
    }

    public static T GETRef<T>(Key key) where T : class {
        string res = PlayerPrefs.GetString(key.ToString());
        if (string.IsNullOrEmpty(res)) return null;

        try {
            return JsonConvert.DeserializeObject<T>(res);
        }
        catch (JsonException jsonEx) {
            Debug.LogWarning($"Deserialization failed: {jsonEx.Message}");
            return null;
        }
    }

    public static void DELETE(Key key) {
        PlayerPrefs.DeleteKey(key.ToString());
        PlayerPrefs.Save();
    }

    public static void SET_TEXTURE(string name, Texture2D texture) {
        string path = Application.persistentDataPath + name;
        byte[] pngData = texture.EncodeToPNG();
        File.WriteAllBytes(path, pngData);
    }
    public static Texture2D GET_TEXTURE(string name) {
        string path = Application.persistentDataPath + name;
        if (File.Exists(path)) {
            byte[] data = File.ReadAllBytes(path);
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(data);
            return tex;
        }
        return null;
    }
    public static void DELETE_TEXTURE(string name) {
        string path = Application.persistentDataPath + name;
        if (File.Exists(path)) {
            File.Delete(path);
        }
    }
}
