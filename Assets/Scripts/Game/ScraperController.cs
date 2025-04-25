using UnityEngine;

public class ScraperController : MonoBehaviour {
    public Scraper currentScraper;
    [SerializeField] SpriteRenderer Renderer;
    public SpriteRenderer WireFrame;
    [SerializeField] Transform Pointer;

    bool isScraping = false;
    int brushSize = 20;

    void OnDisable() {
        isScraping = false;
    }

    void Update() {
        if (!isScraping && GameHelper.TouchBegin()) {
            if (GameController.Instance.mode != GameController.Mode.scrape) return;
            // Vector3 _touch = GameHelper.TouchPosition();
            // isScraping = GameHelper.TouchHitGameObject(_touch, gameObject);
            isScraping = true;
        }

        if (isScraping) {
            if (GameHelper.TouchReleased()) {
                isScraping = false;
                foreach (Color px in WireFrame.sprite.texture.GetPixels()) {
                    if (px.a > 0) return;
                }
                // Notice wining
                GameController.Instance.NoticeWinning();
            }
            if (GameHelper.TouchOverlayWorldGameObject()) {
                Vector2 touchPos = GameHelper.ToWorldPoint(GameHelper.TouchPosition());
                transform.position = touchPos;
                ScrapeWireFrame(WireFrame.transform.InverseTransformPoint(Pointer.position));
            }
        }
    }

    public void SetScraper(Scraper scraper) {
        currentScraper = scraper;
        Renderer.sprite = scraper.sprite;
    }

    public void SetWireFrame(Sprite _sprite) {
        WireFrame.sprite = _sprite;
    }

    void ScrapeWireFrame(Vector2 localPos) {
        Vector2 pivot = WireFrame.sprite.pivot;
        int x = Mathf.RoundToInt(pivot.x + localPos.x * WireFrame.sprite.pixelsPerUnit);
        int y = Mathf.RoundToInt(pivot.y + localPos.y * WireFrame.sprite.pixelsPerUnit);

        for (int i = -brushSize; i <= brushSize; i++) {
            for (int j = -brushSize; j <= brushSize; j++) {
                int px = x + i;
                int py = y + j;
                if (px >= 0 && px < WireFrame.sprite.texture.width && py >= 0 && py < WireFrame.sprite.texture.height) {
                    Color color = WireFrame.sprite.texture.GetPixel(px, py);
                    if (color.a > 0f) {
                        color.a = 0f;
                        WireFrame.sprite.texture.SetPixel(px, py, color);
                    }
                }
            }
        }

        WireFrame.sprite.texture.Apply();
    }
}
