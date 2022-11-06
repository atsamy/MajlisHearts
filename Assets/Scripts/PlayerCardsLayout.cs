using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerCardsLayout : MonoBehaviour
{
    [SerializeField]
    int spacing;
    [SerializeField]
    float rotationFactor = 2;
    [SerializeField]
    float rotation = 0;

    public bool IsVertical;

    public void SetLocations()
    {
        float count = transform.childCount;

        for (int i = 0; i < count; i++)
        {
            float location = i - ((count - 1) / 2);

            float y = Mathf.Abs(location) * - (2 + Mathf.Abs(location)) * (rotationFactor / 1.5f);
            float x = location * spacing;
            float rot = location * -rotationFactor + rotation;

            transform.GetChild(i).DOLocalMove(new Vector3(IsVertical?y:x,IsVertical?x:y, 0),0.15f);
            transform.GetChild(i).DORotate(new Vector3(0, 0, rot), 0.15f);
        }
    }
}
