using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CitiesController : MonoBehaviour {
    [SerializeField] GameObject ItemCityPrefab;
    [SerializeField] SwipePaging Swipe;
    [SerializeField] InfoDialog InfoDialog;
    [SerializeField] LoadingManager Loading;
    [SerializeField] ButtonManager BackBtn;

    void Start() {
        StartCoroutine(InitCities());
    }

    public void GoBack() {
        Navigator.Instance.UnloadSceneAsync(Navigator.Scene.CitiesScene);
    }

    public void GoToCity(City city) {
        InfoDialog.Open(new InfoDialog.Info {
            title = Helper.GetLocalizedValue("goToCity", new string[] { city.name.GetLocalizedString() }),
            OnClick = () => {
                if (city.id == GameManager.Instance.gameState.city) {
                    GoBack();
                    return;
                }
                GameController.Instance.InitCity(city.id);
                GoBack();
            }
        });
    }

    IEnumerator InitCities() {
        BackBtn.gameObject.SetActive(false);
        Loading.StartLoading();
        yield return null;

        GameManager manager = GameManager.Instance;

        if (manager.citySprites.Keys.Count == 0) {
            foreach (City city in manager.resources.cities.data) {
                if (manager.ShouldCityPlayAgain(city.id)) {
                    manager.citySprites[city.id] = city.sprite;
                }
                else if (city.id == manager.gameState.city) {
                    manager.citySprites[city.id] = GameController.Instance.Scraper.WireFrame.sprite;
                }
                else {
                    manager.citySprites[city.id] = GameInit.Instance.GenerateLineArtSprite(city);
                }
                yield return null;
            }
        }
        else {
            manager.citySprites[manager.gameState.city] = GameController.Instance.Scraper.WireFrame.sprite;
            yield return null;
        }

        BackBtn.gameObject.SetActive(true);
        Destroy(Loading.gameObject);

        foreach (City city in manager.resources.cities.data) {
            ItemCity item = Instantiate(ItemCityPrefab, Swipe.ScrollRect.content).GetComponent<ItemCity>();
            item.SetCity(city);
        }

        yield return null;
        LayoutRebuilder.ForceRebuildLayoutImmediate(Swipe.ScrollRect.content);
        Swipe.UpdateItems();
        yield return null;
        int index = manager.resources.cities.data.FindIndex(c => c.id == manager.gameState.city);
        Swipe.ScrollToIndex(index);
    }
}
