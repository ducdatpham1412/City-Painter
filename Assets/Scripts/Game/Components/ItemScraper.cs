using UnityEditor;
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

        var controller = GameController.Instance;
        var manager = GameManager.Instance;

        if (scraper.cityUnlock == "ads") {
            if (manager.profile.items.Contains(scraper.id)) {
                controller.ChangeScraper(scraper.id);
            }
            else {
                void ShowAdToGetItem() {
                    controller.InfoDialog.Close();
                    GoogleAds.Instance.ShowReward(
                        success: () => {
                            manager.profile.items.Add(scraper.id);
                            controller.ChangeScraper(scraper.id);
                            controller.InfoDialog.Open(new InfoDialog.Info {
                                title = Helper.GetLocalizedValue("gotNewScraper", new string[] { scraper.name.GetLocalizedString() }),
                                icon = scraper.sprite,
                                OnClick = () => {
                                    controller.InfoDialog.Close();
                                },
                            });
                        },
                        error: () => {
                            controller.InfoDialog.Open(new InfoDialog.Info {
                                title = Helper.GetLocalizedValue("oppSomeError"),
                                btnTitle = Helper.GetLocalizedValue("retry"),
                                OnClick = ShowAdToGetItem,
                            });
                        }
                    );
                }

                controller.InfoDialog.Open(new InfoDialog.Info {
                    title = Helper.GetLocalizedValue("watchingAdToGetBroom"),
                    btnTitle = Helper.GetLocalizedValue("tryNow"),
                    OnClick = ShowAdToGetItem,
                });
            }
            return;
        }

        if (manager.IsCityUnlocked(scraper.cityUnlock)) {
            controller.ChangeScraper(scraper.id);
        }
        else {
            City city = manager.resources.cities.data.Find(c => c.id == scraper.cityUnlock);
            controller.InfoDialog.Open(new InfoDialog.Info {
                title = Helper.GetLocalizedValue("reachCityOpenScraper", new string[] { city.name.GetLocalizedString() }),
                OnClick = () => controller.InfoDialog.Close(),
            });
        }
    }

    public void UpdateEnable() {
        void Enable() {
            Lock.gameObject.SetActive(false);
            bool isCurrentUsing = IsCurrentUsed();
            Background.enabled = isCurrentUsing;
        }

        void Disable() {
            Background.enabled = false;
            Lock.gameObject.SetActive(true);
        }

        if (scraper.cityUnlock == "ads") {
            if (GameManager.Instance.profile.items.Contains(scraper.id)) {
                Enable();
            }
            else {
                Disable();
            }
            return;
        }

        if (GameManager.Instance.IsCityUnlocked(scraper.cityUnlock)) {
            Enable();
        }
        else {
            Disable();
        }
    }

    bool IsCurrentUsed() {
        return scraper.id == GameManager.Instance.gameState.scraper;
    }
}
