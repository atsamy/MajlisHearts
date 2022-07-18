using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookToWorldObject : MonoBehaviour
{
    Camera myCamera;
    [SerializeField]
    Transform worldObject;

    private void Awake()
    {
        myCamera = Camera.main;
    }
    void Update()
    {
        transform.position = myCamera.WorldToScreenPoint(worldObject.position);
    }
}
