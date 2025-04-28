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
        if (GameManager.Instance.IsCityUnlocked(scraper.cityUnlock)) {
            GameController.Instance.ChangeScraper(scraper.id);
        }
        else {
            City city = GameManager.Instance.resources.cities.data.Find(c => c.id == scraper.cityUnlock);
            GameController.Instance.InfoDialog.Open(new InfoDialog.Info {
                title = Helper.GetLocalizedValue("reachCityOpenScraper", new string[] { city.name.GetLocalizedString() }),
                OnClick = () => GameController.Instance.InfoDialog.Close(),
            });
        }
    }

    public void UpdateEnable() {
        if (GameManager.Instance.IsCityUnlocked(scraper.cityUnlock)) {
            Lock.gameObject.SetActive(false);
            bool isCurrentUsing = IsCurrentUsed();
            Background.enabled = isCurrentUsing;
        }
        else {
            Background.enabled = false;
            Lock.gameObject.SetActive(true);
        }
    }

    bool IsCurrentUsed() {
        return scraper.id == GameManager.Instance.gameState.scraper;
    }
}
