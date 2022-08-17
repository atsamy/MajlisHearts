using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ArabicSupport;
public class FixEntryText : MonoBehaviour
{
    public Text text;
    // Start is called before the first frame update
    void Start()
    {
        //text = GetComponent<Text>();
    }

    public void Fix(string value)
    {
        text.text = ArabicFixer.Fix(value,false,false);
    }
}
