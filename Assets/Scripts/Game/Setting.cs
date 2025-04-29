using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Setting : MonoBehaviour {
    [Header("GameObjects")]
    [SerializeField] List<LanguageButton> LanguageButtons = new List<LanguageButton>();
    [SerializeField] RectTransform BgSoundContainer;
    [SerializeField] RectTransform SfxSoundContainer;
    [SerializeField] RectTransform ScrapersContainer;
    [SerializeField] RectTransform ScrapeSoundContainer;
    [SerializeField] RectTransform Content;

    [Header("Prefabs")]
    [SerializeField] GameObject ItemSoundPrefab;
    [SerializeField] GameObject ItemScraperPrefab;

    void Awake() {
        SetLocale(GameManager.Instance.profile.localeID ?? 0);
    }

    void Start() {
        foreach (BackgroundSound sound in GameManager.Instance.resources.backgroundSounds.data) {
            ItemSound itemSound = Instantiate(ItemSoundPrefab, BgSoundContainer).GetComponent<ItemSound>();
            itemSound.SetSound(sound);
        }
        foreach (SfxSound sound in GameManager.Instance.resources.sfxSounds.data) {
            ItemSound itemSound = Instantiate(ItemSoundPrefab, SfxSoundContainer).GetComponent<ItemSound>();
            itemSound.SetSound(sound);
        }
        foreach (Scraper scraper in GameManager.Instance.resources.scrapers.data) {
            ItemScraper itemScraper = Instantiate(ItemScraperPrefab, ScrapersContainer).GetComponent<ItemScraper>();
            itemScraper.SetScraper(scraper);
        }
        foreach (ScrapeSound sound in GameManager.Instance.resources.scrapeSounds.data) {
            ItemSound itemSound = Instantiate(ItemSoundPrefab, ScrapeSoundContainer).GetComponent<ItemSound>();
            itemSound.SetSound(sound);
            StartCoroutine(RebuildLayout());
        }
    }

    public void SetLocale(int localeID) {
        Helper.Haptic();
        LocalizationManager.Instance.SetLocale(localeID);
        GameManager.Instance.profile.localeID = localeID;
        foreach (var lan in LanguageButtons) {
            if (lan.Id == localeID) {
                lan.Image.color = Helper.ColorFromHex(Configs.Color.yellow);
            }
            else {
                lan.Image.color = Color.white;
            }
        }
    }

    IEnumerator RebuildLayout() {
        yield return null;
        LayoutRebuilder.ForceRebuildLayoutImmediate(Content);
    }

    [Serializable]
    public class LanguageButton {
        public int Id;
        public Image Image;
    }
}
