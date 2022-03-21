using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHover : MonoBehaviour
{
    Vector3 startPosition;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
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
}
