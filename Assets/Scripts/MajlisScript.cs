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

    public Action TaskFinished;

    private void Awake()
    {
        Instance = this;
        cameraHover = Camera.main.GetComponent<CameraHover>();
    }

    private void Start()
    {
        AdjustMajlis(TasksManager.Instance.FinishedTasks);
    }

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.R))
    //        ResetTask(TasksManager.Instance.FinishedTasks);
    //}

    public void AdjustMajlis(List<FinishedTask> finishedTasks)
    {
        foreach (var task in finishedTasks)
        {
            switch (task.ActionType)
            {
                case ActionType.Clean:
                    RoomItems.First(a => a.RoomId == task.TargetArea).OldItems.First(a => a.Code == task.TargetItem).gameObject.SetActive(false);
                    break;
                case ActionType.Change:
                    EditableItem editableItem = RoomItems.First(a => a.RoomId == task.TargetArea).EditableItems.First(a => a.Code == task.TargetItem);
                    editableItem.ChangeItem(task.SelectedIndex);
                    editableItem.SetOriginal();
                    break;
                case ActionType.Add:
                    EditableItem fixItem = RoomItems.First(a => a.RoomId == task.TargetArea).EditableItems.First(a => a.Code == task.TargetItem);
                    fixItem.ChangeItem(0);
                    break;
            }
        }
    }

    public void ResetTask()
    {
        foreach (var task in TasksManager.Instance.FinishedTasks)
        {
            switch (task.ActionType)
            {
                case ActionType.Clean:
                    CleanAnimation oldItems = RoomItems.First(a => a.RoomId == task.TargetArea).OldItems.First(a => a.Code == task.TargetItem);
                    oldItems.Reset();
                    break;
                case ActionType.Change:
                    ResetEditableItem(task);
                    break;
                case ActionType.Add:
                    ResetEditableItem(task);
                    break;
            }
        }
    }

    private void ResetEditableItem(FinishedTask task)
    {
        EditableItem editableItem = RoomItems.First(a => a.RoomId == task.TargetArea).EditableItems.First(a => a.Code == task.TargetItem);
        editableItem.ResetToOriginal();
    }

    public void ExecuteTask(TaskData task)
    {
        switch (task.ActionType)
        {
            case ActionType.Clean:
                CleanRoom(task);
                break;
            case ActionType.Change:
                ShowEditableItem(task);
                break;
            case ActionType.Add:
                FixItem(task);
                break;
        }
    }

    public void ShowEditableItem(TaskData task)
    {
        EditableItem editableItem = RoomItems.First(a => a.RoomId == task.TargetArea).EditableItems.First(a => a.Code == task.TargetItem);

        cameraHover.GoToLocation(editableItem.transform, () =>
        {
            taskPanel.OpenEditPanel(editableItem, task , TaskFinished);
        });

        SFXManager.Instance.PlayClip("Select");
    }

    private void CleanRoom(TaskData task)
    {
        CleanAnimation oldItems = RoomItems.First(a => a.RoomId == task.TargetArea)
            .OldItems.First(a => a.Code == task.TargetItem);

        cameraHover.GoToLocation(oldItems.transform, () =>
        {
            oldItems.Clean(()=>
            {
                MenuManager.Instance.OpenMain();
                TaskFinished?.Invoke();
            });
        });
        taskPanel.ClosePanel();
        FinishedTask tasks = new FinishedTask()
        {
            ActionType = ActionType.Clean,
            TargetArea = task.TargetArea,
            TargetItem = task.TargetItem
        };
        taskPanel.TaskDone(tasks);
    }

    void FixItem(TaskData task)
    {
        EditableItem editableItem = RoomItems.First(a => a.RoomId == task.TargetArea).EditableItems.First(a => a.Code == task.TargetItem);

        cameraHover.GoToLocation(editableItem.transform, () =>
        {
            MenuManager.Instance.OpenMain();
            //taskPanel.OpenEditPanel(editableItem, target, TaskFinished);
            editableItem.ChangeItem(0);
            TaskFinished?.Invoke();
        });

        taskPanel.ClosePanel();
        FinishedTask tasks = new FinishedTask()
        {
            ActionType = ActionType.Add,
            TargetArea = task.TargetArea,
            TargetItem = task.TargetItem
        };
        taskPanel.TaskDone(tasks);
        //SFXManager.Instance.PlayClip("Select");
    }

    public void RetrieveItems()
    {
        RoomItems = new RoomItem[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            RoomItems[i] = new RoomItem();
            RoomItems[i].RoomId = transform.GetChild(i).name;
            RoomItems[i].OldItems = transform.GetChild(i).GetComponentsInChildren<CleanAnimation>();
            RoomItems[i].EditableItems = transform.GetChild(i).GetComponentsInChildren<EditableItem>();
        }
    }
}
[Serializable]
public class RoomItem
{
    public string RoomId;
    public CleanAnimation[] OldItems;
    public EditableItem[] EditableItems;
}


