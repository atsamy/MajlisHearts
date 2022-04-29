using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerCardsLayout : MonoBehaviour
{
    [SerializeField]
    int spacing;
    // Start is called before the first frame update
    void Start()
    {

    }

    public void SetLocations()
    {
        float count = transform.childCount;

        for (int i = 0; i < count; i++)
        {
            float location = i - ((count - 1) / 2);

            float y = Mathf.Abs(location) * - (2 + Mathf.Abs(location));
            float x = location * spacing;
            float rot = location * -2;

            transform.GetChild(i).DOLocalMove(new Vector3(x, y, 0),0.15f);
            transform.GetChild(i).DORotate(new Vector3(0, 0, rot), 0.15f);
        }
    }
}
