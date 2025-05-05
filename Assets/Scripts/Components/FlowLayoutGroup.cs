using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("Layout/Flow Layout Group")]
public class FlowLayoutGroup : LayoutGroup {
    public float spacingX = 10f;
    public float spacingY = 10f;

    public override void CalculateLayoutInputHorizontal() {
        base.CalculateLayoutInputHorizontal();
        // SetLayout();
    }

    public override void CalculateLayoutInputVertical() {
        SetLayout();
    }

    public override void SetLayoutHorizontal() { }
    public override void SetLayoutVertical() { }

    private void SetLayout() {
        float width = rectTransform.rect.width;
        float x = padding.left;
        float y = padding.top;
        float rowHeight = 0f;

        for (int i = 0; i < rectChildren.Count; i++) {
            RectTransform child = rectChildren[i];

            // float childWidth = LayoutUtility.GetPreferredSize(child, 0);
            // float childHeight = LayoutUtility.GetPreferredSize(child, 1);

            float childWidth = child.rect.width;
            float childHeight = child.rect.height;

            if (x + childWidth > width - padding.right) {
                // Wrap to next line
                x = padding.left;
                y += rowHeight + spacingY;
                rowHeight = 0f;
            }

            SetChildAlongAxis(child, 0, x, childWidth);
            SetChildAlongAxis(child, 1, y, childHeight);

            x += childWidth + spacingX;
            rowHeight = Mathf.Max(rowHeight, childHeight);
        }

        y += rowHeight + padding.bottom;
        SetLayoutInputForAxis(y, y, -1, 1);

        // LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
    }
}
