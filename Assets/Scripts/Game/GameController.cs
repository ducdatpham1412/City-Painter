using UnityEngine;

public class GameController : Singleton<GameController> {
    [Header("Data")]
    [SerializeField] CitiesObject Cities;
    [SerializeField] Sprite PanSprite;
    [SerializeField] Sprite DrawSprite;

    [Header("GameObjects")]
    [SerializeField] GameObject SettingDialog;
    [SerializeField] ButtonManager SwitchModeBtn;

    [Header("Stats")]
    public Mode mode = Mode.draw;

    public void SwitchMode() {
        if (mode == Mode.pan) {
            mode = Mode.draw;
            SwitchModeBtn.Icon.sprite = DrawSprite;
        }
        else {
            mode = Mode.pan;
            SwitchModeBtn.Icon.sprite = PanSprite;
        }
    }

    public void OpenCloseSetting() {
        SettingDialog.SetActive(!SettingDialog.activeInHierarchy);
    }

    public enum Mode {
        pan,
        draw,
    }
}