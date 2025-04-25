using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class SwipePaging : MonoBehaviour, IEndDragHandler {
    public ScrollRect ScrollRect;
    [SerializeField] RectTransform Center;
    List<RectTransform> items = new List<RectTransform>();
    Canvas canvas;
    float scaleMultiplier = 1.1f;
    float normalScale = 0.65f;

    void Awake() {
        canvas = GetComponentInParent<Canvas>();
        // center = GameHelper.UIToScreenPos(viewport.TransformPoint(viewport.rect.center), canvas);
        // center = new Vector2(Screen.width / 2, Screen.height / 2);
        // UpdateItems();
        // UpdateItemStatus();
    }

    public void OnEndDrag(PointerEventData eventData) {
        // // TODO: Paging Scroll
        // base.OnEndDrag(eventData);
        // float dir = velocity.x;
        // int currentIndex = GetClosestItemIndex();
        // int targetIndex = currentIndex;
        // if (Mathf.Abs(dir) > 20f) {
        //     if (dir > 0)
        //         targetIndex = Mathf.Max(0, currentIndex - 1);
        //     else
        //         targetIndex = Mathf.Min(items.Count - 1, currentIndex + 1);
        // }
        // StartCoroutine(SmoothScrollToItem(targetIndex));
    }

    public void OnScroll() {
        UpdateItemStatus();
    }

    public void UpdateItems() {
        items.Clear();
        foreach (RectTransform child in ScrollRect.content) {
            items.Add(child);
        }
    }

    public void UpdateItemStatus() {
        for (int i = 0; i < items.Count; i++) {
            float distance = GetDistance(i);
            float scale = Mathf.Lerp(scaleMultiplier, normalScale, distance / ScrollRect.viewport.rect.width * 0.4f);
            scale = Mathf.Clamp(scale, normalScale, scaleMultiplier);
            items[i].localScale = Vector3.one * scale;
        }
    }

    public void ScrollToIndex(int index) {
        Vector3 currentPos = ScrollRect.content.localPosition;
        Vector3 targetPos = new Vector3(-items[index].localPosition.x, currentPos.y, currentPos.z);
        LeanTween.value(gameObject, 0, 1, 0.3f).setOnUpdate((float v) => {
            ScrollRect.content.localPosition = Vector3.Lerp(currentPos, targetPos, v);
            UpdateItemStatus();
        }).setOnComplete(() => {
            ScrollRect.content.localPosition = targetPos;
        }).setEase(LeanTweenType.easeOutQuad);
    }

    float GetDistance(int index) {
        Vector2 screenPos = GameHelper.UIToScreenPos(items[index].position, canvas);
        float distance = Vector2.Distance(screenPos, Center.position);
        return distance;
    }

    // float GetNormalizedPosition(int index) {
    //     float contentWidth = content.rect.width - viewport.rect.width;
    //     float itemPos = items[index].anchoredPosition.x;
    //     return Mathf.Clamp01(itemPos / contentWidth);
    // }

    // int GetClosestItemIndex() {
    //     int closestIndex = 0;
    //     float minDistance = float.MaxValue;
    //     for (int i = 0; i < items.Count; i++) {
    //         float distance = GetDistance(i);
    //         if (distance < minDistance) {
    //             minDistance = distance;
    //             closestIndex = i;
    //         }
    //     }
    //     return closestIndex;
    // }

    // IEnumerator SmoothScrollToItem(int index) {
    //     Vector3 itemWorldPos = items[index].position;
    //     Vector3 viewportCenterWorld = viewport.position;

    //     // Get how far we need to move the content
    //     float difference = viewportCenterWorld.x - itemWorldPos.x;
    //     Vector3 targetPos = content.localPosition + new Vector3(difference, 0f, 0f);

    //     Vector3 startPos = content.localPosition;
    //     float time = 0f;

    //     while (time < snapDuration) {
    //         time += Time.deltaTime;
    //         float t = time / snapDuration;
    //         content.localPosition = Vector3.Lerp(startPos, targetPos, Mathf.SmoothStep(0f, 1f, t));
    //         yield return null;
    //     }

    //     content.localPosition = targetPos;
    // }
}

