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

    public void SetLocations(float speed)
    {
        float count = transform.childCount;
        //print(count);
        for (int i = 0; i < count; i++)
        {
            float location = i - ((count - 1) / 2);

            float y = Mathf.Abs(location) * -(2 + Mathf.Abs(location)) * (rotationFactor / 1.5f) * (IsVertical ? -1 : 1);
            float x = location * spacing;
            float rot = location * -rotationFactor + rotation;

            transform.GetChild(i).DOLocalMove(new Vector3(IsVertical ? y : x, IsVertical ? x : y, 0), speed);
            transform.GetChild(i).DORotate(new Vector3(0, 0, rot), speed);
        }
    }

    public void SetLocations()
    {
        SetLocations(0.15f);
    }

    public void RemoveCards()
    {
        foreach (Transform item in transform)
        {
            Destroy(item.gameObject);
        }
    }
}
