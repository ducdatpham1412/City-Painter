using System.Collections.Generic;
using UnityEngine;

public class ScraperController : MonoBehaviour {
    [Header("GameObjects")]
    public SpriteRenderer WireFrame;
    [SerializeField] Transform Pointer;
    [SerializeField] Transform ParticlesContainer;

    [Header("Prefabs")]
    [SerializeField] GameObject ScraperParticlesPrefab;
    [SerializeField] Material ParticlesMaterial;

    [Header("Stats")]
    public Scraper currentScraper { get; private set; }

    SpriteRenderer Renderer;
    List<ParticleSystem> particlesPool = new();
    bool isScraping = false;
    int brushSize = 30;
    bool readyForVFX = true;

    void Awake() {
        Renderer = GetComponent<SpriteRenderer>();
    }

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
        ParticlesMaterial.SetTexture("_MainTex", scraper.particle);
    }

    public void SetWireFrame(Sprite _sprite) {
        WireFrame.sprite = _sprite;
    }

    void EnableVFX() {
        readyForVFX = true;
    }


    void ScrapeWireFrame(Vector2 localPos) {
        Vector2 pivot = WireFrame.sprite.pivot;
        int x = Mathf.RoundToInt(pivot.x + localPos.x * WireFrame.sprite.pixelsPerUnit);
        int y = Mathf.RoundToInt(pivot.y + localPos.y * WireFrame.sprite.pixelsPerUnit);

        bool hasScraped = false;

        for (int i = -brushSize; i <= brushSize; i++) {
            for (int j = -brushSize; j <= brushSize; j++) {
                int px = x + i;
                int py = y + j;
                if (px >= 0 && px < WireFrame.sprite.texture.width && py >= 0 && py < WireFrame.sprite.texture.height) {
                    Color color = WireFrame.sprite.texture.GetPixel(px, py);
                    if (color.a > 0f) {
                        hasScraped = true;
                        color.a = 0f;
                        WireFrame.sprite.texture.SetPixel(px, py, color);
                    }
                }
            }
        }

        if (hasScraped) {
            WireFrame.sprite.texture.Apply();

            // TODO: Playing sound

            if (readyForVFX) {
                readyForVFX = false;
                var freeParticle = particlesPool.Find(p => !p.isPlaying);
                if (freeParticle == null) {
                    ParticleSystem newParticles = Instantiate(ScraperParticlesPrefab, ParticlesContainer).GetComponent<ParticleSystem>();
                    newParticles.GetComponent<Renderer>().sharedMaterial = ParticlesMaterial;
                    newParticles.Play();
                    particlesPool.Add(newParticles);
                }
                else {
                    freeParticle.Play();
                }
                Invoke(nameof(EnableVFX), 1);
            }
        }
    }
}
