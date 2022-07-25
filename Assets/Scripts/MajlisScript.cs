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
                GameObject oldItems = RoomItems.First(a => a.RoomId == task.Target).OldItems;
                cameraHover.GoToLocation(oldItems.transform, () => 
                {
                    oldItems.SetActive(false);
                });
                taskPanel.Close();
                FinishedTask tasks = new FinishedTask()
                {
                    ActionType = TaskAction.Clean,
                    Target = task.Target
                };
                taskPanel.TaskDone(tasks);
                break;
            case TaskAction.Change:
                string[] ids = task.Target.Split('_');
                EditableItem editableItem = RoomItems.First(a => a.RoomId == ids[0]).EditableItems.First(a => a.Code == task.Target);

                cameraHover.GoToLocation(editableItem.transform, () => 
                {
                    taskPanel.OpenEditPanel(editableItem,task.Target);
                });
                
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


