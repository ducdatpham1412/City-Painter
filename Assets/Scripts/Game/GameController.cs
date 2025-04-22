using UnityEngine;

public class GameController : Singleton<GameController> {
    [Header("Data")]
    public CitiesObject Cities;
    public ScrapersObject Scrapers;
    public Sprite PanSprite;

    [Header("GameObjects")]
    [SerializeField] GameObject SettingDialog;
    [SerializeField] ButtonManager SwitchModeBtn;
    public ScraperController Scraper;

    [Header("Stats")]
    public Mode mode = Mode.scrape;

    void Start() {
        if (GameManager.Instance.gameState.city == "") {
            GameManager.Instance.gameState.city = Cities.Cities[0].id;
        }
        if (GameManager.Instance.gameState.scraper == "") {
            GameManager.Instance.gameState.scraper = Scrapers.Scrapers[0].id;
        }
        City city = Cities.Cities.Find(c => c.id == GameManager.Instance.gameState.city);
        Scraper.SetScraper(Scrapers.Scrapers.Find(s => s.id == GameManager.Instance.gameState.scraper));
        SwitchModeBtn.Icon.sprite = PanSprite;
        GameInit.Instance.InitCity(city);
    }

    public void SwitchMode() {
        if (mode == Mode.pan) {
            mode = Mode.scrape;
            SwitchModeBtn.Icon.sprite = PanSprite;
            Scraper.SwitchScrapeMode();
        }
        else {
            mode = Mode.pan;
            SwitchModeBtn.Icon.sprite = Scraper.currentScraper.sprite;
            Scraper.SwitchPanMode();
        }
        Scraper.transform.position = GameHelper.ToWorldPoint(new Vector2(Screen.width / 2f, Screen.height / 2f));
    }

    public void OpenCloseSetting() {
        SettingDialog.SetActive(!SettingDialog.activeInHierarchy);
    }

    public void NoticeWinning() {

    }

    public void ChangeScraper(string id) {
        Scraper temp = Scrapers.Scrapers.Find(s => s.id == id);
        if (temp != null) {
            GameManager.Instance.gameState.scraper = id;
            Scraper.SetScraper(temp);
            SwitchModeBtn.Icon.sprite = temp.sprite;
        }
    }

    public enum Mode {
        pan,
        scrape,
    }
}