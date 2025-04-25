using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.EventSystems;

public static class GameHelper {
    public static bool TouchBegin() {
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame) {
            return true;
        }

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) {
            return true;
        }

        return false;
    }

    public static bool TouchOverlayWorldGameObject() {
        // 1) If pointer is over UI, abort:
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return false;
        return true;
    }

    // public static bool TouchHitGameObject(Vector3 worldPos, GameObject gameObject) {
    //     if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
    //         return false;

    //     RaycastHit2D[] hits = Physics2D.RaycastAll(worldPos, Vector2.zero);
    //     foreach (var h in hits) {
    //         if (h.collider.gameObject == gameObject) return true;
    //     }

    //     return false;
    // }

    public static Vector2 TouchPosition() {
        if (Touchscreen.current != null) {
            return Touchscreen.current.primaryTouch.position.ReadValue();
        }

        if (Mouse.current != null) {
            return Mouse.current.position.ReadValue();
        }

        return Vector2.zero;
    }

    public static bool TouchReleased() {
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasReleasedThisFrame) {
            return true;
        }

        if (Mouse.current != null && Mouse.current.leftButton.wasReleasedThisFrame) {
            return true;
        }

        return false;
    }

    public static Vector3 ToWorldPoint(Vector3 localPos) {
        return Camera.main.ScreenToWorldPoint(localPos);
    }

    public static Vector2 UIToScreenPos(Vector2 worldPos, Canvas canvas) {
        Camera cam = null;
        if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay) {
            cam = canvas.worldCamera;
        }
        return RectTransformUtility.WorldToScreenPoint(cam, worldPos);
    }
}
