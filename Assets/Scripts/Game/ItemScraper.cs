using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class ItemScraper : MonoBehaviour {
    [SerializeField] Image Background;
    [SerializeField] Image Lock;
    [SerializeField] Image Icon;
    [SerializeField] LocalizeStringEvent Name;
    Scraper scraper;

    void Awake() {
        GameManager.Instance.ItemScrapers.Add(this);
    }

    public void SetScraper(Scraper _scraper) {
        scraper = _scraper;
        Icon.sprite = scraper.sprite;
        Name.StringReference = scraper.name;
        Name.RefreshString();
        UpdateEnable();
    }

    public void OnPress() {
        if (IsCurrentUsed()) return;
        if (IsUnlocked()) {
            GameController.Instance.ChangeScraper(scraper.id);
        }
        else {
            City city = GameManager.Instance.resources.cities.data.Find(c => c.id == scraper.cityUnlock);
            GameController.Instance.PlayCityToUnlock(city.name.GetLocalizedString());
        }
    }

    public void UpdateEnable() {
        if (IsUnlocked()) {
            Lock.gameObject.SetActive(false);
            bool isCurrentUsing = IsCurrentUsed();
            Background.enabled = isCurrentUsing;
        }
        else {
            Background.enabled = false;
            Lock.gameObject.SetActive(true);
        }
    }

    bool IsUnlocked() {
        var cities = GameManager.Instance.resources.cities.data;
        int index = cities.FindIndex(c => c.id == scraper.cityUnlock);
        int lastIndex = cities.FindIndex(c => c.id == GameManager.Instance.profile.lastCity);
        bool unlocked = index <= lastIndex;
        return unlocked;
    }

    bool IsCurrentUsed() {
        return scraper.id == GameManager.Instance.gameState.scraper;
    }
}
