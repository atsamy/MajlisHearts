using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasFixer : MonoBehaviour
{

    void Start()
    {
        GetComponent<CanvasScaler>().matchWidthOrHeight = ((float)Screen.width / (float)Screen.height) > 1.7f ? 1 : 0;
    }
}
