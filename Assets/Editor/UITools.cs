using UnityEngine;
using UnityEditor;
using System.Collections;

public class UITools : Editor {

    [MenuItem("My Tools/Fit Anchor to Rect")]
    static void MenuItem()
    {
        foreach (GameObject go in Selection.gameObjects)
        {
            RectTransform selectedRect = go.GetComponent<RectTransform>();
            RectTransform parentRect = go.transform.parent.GetComponent<RectTransform>();
            if (!selectedRect || !parentRect) return;

            Undo.RecordObject(selectedRect, "Change Anchor");

            selectedRect.anchorMin = new Vector2(selectedRect.anchorMin.x + selectedRect.offsetMin.x / parentRect.rect.width,
                selectedRect.anchorMin.y + selectedRect.offsetMin.y / parentRect.rect.height);
            selectedRect.anchorMax = new Vector2(selectedRect.anchorMax.x + selectedRect.offsetMax.x / parentRect.rect.width,
                selectedRect.anchorMax.y + selectedRect.offsetMax.y / parentRect.rect.height);

            selectedRect.offsetMin = selectedRect.offsetMax = Vector2.zero;
        }
    }

    [MenuItem("My Tools/Clear Playerprefs")]

    static void ClearPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
}
