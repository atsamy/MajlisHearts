using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using System;

public class CameraHover : MonoBehaviour
{
    //Vector3 startPosition;
    bool locked;

    //Vector3 originalPosition;
    //Quaternion originalRotation;

    [SerializeField]
    Vector2 minBounds;
    [SerializeField]
    Vector2 maxBounds;

    Vector3 prevPos;

    Vector3 velocity;

    //[SerializeField]
    //float hoverSpeed = 5;
    private Camera mainCamera;

    float prevDistance;
    float NewTime = 0;

    bool firstTouch = true;
    bool doubleTouch;
    float startSize = 0;
    public float MaxDoubleTapTime = 0.5f;
    int TapCount;

    private void Start()
    {
        mainCamera = GetComponent<Camera>();
        TapCount = 0;
    }

    void LateUpdate()
    {
        if (locked)
            return;

        if (IsPointerOverUIObject())
            return;

        if (Input.touchCount >= 2)
        {
            Vector2 touch0 = Input.GetTouch(0).position;
            Vector2 touch1 = Input.GetTouch(1).position;
            float distance = Vector2.Distance(touch0, touch1);

            doubleTouch = true;

            if (firstTouch)
            {
                prevDistance = distance;
                firstTouch = false;

                startSize = mainCamera.orthographicSize;
            }

            mainCamera.orthographicSize = Mathf.Clamp(startSize + ((prevDistance - distance) / 50), 3, 10);
            return;
        }
        else if (Input.touchCount == 0)
            doubleTouch = false;

        firstTouch = true;

        if (doubleTouch)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            prevPos = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            Vector3 mousePosition = new Vector3(Input.mousePosition.x - prevPos.x, Input.mousePosition.y - prevPos.y, 0);
            float cameraHeight = mainCamera.orthographicSize * 2 / Screen.height;
            float cameraWidth = mainCamera.orthographicSize * 2 * mainCamera.aspect / Screen.width;
            Vector3 moveDelta = new Vector3(mousePosition.x * cameraWidth, mousePosition.y * cameraHeight, 0);

            transform.position -= moveDelta;
            prevPos = Input.mousePosition;
            velocity = mousePosition;

            transform.position = new Vector3(Mathf.Clamp(transform.position.x, minBounds.x, maxBounds.x),
                Mathf.Clamp(transform.position.y, minBounds.y, maxBounds.y), -20);
        }
        else
        {
            if (velocity.magnitude > 0.1f)
            {
                transform.position -= velocity * Time.deltaTime;
                velocity -= velocity * Time.deltaTime * 5;

                transform.position = new Vector3(Mathf.Clamp(transform.position.x, minBounds.x, maxBounds.x),
                    Mathf.Clamp(transform.position.y, minBounds.y, maxBounds.y), -20);
            }
        }
    }

    void Update()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Ended)
            {
                TapCount += 1;
            }

            if (TapCount == 1)
            {

                NewTime = Time.time + MaxDoubleTapTime;
            }
            else if (TapCount == 2 && Time.time <= NewTime)
            {

                //Whatever you want after a dubble tap    
                print("Dubble tap");
                if (mainCamera.orthographicSize != 6)
                {
                    mainCamera.DOOrthoSize(6, 0.5f).SetEase(Ease.InCirc);
                }

                TapCount = 0;
            }

        }
        if (Time.time > NewTime)
        {
            TapCount = 0;
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

    public void GoToLocation(Transform Location, Action onArrived)
    {
        //originalPosition = transform.position;
        //originalRotation = transform.rotation;

        locked = true;
        Vector3 tagetLocation = Location.position;
        tagetLocation.z = -20;
        tagetLocation.y -= 1;

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
