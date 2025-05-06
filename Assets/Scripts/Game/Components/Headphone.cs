using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class Headphone : MonoBehaviour {
    [SerializeField] LocalizeStringEvent Text;
    [SerializeField] RectTransform Content;

    void OnEnable() {
        LayoutRebuilder.ForceRebuildLayoutImmediate(Content);
    }

    public void OnClose() {
        gameObject.SetActive(false);
    }
}
