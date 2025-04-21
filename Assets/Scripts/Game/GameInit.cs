using UnityEngine;

public class GameInit : Singleton<GameInit> {
    public SpriteRenderer CityImage;

    void Start() {
        SetImage(CityImage.sprite);
    }

    void SetImage(Sprite sprite) {
        Vector2 pivot = sprite.pivot;
        Texture2D grayscaleTex = GenerateLineArt(Helper.SpriteToTexture(sprite));
        Rect rect = new Rect(0, 0, grayscaleTex.width, grayscaleTex.height);
        CityImage.sprite = Sprite.Create(grayscaleTex, rect, pivot);
    }

    Texture2D GenerateLineArt(Texture2D tex) {
        int width = tex.width;
        int height = tex.height;

        Color[] pixels = tex.GetPixels();
        Color[] output = new Color[pixels.Length];

        float[,] gray = new float[width, height];

        /*
        * Gray scale
        */
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                Color c = pixels[y * width + x];
                gray[x, y] = c.r * 0.299f + c.g * 0.587f + c.b * 0.114f;

                float g = gray[x, y];
                output[y * width + x] = new Color(g, g, g, c.a);
            }
        }

        /*
        * Wireframe line art
        */


        /*
        * Set result
        */
        Texture2D result = new Texture2D(width, height, TextureFormat.RGBA32, false);
        result.SetPixels(output);
        result.Apply();
        return result;
    }
}
