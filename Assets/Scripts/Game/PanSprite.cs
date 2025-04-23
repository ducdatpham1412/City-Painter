using System;
using UnityEngine;

public class PanSprite : MonoBehaviour {
    Vector2 pivotPos;
    Vector2 touchPos;
    Vector2 currentPos;
    bool isPanning = false;

    Vector2 velocity = Vector2.zero;
    float inertiaDamping = 3f;
    float maxVelocity = 20f;
    Bound bound;

    public void SetSprite(Sprite sprite) {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.sprite = sprite;
        float width = sr.bounds.size.x;
        float height = sr.bounds.size.y;
        Vector2 pos = transform.position;
        bound = new Bound {
            minX = pos.x - width / 2,
            maxX = pos.x + width / 2,
            minY = pos.y - height / 2,
            maxY = pos.y + height / 2,
        };
    }

    void Update() {
        if (!isPanning && GameHelper.TouchBegin()) {
            if (GameController.Instance.mode != GameController.Mode.pan) return;
            if (LeanTween.isTweening(gameObject)) LeanTween.cancel(gameObject);

            isPanning = true;
            pivotPos = transform.position;
            touchPos = GameHelper.ToWorldPoint(GameHelper.TouchPosition());
            currentPos = touchPos;
        }

        if (isPanning) {
            if (GameHelper.TouchReleased()) {
                isPanning = false;
                CheckBackToBound();
                return;
            }

            if (GameHelper.TouchOverlayWorldGameObject()) {
                Vector2 lastPos = currentPos;
                currentPos = GameHelper.ToWorldPoint(GameHelper.TouchPosition());
                transform.position = pivotPos + (currentPos - touchPos);
                GameController.Instance.Scraper.transform.position = currentPos;
                Vector2 delta = currentPos - lastPos;
                velocity = delta / Time.deltaTime;
                velocity = Vector2.ClampMagnitude(velocity, maxVelocity);
            }
        }

        else {
            if (velocity.magnitude > 0.01f) {
                transform.position += (Vector3)(velocity * Time.deltaTime);
                velocity = Vector2.Lerp(velocity, Vector2.zero, inertiaDamping * Time.deltaTime);
            }
            else {
                CheckBackToBound();
            }
        }
    }

    void CheckBackToBound() {
        Vector2 pos = transform.position;
        Vector2 backToPos = pos;

        if (pos.x < bound.minX) {
            backToPos.x = bound.minX;
        }
        else if (pos.x > bound.maxX) {
            backToPos.x = bound.maxX;
        }

        if (pos.y < bound.minY) {
            backToPos.y = bound.minY;
        }
        else if (pos.y > bound.maxY) {
            backToPos.y = bound.maxY;
        }

        if (backToPos.Equals(pos)) return;

        velocity = Vector2.zero;
        LeanTween.move(gameObject, backToPos, 0.65f).setEase(LeanTweenType.easeOutBack);
    }

    [Serializable]
    class Bound {
        public float minX;
        public float maxX;
        public float minY;
        public float maxY;
    }
}
