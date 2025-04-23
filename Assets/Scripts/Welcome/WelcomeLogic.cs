using UnityEngine;

public class WelcomeLogic : MonoBehaviour {
    [SerializeField] Canvas SelectLanguage;

    void Start() {
        GameManager.Instance.Initialize();
        if (GameManager.Instance.profile.localeID == null) {
            SelectLanguage.gameObject.SetActive(true);
        }
        else {
            Navigator.Instance.NavigateTo(Navigator.Scene.GameScene);
        }
    }

    public void SetLocale(int localeID) {
        LocalizationManager.Instance.SetLocale(localeID, callback: () => {
            GameManager.Instance.profile.localeID = localeID;
            Navigator.Instance.NavigateTo(Navigator.Scene.GameScene);
        });
    }
}
