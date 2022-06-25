using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditableItem : MonoBehaviour
{
    public string Code;
    public GameObject Model;
    public Transform CameraLocation;

    [HideInInspector]
    public string SelectedID;
    private void OnMouseDown()
    {
        EditorUI.Instance.ShowItems(Code);
    }
}
