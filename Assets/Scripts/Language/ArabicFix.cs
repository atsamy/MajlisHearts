using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ArabicSupport;
using TMPro;
public class ArabicFix : MonoBehaviour 
{
	void Awake () 
	{
		if (GetComponent<Text>() != null)
		{
			Text text = GetComponent<Text>();
			text.text = ArabicFixer.Fix(text.text);
		}
		else
		{
			TextMeshProUGUI text = GetComponent<TextMeshProUGUI>();
			text.text = ArabicFixer.Fix(text.text);
		}
	}
}
