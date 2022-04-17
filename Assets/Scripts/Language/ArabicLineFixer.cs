using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using ArabicSupport;

[ExecuteInEditMode()]
public class ArabicLineFixer : MonoBehaviour
{
    public static ArabicLineFixer Instance;
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    void OnEnable()
    {
    #if UNITY_EDITOR
        if (Instance == null)
            Instance = this;
    #endif
    }

    public void FixLines( Text UIText, string arText, bool returnToBF = true)
    {
        StartCoroutine(FixLinesEnumerator(UIText, arText, returnToBF));
    }
    static IEnumerator FixLinesEnumerator(Text UIText, string arText, bool returnToBF)
    {
        RectTransform rt = UIText.GetComponent<RectTransform>();
        Vector3 originalPos = rt.position;
        rt.position = new Vector3(100000, rt.position.y, rt.position.z);

        UIText.text = arText;
        
        //UIText.cachedTextGenerator.Populate(arText, new TextGenerationSettings() { });
        yield return new WaitForEndOfFrame();

        List<int> linesStart = new List<int>();
        foreach (UILineInfo l in UIText.cachedTextGenerator.lines)
            linesStart.Add(l.startCharIdx);

        List<string> lines = new List<string>();
        for (int i = 0; i < linesStart.Count; i++)
        {
            if (i < linesStart.Count - 1)
                lines.Add(UIText.text.Substring(linesStart[i], linesStart[i + 1] - linesStart[i] - 1));
            else
                lines.Add(UIText.text.Substring(linesStart[i]));
        }

		bool originalBestFit = UIText.resizeTextForBestFit;
		VerticalWrapMode originalVWrapMode = UIText.verticalOverflow;
		HorizontalWrapMode originalHWrapMode = UIText.horizontalOverflow;

        UIText.resizeTextForBestFit = false;
        UIText.verticalOverflow = VerticalWrapMode.Overflow;
        UIText.horizontalOverflow = HorizontalWrapMode.Overflow;

        UIText.fontSize = UIText.cachedTextGenerator.fontSizeUsedForBestFit;

        string finalString = "";
        for (int i = 0; i < lines.Count; i++)
        {
            finalString += ArabicSupport.ArabicFixer.Fix(lines[i], true, false);
            if (i < lines.Count - 1)
                finalString += '\n';
        }
        UIText.text = finalString;
        rt.position = originalPos;

        if (returnToBF)
		    UIText.resizeTextForBestFit = originalBestFit;

        //UIText.Rebuild(CanvasUpdate.Prelayout);
        //UIText.verticalOverflow = originalVWrapMode;
        //UIText.horizontalOverflow = originalHWrapMode;

    }
}
