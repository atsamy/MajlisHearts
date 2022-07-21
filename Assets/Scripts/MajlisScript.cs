using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class MajlisScript : MonoBehaviour
{
    public static MajlisScript Instance;
    public RoomItem[] RoomItems;
    CameraHover cameraHover;
    [SerializeField]
    TaskPanel taskPanel;

    private void Awake()
    {
        Instance = this;
        cameraHover = Camera.main.GetComponent<CameraHover>();
    }

    public void ExecuteTask(TaskData task)
    {
        switch (task.ActionType)
        {
            case TaskAction.Clean:
                RoomItems.First(a => a.RoomId == task.Target).OldItems.SetActive(false);
                taskPanel.Close();
                break;
            case TaskAction.Change:
                string[] ids = task.Target.Split('_');
                EditableItem editableItem = RoomItems.First(a => a.RoomId == ids[0]).EditableItems.First(a => a.Code == task.Target);
                cameraHover.GoToLocation(editableItem.transform);
                taskPanel.OpenEditPanel(editableItem);
                break;
        }
    }
}
[Serializable]
public class RoomItem
{
    public string RoomId;
    public GameObject OldItems;
    public EditableItem[] EditableItems;
}


