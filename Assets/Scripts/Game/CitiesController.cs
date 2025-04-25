using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CitiesController : MonoBehaviour {
    [SerializeField] GameObject ItemCityPrefab;
    [SerializeField] SwipePaging Swipe;
    [SerializeField] InfoDialog InfoDialog;

    void Start() {
        foreach (City city in GameManager.Instance.resources.cities.data) {
            ItemCity item = Instantiate(ItemCityPrefab, Swipe.ScrollRect.content).GetComponent<ItemCity>();
            item.SetCity(city);
        }
        StartCoroutine(RebuildLayout());
    }

    public void GoBack() {
        Navigator.Instance.UnloadSceneAsync(Navigator.Scene.CitiesScene);
    }

    public void GoToCity(City city) {
        InfoDialog.Open(new InfoDialog.Info {
            title = Helper.GetLocalizedValue("goToCity", new string[] { city.name.GetLocalizedString() }),
            btnTitle = "Ok",
            OnClick = () => {
                // TODO: Init city
                GoBack();
            }
        });
    }

    IEnumerator RebuildLayout() {
        yield return null;
        LayoutRebuilder.ForceRebuildLayoutImmediate(Swipe.ScrollRect.content);
        Swipe.UpdateItems();
        yield return null;
        int index = GameManager.Instance.resources.cities.data.FindIndex(c => c.id == GameManager.Instance.gameState.city);
        Swipe.ScrollToIndex(index);
    }
}
