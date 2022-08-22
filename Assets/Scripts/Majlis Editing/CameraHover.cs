using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using System;

public class CameraHover : MonoBehaviour
{
    Vector3 startPosition;
    bool locked;

    //Vector3 originalPosition;
    //Quaternion originalRotation;

    [SerializeField]
    Vector2 minBounds;
    [SerializeField]
    Vector2 maxBounds;

    [SerializeField]
    float hoverSpeed = 5;

    void LateUpdate()
    {
        if (locked)
            return;

        if (IsPointerOverUIObject())
            return;

        if (Input.GetMouseButtonDown(0))
        {
            startPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            Vector3 mousePosition = new Vector3(Input.mousePosition.x - startPosition.x, Input.mousePosition.y - startPosition.y, 0);
            transform.position += mousePosition.normalized * Time.deltaTime * hoverSpeed;

            transform.position = new Vector3(Mathf.Clamp(transform.position.x,minBounds.x, maxBounds.x),
                Mathf.Clamp(transform.position.y,minBounds.y, maxBounds.y), -20);
        }
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    public static bool IsPointerOverGameObject()
    {
        // Check mouse
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return true;
        }

        // Check touches
        for (int i = 0; i < Input.touchCount; i++)
        {
            var touch = Input.GetTouch(i);
            if (touch.phase == TouchPhase.Began)
            {
                if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void GoToLocation(Transform Location,Action onArrived)
    {
        //originalPosition = transform.position;
        //originalRotation = transform.rotation;

        locked = true;
        Vector3 tagetLocation = Location.position;
        tagetLocation.z = -20;

        transform.DOMove(tagetLocation, 0.5f).SetEase(Ease.InOutCubic).OnComplete(() => 
        {
            onArrived?.Invoke();
        });
        //transform.DORotateQuaternion(Location.rotation, 0.5f);
    }

    public void Unlock()
    {
        locked = false;
        //transform.DOMove(originalPosition, 0.5f).SetEase(Ease.InOutCubic);
        //transform.DORotateQuaternion(originalRotation, 0.5f);
    }
}
