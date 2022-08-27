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
                case TaskAction.Clean:
                    RoomItems.First(a => a.RoomId == task.Target).OldItems.gameObject.SetActive(false);
                    //oldItems.SetActive(false);
                    break;
                case TaskAction.Change:
                    string[] ids = task.Target.Split('_');

                    //if (!RoomItems.First(a => a.RoomId == ids[0]).EditableItems.Any(a => a.Code == (ids[0] + "_" + ids[1])))
                    //{
                    //    print("id: " + (ids[0] + "_" + ids[1]));
                    //    return;
                    //}

                    EditableItem editableItem = RoomItems.First(a => a.RoomId == ids[0]).EditableItems.First(a => a.Code == (ids[0] + "_" + ids[1]));
                    editableItem.ChangeItem(int.Parse(ids[2]));
                    editableItem.SetOriginal();
                    break;
                case TaskAction.Fix:
                    string[] fixids = task.Target.Split('_');
                    EditableItem fixItem = RoomItems.First(a => a.RoomId == fixids[0]).EditableItems.First(a => a.Code == (fixids[0] + "_" + fixids[1]));
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
                case TaskAction.Clean:
                    CleanAnimation oldItems = RoomItems.First(a => a.RoomId == task.Target).OldItems;
                    oldItems.Reset();
                    break;
                case TaskAction.Change:
                    string[] ids = task.Target.Split('_');
                    EditableItem editableItem = RoomItems.First(a => a.RoomId == ids[0]).EditableItems.First(a => a.Code == (ids[0] + "_" + ids[1]));
                    editableItem.ResetToOriginal();
                    break;
            }
        }
    }

    public void ExecuteTask(TaskData task)
    {
        switch (task.ActionType)
        {
            case TaskAction.Clean:
                CleanRoom(task.Target);
                break;
            case TaskAction.Change:
                ShowEditableItem(task.Target);
                break;
            case TaskAction.Fix:
                FixItem(task.Target);
                break;
        }
    }

    public void ShowEditableItem(string target)
    {
        EditableItem editableItem = RoomItems.First(a => a.RoomId == target.Split('_')[0]).EditableItems.First(a => a.Code == target);

        cameraHover.GoToLocation(editableItem.transform, () =>
        {
            taskPanel.OpenEditPanel(editableItem, target, TaskFinished);
        });

        SFXManager.Instance.PlayClip("Select");
    }

    private void CleanRoom(string target)
    {
        CleanAnimation oldItems = RoomItems.First(a => a.RoomId == target).OldItems;
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
            ActionType = TaskAction.Clean,
            Target = target
        };
        taskPanel.TaskDone(tasks);
    }

    void FixItem(string target)
    {
        EditableItem editableItem = RoomItems.First(a => a.RoomId == target.Split('_')[0]).EditableItems.First(a => a.Code == target);

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
            ActionType = TaskAction.Fix,
            Target = target
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
            RoomItems[i].OldItems = transform.GetChild(i).GetComponentInChildren<CleanAnimation>();
            RoomItems[i].EditableItems = transform.GetChild(i).GetComponentsInChildren<EditableItem>();
        }
    }
}
[Serializable]
public class RoomItem
{
    public string RoomId;
    public CleanAnimation OldItems;
    public EditableItem[] EditableItems;
}


