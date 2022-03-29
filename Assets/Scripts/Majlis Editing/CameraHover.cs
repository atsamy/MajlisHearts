using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class CameraHover : MonoBehaviour
{
    Vector3 startPosition;
    bool locked;

    Vector3 originalPosition;
    Quaternion originalRotation;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (locked)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            startPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            

            Vector3 mousePosition = new Vector3(Input.mousePosition.x - startPosition.x, 0, Input.mousePosition.y - startPosition.y);

            transform.position += mousePosition.normalized * Time.deltaTime * 5;
        }
    }

    public void GoToLocation(Transform Location)
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;

        locked = true;
        transform.DOMove(Location.position, 0.5f).SetEase(Ease.InOutCubic);
        transform.DORotateQuaternion(Location.rotation,0.5f);
    }

    public void GoBack()
    {
        locked = false;
        transform.DOMove(originalPosition, 0.5f).SetEase(Ease.InOutCubic);
        transform.DORotateQuaternion(originalRotation, 0.5f);
    }
}
