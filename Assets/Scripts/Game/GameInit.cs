using UnityEngine;
using UnityEngine.Localization.Components;

public class GameInit : Singleton<GameInit> {
    public PanSprite CityImage;
    [SerializeField] LocalizeStringEvent CityName;

    public void InitCity(City city) {
        CityName.StringReference = city.name;
        CityName.RefreshString();
        CityImage.transform.position = Vector2.zero;
        CityImage.SetSprite(city.sprite);

        Vector2 pivot = new Vector2(
            city.sprite.pivot.x / city.sprite.rect.width,
            city.sprite.pivot.y / city.sprite.rect.height
        );
        Texture2D grayscaleTex = GenerateLineArt(city.sprite.texture);
        Rect rect = new Rect(0, 0, grayscaleTex.width, grayscaleTex.height);
        GameController.Instance.Scraper.SetWireFrame(Sprite.Create(texture: grayscaleTex, rect: rect, pivot: pivot, pixelsPerUnit: city.sprite.pixelsPerUnit));
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
        * Sobel edge detection
        */
        int[,] gx = new int[,] { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };
        int[,] gy = new int[,] { { 1, 2, 1 }, { 0, 0, 0 }, { -1, -2, -1 } };

        for (int y = 1; y < height - 1; y++) {
            for (int x = 1; x < width - 1; x++) {
                float sumX = 0;
                float sumY = 0;

                for (int i = -1; i <= 1; i++)
                    for (int j = -1; j <= 1; j++) {
                        float val = gray[x + j, y + i];
                        sumX += gx[i + 1, j + 1] * val;
                        sumY += gy[i + 1, j + 1] * val;
                    }

                float mag = Mathf.Sqrt(sumX * sumX + sumY * sumY);
                mag = Mathf.Clamp(mag * 0.5f, 0.1f, 0.4f);

                Color c = pixels[y * width + x];
                output[y * width + x] = new Color(mag, mag, mag, c.a);
            }
        }

        /*
        * Set result
        */
        Texture2D result = new Texture2D(width, height, TextureFormat.RGBA32, false);
        result.SetPixels(output);
        result.Apply();
        return result;
    }
}
