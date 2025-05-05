using UnityEngine;

public class WelcomeLogic : MonoBehaviour {
    [SerializeField] Canvas SelectLanguage;

    void Start() {
        if (GameManager.Instance.profile.localeID == null) {
            SelectLanguage.gameObject.SetActive(true);
        }
        else {
            Navigator.Instance.NavigateTo(Navigator.Scene.GameScene);
        }

        Screen.orientation = ScreenOrientation.AutoRotation;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
    }

    public void SetLocale(int localeID) {
        LocalizationManager.Instance.SetLocale(localeID, callback: () => {
            GameManager.Instance.profile.localeID = localeID;
            Navigator.Instance.NavigateTo(Navigator.Scene.GameScene);
        });
    }
}
