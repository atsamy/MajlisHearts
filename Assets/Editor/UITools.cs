using UnityEngine;
using UnityEditor;
using System.Collections;
using TMPro;
using ArabicSupport;

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

    [MenuItem("My Tools/Enable Tutorial")]
    static void EnableTutorial()
    {
        PlayerPrefs.DeleteKey("tutorial");
    }

    [MenuItem("My Tools/Disable Tutorial")]
    static void DisableTutorial()
    {
        PlayerPrefs.SetInt("tutorial",2);
    }

    [MenuItem("My Tools/Reset Daily Rewards")]
    static void ResetDailyRewards()
    {
        PlayerPrefs.DeleteKey("LastReward");
        PlayerPrefs.DeleteKey("LastRewardTime");
        PlayerPrefs.DeleteKey("DebugTime");
    }

    [MenuItem("My Tools/FixArabicTMP")]
    static void FixArabicTMP()
    {
        TextMeshProUGUI text;
        if ((text = Selection.activeTransform.gameObject.GetComponent<TextMeshProUGUI>()) != null)
        {
            text.text = ArabicFixer.Fix(text.text);
        }
    }
}
