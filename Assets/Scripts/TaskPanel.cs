using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TaskPanel : MonoBehaviour
{
    //[SerializeField]
    //Transform content;
    [SerializeField]
    TaskItemScript taskItem;
    [SerializeField]
    GameObject taskPanel;
    [SerializeField]
    ItemSelectPanel editPanel;

    //bool isNew;
    //[SerializeField]
    //CameraHover cameraHover;
    //TaskItemScript currentTaskItem;
    int currentCost;

    private void Awake()
    {
        InitTask();
    }

    private void InitTask()
    {
        if (TasksManager.Instance.tasksCompleted)
            return;

        //currentTaskItem = Instantiate(taskItem, content).GetComponent<TaskItemScript>();
        TaskData currentTask = TasksManager.Instance.CurrentTask;

        string taskName = LanguageManager.Instance.GetString(currentTask.ActionName);

        string itenName;
        string count = "";
        bool isMulti;


        if (isMulti = currentTask.TargetItem.Contains("#"))
        {
            isMulti = true;
            string[] data = currentTask.TargetItem.Split("#");
            count = data[1];
            itenName = data[0];
        }
        else
        {
            itenName = currentTask.TargetItem;
        }

        if (currentTask.ActionType == ActionType.Clean)
        {
            taskName = taskName.Replace("%R", LanguageManager.Instance.GetString(itenName));
        }
        else
        {

            taskName = taskName.Replace("%R", LanguageManager.Instance.GetString(currentTask.TargetArea))
                .Replace("%I", LanguageManager.Instance.GetString(itenName));
        }

        if (isMulti)
        {
            if (LanguageManager.Instance.CurrentLanguage == Language.English)
                taskName += " " + count;
            else
                taskName = count + " " + taskName;
        }

        Sprite taskSprite = null;

        if (currentTask.ActionType != ActionType.Clean)
        {
            IJsonTask editableItem = MajlisScript.Instance.RoomItems.First(a => a.RoomId == currentTask.TargetArea).EditableItems.First(a => a.Code == currentTask.TargetItem);
            taskSprite = editableItem.GetTaskIcon();
        }
        else
        {
            IJsonTask editableItem = MajlisScript.Instance.RoomItems.First(a => a.RoomId == currentTask.TargetArea).OldItems.First(a => a.Code == currentTask.TargetItem);
            taskSprite = editableItem.GetTaskIcon();
        }

        taskItem.Set(taskName, currentTask.Cost, taskSprite, () =>
        {
            MajlisScript.Instance.ExecuteTask(currentTask);
        });

        currentCost = currentTask.Cost;
    }

    public void Open()
    {
        taskPanel.SetActive(true);
        MenuManager.Instance.CloseMain();

        SFXManager.Instance.PlayClip("Open");
    }

    public void Close()
    {
        ClosePanel();
        MenuManager.Instance.OpenMain();
    }

    public void ClosePanel()
    {
        MenuManager.Instance.CameraHover.Unlock();
        taskPanel.SetActive(false);
        SFXManager.Instance.PlayClip("Close");
    }

    internal void OpenEditPanel(EditableItem editableItem,TaskData taskData,Action taskFinished,bool isNew)
    {
        taskPanel.SetActive(false);

        editPanel.Show(ref editableItem,isNew, (index) =>
        {
            FinishedTask task = new FinishedTask()
            {
                ActionType = ActionType.Change,
                TargetArea = taskData.TargetArea,
                TargetItem = taskData.TargetItem,
                SelectedIndex = index
            };

            TaskDone(task,isNew);
            taskFinished?.Invoke();
            //save edit
            MenuManager.Instance.OpenMain();
        }, () => 
        {
            MenuManager.Instance.OpenMain();
        });
    }

    public void TaskDone(FinishedTask task,bool isNew)
    {
        MenuManager.Instance.CameraHover.Unlock();
        TasksManager.Instance.TaskFinished(task,isNew);
        taskItem.gameObject.SetActive(false);

        GameManager.Instance.DeductCurrency(currentCost);
        //Destroy(taskItem.gameObject);
        InitTask();
    }
}
