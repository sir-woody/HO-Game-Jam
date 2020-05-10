using UnityEditor;
using UnityEngine;

public static class RectTransformFitter
{
    [MenuItem("CONTEXT/RectTransform/Set Native Position With Anchors", false, 0)]
    private static void SetNativePosition()
    {
        foreach (UnityEngine.Object obj in Selection.objects)
        {
            GameObject transform = obj as GameObject;
            if (transform == null)
            {
                continue;
            }
            RectTransform rectTransform = transform.GetComponent<RectTransform>();
            if (rectTransform == null)
            {
                continue;
            }
            if (rectTransform.parent == null)
            {
                continue;
            }
            RectTransform parent = rectTransform.parent.GetComponent<RectTransform>();
            if (parent == null)
            {
                continue;
            }

            Vector2 min = new Vector2(
                InverseLerpUnclamped(-parent.rect.width * parent.pivot.x, parent.rect.width * (1 - parent.pivot.x), AnchoredToCenter(rectTransform).x - rectTransform.rect.width * 0.5f),
                InverseLerpUnclamped(-parent.rect.height * parent.pivot.y, parent.rect.height * (1 - parent.pivot.y), AnchoredToCenter(rectTransform).y - rectTransform.rect.height * 0.5f));
            Vector2 max = new Vector2(
                InverseLerpUnclamped(-parent.rect.width * parent.pivot.x, parent.rect.width * (1 - parent.pivot.x), AnchoredToCenter(rectTransform).x + rectTransform.rect.width * 0.5f),
                InverseLerpUnclamped(-parent.rect.height * parent.pivot.y, parent.rect.height * (1 - parent.pivot.y), AnchoredToCenter(rectTransform).y + rectTransform.rect.height * 0.5f));

            Undo.RecordObject(rectTransform, "Fit Anchors");
            rectTransform.anchorMin = min;
            rectTransform.anchorMax = max;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.anchoredPosition = Vector2.zero;
        }
    }

    private static Vector2 AnchoredToCenter(RectTransform rectTransform)
    {
        return (Vector2)rectTransform.localPosition - (rectTransform.pivot - Vector2.one * 0.5f) * rectTransform.rect.size;
    }
    private static float InverseLerpUnclamped(float a, float b, float value)
    {
        return (value - a) / (b - a);
    }

}
