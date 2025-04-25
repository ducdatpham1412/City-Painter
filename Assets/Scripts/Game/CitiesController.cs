using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CitiesController : MonoBehaviour {
    [SerializeField] GameObject ItemCityPrefab;
    [SerializeField] SwipePaging Swipe;
    [SerializeField] InfoDialog InfoDialog;
    [SerializeField] LoadingManager Loading;

    void Start() {
        StartCoroutine(InitCities());
    }

    public void GoBack() {
        Navigator.Instance.UnloadSceneAsync(Navigator.Scene.CitiesScene);
    }

    public void GoToCity(City city) {
        InfoDialog.Open(new InfoDialog.Info {
            title = Helper.GetLocalizedValue("goToCity", new string[] { city.name.GetLocalizedString() }),
            btnTitle = "Ok",
            OnClick = () => {
                GameController.Instance.InitCity(city.id);
                GoBack();
            }
        });
    }

    IEnumerator InitCities() {
        Loading.StartLoading();
        yield return null;
        if (GameManager.Instance.citySprites.Keys.Count == 0) {
            foreach (City city in GameManager.Instance.resources.cities.data) {
                bool isUnlocked = GameManager.Instance.IsCityUnlocked(city.id);
                if (isUnlocked) {
                    GameManager.Instance.citySprites[city.id] = city.sprite;
                }
                else {
                    GameManager.Instance.citySprites[city.id] = GameInit.Instance.GenerateLineArtSprite(city.sprite);
                }
                yield return null;
            }
        }
        else {
            City currentCity = GameManager.Instance.resources.cities.data.Find(c => c.id == GameManager.Instance.gameState.city);
            bool isUnlocked = GameManager.Instance.IsCityUnlocked(currentCity.id);
            if (isUnlocked) {
                GameManager.Instance.citySprites[currentCity.id] = currentCity.sprite;
            }
            else {
                GameManager.Instance.citySprites[currentCity.id] = GameInit.Instance.GenerateLineArtSprite(currentCity.sprite);
            }
            yield return null;
        }

        Destroy(Loading.gameObject);

        foreach (City city in GameManager.Instance.resources.cities.data) {
            ItemCity item = Instantiate(ItemCityPrefab, Swipe.ScrollRect.content).GetComponent<ItemCity>();
            item.SetCity(city);
            item.LoadSprite(GameManager.Instance.citySprites[city.id]);
        }

        yield return null;
        LayoutRebuilder.ForceRebuildLayoutImmediate(Swipe.ScrollRect.content);
        Swipe.UpdateItems();
        yield return null;
        int index = GameManager.Instance.resources.cities.data.FindIndex(c => c.id == GameManager.Instance.gameState.city);
        Swipe.ScrollToIndex(index);
    }
}
