using System;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class ItemSound : MonoBehaviour {
    static Status selectedStatus = new Status {
        localScale = Vector3.one,
        color = Helper.ColorFromHex("#FEFFAA")
    };
    static Status deselectedStatus = new Status {
        localScale = new Vector3(0.9f, 0.9f, 0.9f),
        color = Helper.ColorFromHex("#807979")
    };

    [SerializeField] Image Background;
    [SerializeField] Image Icon;
    [SerializeField] LocalizeStringEvent Title;

    RectTransform rect;
    BaseSound sound;

    Status currentStatus;

    void Awake() {
        rect = GetComponent<RectTransform>();
        GameManager.Instance.ItemSounds.Add(this);
    }

    static bool IsSelected(string soundID) {
        return GameManager.Instance.profile.backgroundSounds.Find(s => s == soundID) != null || GameManager.Instance.profile.sfxSounds.Find(s => s == soundID) != null;
    }

    static void ToggleSound(BaseSound sound) {
        if (sound.type == BaseSound.Type.background) {
            SoundManager.Instance.PlayStopBackgroundSound((BackgroundSound)sound);
        }
        else {
            SoundManager.Instance.PlayStopSfxSound((SfxSound)sound);
        }
    }

    public void SetSound(BaseSound _sound) {
        sound = _sound;
        bool isSelected = IsSelected(sound.id);
        Icon.sprite = sound.icon;
        Title.StringReference = sound.name;
        Title.RefreshString();
        currentStatus = isSelected ? selectedStatus : deselectedStatus;
        rect.localScale = currentStatus.localScale;
        Background.color = currentStatus.color;
    }

    public void OnPress() {
        if (sound.id == "none") {
            foreach (ItemSound item in GameManager.Instance.ItemSounds) {
                item.ChangeStatus(false);
            }
            return;
        }

        bool isSelected = IsSelected(sound.id);
        ChangeStatus(!isSelected);
    }

    void ChangeStatus(bool selected) {
        Status beforeStatus = currentStatus;
        currentStatus = selected ? selectedStatus : deselectedStatus;
        ToggleSound(sound);
        LeanTween.cancel(gameObject);
        LeanTween.value(gameObject, 0, 1, 0.15f).setEase(LeanTweenType.easeOutQuad).setOnUpdate((float v) => {
            rect.localScale = Vector3.Lerp(beforeStatus.localScale, currentStatus.localScale, v);
            Background.color = Color.Lerp(beforeStatus.color, currentStatus.color, v);
        });
    }

    [Serializable]
    class Status {
        public Vector3 localScale;
        public Color color;
    }
}
