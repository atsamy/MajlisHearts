using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class MajlisScript : MonoBehaviour
{
    public static MajlisScript Instance;
    public RoomItem[] RoomItems;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {

    }
}
[Serializable]
public class RoomItem
{
    public string RoomId;
    public GameObject OldItems;
    public EditableItem[] EditableItems;
}


