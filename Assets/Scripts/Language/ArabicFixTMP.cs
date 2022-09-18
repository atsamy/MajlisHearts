using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ArabicSupport;
using TMPro;

public class ArabicFixTMP : MonoBehaviour 
{
	void Awake () 
	{
		TextMeshProUGUI text;
		if ((text = GetComponent<TextMeshProUGUI>()) != null)
		{
			text.text = ArabicFixer.Fix(text.text);
		}
	}
}
