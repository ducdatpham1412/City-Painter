using UnityEngine;
using UnityEngine.InputSystem;

public class PinchZoom : MonoBehaviour {
    public float zoomSpeed = 0.005f;
    public float minScale = 0.5f;
    public float maxScale = 3f;

    void Update() {
        if (Touchscreen.current == null || Touchscreen.current.touches.Count < 2)
            return;

        var touch0 = Touchscreen.current.touches[0];
        var touch1 = Touchscreen.current.touches[1];

        if (!touch0.isInProgress || !touch1.isInProgress)
            return;

        Vector2 touch0Current = touch0.position.ReadValue();
        Vector2 touch1Current = touch1.position.ReadValue();

        Vector2 touch0Prev = touch0Current - touch0.delta.ReadValue();
        Vector2 touch1Prev = touch1Current - touch1.delta.ReadValue();

        float prevTouchDeltaMag = (touch0Prev - touch1Prev).magnitude;
        float currentTouchDeltaMag = (touch0Current - touch1Current).magnitude;

        float deltaMagnitudeDiff = currentTouchDeltaMag - prevTouchDeltaMag;

        float scaleFactor = 1 + deltaMagnitudeDiff * zoomSpeed;
        Vector3 newScale = transform.localScale * scaleFactor;
        newScale = ClampScale(newScale, minScale, maxScale);

        // Convert midpoint to world point to zoom toward it
        Vector2 touchMidpoint = (touch0Current + touch1Current) * 0.5f;
        Vector3 worldMid = Camera.main.ScreenToWorldPoint(new Vector3(touchMidpoint.x, touchMidpoint.y, Camera.main.nearClipPlane));
        Vector3 direction = transform.position - worldMid;

        transform.localScale = newScale;
        transform.position = worldMid + direction * scaleFactor;
    }

    private Vector3 ClampScale(Vector3 scale, float min, float max) {
        float clampedX = Mathf.Clamp(scale.x, min, max);
        float clampedY = Mathf.Clamp(scale.y, min, max);
        return new Vector3(clampedX, clampedY, 1f);
    }
}
