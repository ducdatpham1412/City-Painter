using System;
using UnityEngine;
using UnityEngine.UI;

public class GameController : Singleton<GameController> {
    [Header("Data")]
    public Sprite PanSprite;
    [SerializeField] Sprite ZoomOutSprite;
    [SerializeField] Sprite ZoomInSprite;

    [Header("GameObjects")]
    [SerializeField] GameObject SettingDialog;
    [SerializeField] ButtonManager SwitchModeBtn;
    [SerializeField] Image ZoomIcon;
    [SerializeField] Text TextPanningMode;
    [SerializeField] InfoDialog InfoDialog;
    public ScraperController Scraper;

    [Header("Stats")]
    public Mode mode = Mode.scrape;
    CityImageState lastCityImgState = new();


    void Start() {
        City city = GameManager.Instance.resources.cities.data.Find(c => c.id == GameManager.Instance.gameState.city);
        Scraper.SetScraper(GameManager.Instance.resources.scrapers.data.Find(s => s.id == GameManager.Instance.gameState.scraper));
        SwitchModeBtn.Icon.sprite = PanSprite;
        GameInit.Instance.InitCity(city);
    }

    public void SwitchMode() {
        if (mode == Mode.pan) {
            mode = Mode.scrape;
            SwitchModeBtn.Icon.sprite = PanSprite;
            Scraper.gameObject.SetActive(true);
            TextPanningMode.gameObject.SetActive(false);
        }
        else {
            mode = Mode.pan;
            SwitchModeBtn.Icon.sprite = Scraper.currentScraper.sprite;
            Scraper.gameObject.SetActive(false);
            TextPanningMode.gameObject.SetActive(true);
        }
        Scraper.transform.position = GameHelper.ToWorldPoint(new Vector2(Screen.width / 2f, Screen.height / 2f));
    }

    public void OpenCloseSetting() {
        SettingDialog.SetActive(!SettingDialog.activeInHierarchy);
    }

    public void PlayCityToUnlock(string city) {
        InfoDialog.Open(new InfoDialog.Info {
            title = Helper.GetLocalizedValue("reachCityOpenScraper", new string[] { city }),
            fontSize = 16,
            btnTitle = "Ok",
            OnClick = () => InfoDialog.Close(),
        });
    }

    public void NoticeWinning() {

    }

    public void ChangeScraper(string id) {
        Scraper s = GameManager.Instance.resources.scrapers.data.Find(s => s.id == id);
        if (s != null) {
            GameManager.Instance.gameState.scraper = s.id;
            Scraper.SetScraper(s);
            SwitchModeBtn.Icon.sprite = s.sprite;
            foreach (var item in GameManager.Instance.ItemScrapers) {
                item.UpdateEnable();
            }
        }
    }

    public void Zoom() {
        float duration = 0.6f;
        LeanTweenType TweenType = LeanTweenType.easeOutQuad;

        Transform cityTransform = GameInit.Instance.CityImage.transform;
        if (cityTransform.localScale.Equals(Vector3.one)) {
            lastCityImgState.position = cityTransform.position;
            lastCityImgState.localScale = cityTransform.localScale;
            LeanTween.scale(cityTransform.gameObject, Vector3.one / 2f, duration).setEase(TweenType);
            LeanTween.move(cityTransform.gameObject, Vector3.zero, duration).setEase(TweenType);
            ZoomIcon.sprite = ZoomInSprite;
            SoundManager.Instance.PlaySF(SoundManager.SF.Whoosh_Transition);
            Scraper.gameObject.SetActive(false);
            SwitchModeBtn.gameObject.SetActive(false);
            return;
        }

        LeanTween.scale(cityTransform.gameObject, lastCityImgState.localScale, duration).setEase(TweenType);
        LeanTween.move(cityTransform.gameObject, lastCityImgState.position, duration).setEase(TweenType);
        ZoomIcon.sprite = ZoomOutSprite;
        SoundManager.Instance.PlaySF(SoundManager.SF.Whoosh_Transition);
        if (mode == Mode.scrape) {
            Scraper.gameObject.SetActive(true);
        }
        SwitchModeBtn.gameObject.SetActive(true);
        Scraper.transform.position = Vector3.zero;
    }

    public enum Mode {
        pan,
        scrape,
    }

    [Serializable]
    class CityImageState {
        public Vector3 position;
        public Vector3 localScale;
    }
}