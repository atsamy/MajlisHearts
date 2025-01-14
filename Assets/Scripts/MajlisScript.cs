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

    public ParticleSystem DustParticles;
    public ParticleSystem SparkleParticles;

    public Material GlowMaterial;

    public Action TaskFinished;

    private void Awake()
    {
        Instance = this;
        cameraHover = Camera.main.GetComponent<CameraHover>();
    }

    private void Start()
    {
        SetMyMajlis();
    }

    public void SetMyMajlis()
    {
        AdjustMajlis(TasksManager.Instance.FinishedTasks);
    }

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
                    editableItem.ChangeItem(task.SelectedIndex,false);
                    editableItem.SetModified(task.SelectedIndex,false);
                    Collider2D[] colliders = editableItem.GetComponents<Collider2D>();

                    foreach (var item in colliders)
                    {
                        item.enabled = true;
                    }
                    break;
                case ActionType.Add:
                    EditableItem fixItem = RoomItems.First(a => a.RoomId == task.TargetArea).EditableItems.First(a => a.Code == task.TargetItem);
                    fixItem.ChangeItem(0,false);
                    //fixItem.SetModified(0);
                    break;
            }
        }
    }

    public void ResetTask(List<FinishedTask> tasks)
    {
        if (tasks == null)
            return;

        foreach (var task in tasks)
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
        if (task.Cost > GameManager.Instance.Gems)
        {
            MenuManager.Instance.OpenPopup("nogems",true,false);
            return;
        }

        switch (task.ActionType)
        {
            case ActionType.Clean:
                CleanRoom(task);
                break;
            case ActionType.Change:
                ShowEditableItem(task,true,0);
                break;
            case ActionType.Add:
                FixItem(task);
                break;
        }
    }

    public void ShowEditableItem(TaskData task,bool isNew,int index)
    {
        EditableItem editableItem = RoomItems.First(a => a.RoomId == task.TargetArea).EditableItems.First(a => a.Code == task.TargetItem);

        if (isNew)
        {
            cameraHover.GoToLocation(editableItem.transform, () =>
            {
                taskPanel.OpenEditPanel(editableItem, task, TaskFinished, 0,isNew);
            });
        }
        else
        {
            MenuManager.Instance.HideMain(false, true);
            //StartCoroutine(OpenEditPanel(editableItem,task,isNew));
            taskPanel.OpenEditPanel(editableItem, task, TaskFinished, index,isNew);
        }

        SFXManager.Instance.PlayClip("Select");
    }

    //IEnumerator OpenEditPanel(EditableItem editableItem, TaskData task, bool isNew)
    //{
    //    yield return new WaitForFixedUpdate();

    //}

    private void CleanRoom(TaskData task)
    {
        CleanAnimation oldItems = RoomItems.First(a => a.RoomId == task.TargetArea)
            .OldItems.First(a => a.Code == task.TargetItem);

        cameraHover.GoToLocation(oldItems.transform, () =>
        {
            oldItems.Clean(()=>
            {
                MenuManager.Instance.ShowMain();
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
        taskPanel.TaskDone(tasks,true);
    }

    void FixItem(TaskData task)
    {
        EditableItem editableItem = RoomItems.First(a => a.RoomId == task.TargetArea).EditableItems.First(a => a.Code == task.TargetItem);

        cameraHover.GoToLocation(editableItem.transform, () =>
        {
            MenuManager.Instance.ShowMain();
            //taskPanel.OpenEditPanel(editableItem, target, TaskFinished);
            editableItem.ChangeItem(0,true);
            TaskFinished?.Invoke();
            SFXManager.Instance.PlayClip("Confirm");
        });

        taskPanel.ClosePanel();
        FinishedTask tasks = new FinishedTask()
        {
            ActionType = ActionType.Add,
            TargetArea = task.TargetArea,
            TargetItem = task.TargetItem
        };
        taskPanel.TaskDone(tasks,true);
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


