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
    public AudioSource audioSfx;

    SpriteRenderer Renderer;
    List<ParticleSystem> particlesPool = new();
    bool isScraping = false;
    int brushSize = 25;
    bool readyForVFX = true;
    float minY = 0;
    float screenWorldWidth = 0;

    void Awake() {
        Renderer = GetComponent<SpriteRenderer>();
    }

    void Start() {
        UpdateScapeSound();
        float maxY = Camera.main.orthographicSize * Camera.main.aspect;
        minY = -maxY;
        screenWorldWidth = 2 * maxY;
    }

    void OnDisable() {
        isScraping = false;
    }

    void Update() {
        if (GameController.Instance.inZoomMode) return;

        if (!isScraping && GameHelper.TouchBegin()) {
            if (GameController.Instance.mode != GameController.Mode.scrape) return;
            isScraping = true;
        }

        if (isScraping) {
            if (GameHelper.TouchReleased()) {
                isScraping = false;
                if (audioSfx != null) {
                    audioSfx.Stop();
                }
                CheckEndGame();
                return;
            }
            if (GameHelper.TouchOverlayWorldGameObject()) {
                Vector2 touchPos = GameHelper.ToWorldPoint(GameHelper.TouchPosition());
                transform.position = touchPos;
                ScrapeWireFrame(
                    localPos: WireFrame.transform.InverseTransformPoint(Pointer.position),
                    worldPos: touchPos
                );
            }
        }
    }

    public void SetScraper(Scraper scraper) {
        currentScraper = scraper;
        Renderer.sprite = scraper.sprite;
        ParticlesMaterial.SetTexture("_MainTex", scraper.particle);
    }

    public void UpdateScapeSound() {
        ScrapeSound sound = GameManager.Instance.resources.scrapeSounds.data.Find(s => s.id == GameManager.Instance.gameState.scape_sound);

        if (sound != null) {
            if (audioSfx == null) {
                audioSfx = gameObject.AddComponent<AudioSource>();
                audioSfx.playOnAwake = false;
                audioSfx.loop = true;
                audioSfx.clip = sound.audioClip;
            }
            else {
                audioSfx.clip = sound.audioClip;
            }
        }
        else if (audioSfx != null) {
            Destroy(audioSfx);
        }
    }

    public void SetWireFrame(Sprite _sprite) {
        WireFrame.sprite = _sprite;
    }

    void EnableVFX() {
        readyForVFX = true;
    }

    void CheckEndGame() {
        if (GameController.Instance.ended) return;

        Color[] pixels = WireFrame.sprite.texture.GetPixels();
        int clearedPixelsCount = 0;

        foreach (Color px in pixels) {
            if (px.a < 0.1f) {
                clearedPixelsCount++;
            }
        }

        float ratio = (float)clearedPixelsCount / pixels.Length;
        // Debug.Log($"Ratio: {ratio} | {clearedPixelsCount}/{pixels.Length}");

        if (ratio >= 0.98f) {
            GameController.Instance.NoticeWinning();
        }
    }

    void ScrapeWireFrame(Vector2 localPos, Vector2 worldPos) {
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

            if (audioSfx != null) {
                audioSfx.panStereo = Mathf.Lerp(-1, 1, (worldPos.x - minY) / screenWorldWidth);
                if (!audioSfx.isPlaying) {
                    audioSfx.Play();
                }
            }


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
