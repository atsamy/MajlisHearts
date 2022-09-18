using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ArabicSupport;

public class ArabicFix : MonoBehaviour 
{
	void Awake () 
	{
		Text text;
		if ((text = GetComponent<Text>()) != null)
		{
			text.text = ArabicFixer.Fix(text.text);
		}
	}
}
