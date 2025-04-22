using UnityEngine.InputSystem;
using UnityEngine;

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

    public static bool TouchHitGameObject(Vector3 localPos, GameObject gameObject) {
        Vector3 worldPoint = ToWorldPoint(localPos);
        RaycastHit2D[] hits = Physics2D.RaycastAll(worldPoint, Vector2.zero);
        foreach (var h in hits) {
            if (h.collider.gameObject == gameObject) return true;
        }
        return false;
    }

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
}
