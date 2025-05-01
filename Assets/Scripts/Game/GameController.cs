using System;
using System.Collections;
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
    public ButtonManager BtnPlayAgain;
    public InfoDialog InfoDialog;
    public ScraperController Scraper;

    [Header("Stats")]
    public Mode mode = Mode.scrape;
    public bool isPlaying = false;
    public bool ended { get; private set; } = false;
    public bool inZoomMode { get; private set; } = false;
    City currentCity;
    CityImageState lastCityImgState = new();
    Transform cityImgTransform;


    void Start() {
        Scraper.SetScraper(GameManager.Instance.resources.scrapers.data.Find(s => s.id == GameManager.Instance.gameState.scraper));
        cityImgTransform = GameInit.Instance.CityImage.transform;
        InitCity(GameManager.Instance.gameState.city);
    }

    void Update() {
        if (!inZoomMode) return;
        if (GameHelper.TouchBegin()) {
            Vector3 worldPos = GameHelper.ToWorldPoint(GameHelper.TouchPosition());
            if (GameHelper.TouchHitGameObject(worldPos, cityImgTransform.gameObject
            )) {
                Vector3 localPos = cityImgTransform.InverseTransformPoint(worldPos);
                lastCityImgState.position = -localPos;
                Zoom();
            }
        }
    }

    public void InitCity(string cityID) {
        if (!ended) {
            SaveCityWireFrame();
        }
        else {
            ended = false;
        }
        GameManager.Instance.gameState.city = cityID;
        City city = GameManager.Instance.resources.cities.data.Find(c => c.id == cityID);
        currentCity = city;
        GameInit.Instance.InitCity(city);
        if (mode == Mode.pan) {
            SwitchMode();
        }
        if (inZoomMode) {
            lastCityImgState.position = Vector3.zero;
            Zoom(false);
        }
    }

    public void SwitchMode() {
        if (mode == Mode.pan) {
            mode = Mode.scrape;
            SwitchModeBtn.Icon.sprite = PanSprite;
            TextPanningMode.gameObject.SetActive(false);
            if (isPlaying) {
                Scraper.gameObject.SetActive(true);
            }
            else {
                BtnPlayAgain.gameObject.SetActive(true);
            }
        }
        else {
            mode = Mode.pan;
            SwitchModeBtn.Icon.sprite = Scraper.currentScraper.sprite;
            TextPanningMode.gameObject.SetActive(true);
            if (isPlaying) {
                Scraper.gameObject.SetActive(false);
            }
            else {
                BtnPlayAgain.gameObject.SetActive(false);
            }
        }
        Scraper.transform.position = GameHelper.ToWorldPoint(new Vector2(Screen.width / 2f, Screen.height / 2f));
    }

    public void OpenCloseSetting() {
        SettingDialog.SetActive(!SettingDialog.activeInHierarchy);
    }

    public void NoticeWinning() {
        ended = true;
        FirebaseTracking.Instance.FinishCity(currentCity.id);
        Storage.DELETE_TEXTURE(currentCity.id);
        SoundManager.Instance.PlaySF(SoundManager.SF.Win_01);
        if (GameManager.Instance.citySprites.ContainsKey(currentCity.id)) {
            GameManager.Instance.citySprites[currentCity.id] = currentCity.sprite;
        }
        var cities = GameManager.Instance.resources.cities.data;
        int index = cities.FindIndex(c => c.id == currentCity.id);
        bool hasNextCity = index < (cities.Count - 1);

        void OnNextCity() {
            if (hasNextCity) {
                InfoDialog.Close();
                int nextIndex = index + 1;
                string nextCityID = cities[nextIndex].id;
                int lastIndex = GameManager.Instance.resources.cities.data.FindIndex(c => c.id == GameManager.Instance.profile.lastCity);
                if (lastIndex < nextIndex) {
                    Scraper unlockedScraper = GameManager.Instance.resources.scrapers.data.Find(s => s.cityUnlock == nextCityID);
                    if (unlockedScraper != null) {
                        StartCoroutine(CollectNewScraper(unlockedScraper));
                    }
                    GameManager.Instance.profile.lastCity = nextCityID;
                }
                InitCity(nextCityID);
                return;
            }

            StartCoroutine(FinishedAll());
        }

        IEnumerator CollectNewScraper(Scraper _scraper) {
            InfoDialog.Close();
            yield return new WaitForSeconds(0.3f);
            InfoDialog.Open(new InfoDialog.Info {
                title = Helper.GetLocalizedValue("gotNewScraper", new string[] {
                    _scraper.name.GetLocalizedString()
                }),
                btnTitle = Helper.GetLocalizedValue("tryNow"),
                icon = _scraper.sprite,
                OnClick = () => {
                    ChangeScraper(_scraper.id);
                    InfoDialog.Close();
                },
            });
        }

        IEnumerator FinishedAll() {
            InfoDialog.Close();
            yield return new WaitForSeconds(0.3f);
            InfoDialog.Open(new InfoDialog.Info {
                title = Helper.GetLocalizedValue("finishedAll"),
                sfx = SoundManager.SF.None,
                OnClick = () => {
                    InfoDialog.Close();
                    Navigator.Instance.NavigateTo(Navigator.Scene.CitiesScene, UnityEngine.SceneManagement.LoadSceneMode.Additive);
                },
            });
        }

        InfoDialog.Open(new InfoDialog.Info {
            title = Helper.GetLocalizedValue("finishCity", new string[] { currentCity.name.GetLocalizedString() }),
            btnTitle = Helper.GetLocalizedValue("letGo"),
            sfx = SoundManager.SF.None,
            OnClick = OnNextCity,
        });
    }

    public void ChangeScraper(string id) {
        Scraper s = GameManager.Instance.resources.scrapers.data.Find(s => s.id == id);
        if (s != null) {
            FirebaseTracking.Instance.UseScraper(id);
            GameManager.Instance.gameState.scraper = s.id;
            Scraper.SetScraper(s);
            if (mode == Mode.pan) {
                SwitchModeBtn.Icon.sprite = s.sprite;
            }
            foreach (var item in GameManager.Instance.ItemScrapers) {
                item.UpdateEnable();
            }
        }
    }

    public void Zoom(bool playSF = true) {
        float duration = 0.6f;
        LeanTweenType TweenType = LeanTweenType.easeOutQuad;

        if (inZoomMode) {
            LeanTween.scale(cityImgTransform.gameObject, Vector3.one, duration).setEase(TweenType);
            LeanTween.move(cityImgTransform.gameObject, lastCityImgState.position, duration).setEase(TweenType).setOnComplete(() => {
                inZoomMode = false;
                SwitchModeBtn.gameObject.SetActive(true);
                if (isPlaying) {
                    if (mode == Mode.scrape) {
                        Scraper.gameObject.SetActive(true);
                    }
                }
                else {
                    BtnPlayAgain.gameObject.SetActive(true);
                }
                Scraper.transform.position = Vector3.zero;
            });
            ZoomIcon.sprite = ZoomOutSprite;
            if (playSF) SoundManager.Instance.PlaySF(SoundManager.SF.Whoosh_Transition);
            return;
        }

        lastCityImgState.position = cityImgTransform.position;
        LeanTween.scale(cityImgTransform.gameObject, Vector3.one / 2f, duration).setEase(TweenType);
        LeanTween.move(cityImgTransform.gameObject, Vector3.zero, duration).setEase(TweenType).setOnComplete(() => {
            inZoomMode = true;
        });
        ZoomIcon.sprite = ZoomInSprite;
        if (playSF) SoundManager.Instance.PlaySF(SoundManager.SF.Whoosh_Transition);
        SwitchModeBtn.gameObject.SetActive(false);
        if (isPlaying) {
            Scraper.gameObject.SetActive(false);
        }
        else {
            BtnPlayAgain.gameObject.SetActive(false);
        }
    }

    public void GoToCitiesScene() {
        Navigator.Instance.NavigateTo(Navigator.Scene.CitiesScene, UnityEngine.SceneManagement.LoadSceneMode.Additive);
    }

    public void SaveCityWireFrame() {
        if (currentCity == null) return;
        if (GameManager.Instance.ShouldCityPlayAgain(currentCity.id)) return;
        Storage.SET_TEXTURE(currentCity.id, Scraper.WireFrame.sprite.texture);
        if (GameManager.Instance.citySprites.ContainsKey(currentCity.id)) {
            GameManager.Instance.citySprites[currentCity.id] = Scraper.WireFrame.sprite;
        }
    }

    public void PlayAgain() {
        ended = true;
        Storage.SET_TEXTURE(currentCity.id, GameInit.Instance.GenerateLineArtSprite(currentCity).texture);
        if (GameManager.Instance.citySprites.ContainsKey(currentCity.id)) {
            GameManager.Instance.citySprites[currentCity.id] = Scraper.WireFrame.sprite;
        }
        InitCity(currentCity.id);
    }

    public enum Mode {
        pan,
        scrape,
    }

    [Serializable]
    class CityImageState {
        public Vector3 position;
    }
}