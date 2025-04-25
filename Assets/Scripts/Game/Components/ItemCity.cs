using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class ItemCity : MonoBehaviour {
    [SerializeField] Image Image;
    [SerializeField] LocalizeStringEvent Name;
    [SerializeField] GameObject Lock;
    City city;
    bool isUnlocked;

    static CitiesController controller;

    public void SetCity(City _city) {
        city = _city;
        isUnlocked = GameManager.Instance.IsCityUnlocked(city.id);
        Lock.SetActive(!isUnlocked);
        Image.sprite = city.sprite;
        Name.StringReference = city.name;
        Name.RefreshString();
    }

    public void OnPress() {
        if (isUnlocked) {
            Punch(1.2f);
            if (controller == null) {
                controller = FindFirstObjectByType<CitiesController>();
            }
            controller.GoToCity(city);
        }
        else {
            Punch(0.8f);
        }
    }

    void Punch(float scale) {
        if (LeanTween.isTweening(gameObject)) return;
        SoundManager.Instance.PlaySF(SoundManager.SF.Pop_01);
        Vector3 localScale = GetComponent<RectTransform>().localScale * scale;
        LeanTween.scale(gameObject, localScale, 0.8f).setEase(LeanTweenType.punch);
    }
}
