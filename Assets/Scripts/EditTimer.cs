using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditTimer : MonoBehaviour
{
    [SerializeField]
    Image[] bars;


    public void Fill(float time)
    {
        int index = Mathf.Min(3,Mathf.FloorToInt(time * 4));
        bars[index].color = new Color(1,1,1,time * 4 - index);
    }

    public void Hide()
    {
        foreach (var item in bars)
        {
            item.color = new Color(1,1,1,0);
        }
    }

    internal void SetPosition(Vector3 clickPos)
    {
        transform.position = clickPos + Vector3.up * 100;
    }
}
