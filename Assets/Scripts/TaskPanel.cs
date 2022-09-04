using System;
using System.Collections;
using System.Collections.Generic;
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

        if (currentTask.ActionType == ActionType.Clean)
        {
            taskName = taskName.Replace("%R", LanguageManager.Instance.GetString(currentTask.TargetItem));
        }
        else
        {
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

            taskName = taskName.Replace("%R", LanguageManager.Instance.GetString(currentTask.TargetArea))
                .Replace("%I", LanguageManager.Instance.GetString(itenName));

            if (isMulti)
            {
                if(LanguageManager.Instance.CurrentLanguage == Language.English)
                    taskName += " " + count;
                else
                    taskName = count + " " + taskName;
            }
        }

        taskItem.Set(taskName, currentTask.Cost, () =>
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

    internal void OpenEditPanel(EditableItem editableItem,TaskData taskData,Action taskFinished)
    {
        taskPanel.SetActive(false);

        editPanel.Show(ref editableItem, (index) =>
        {
            FinishedTask task = new FinishedTask()
            {
                ActionType = ActionType.Change,
                TargetArea = taskData.TargetArea,
                TargetItem = taskData.TargetItem,
                SelectedIndex = index
            };

            TaskDone(task);
            taskFinished?.Invoke();
            //save edit
            MenuManager.Instance.OpenMain();
        }, () => 
        {
            MenuManager.Instance.OpenMain();
        });
    }

    public void TaskDone(FinishedTask task)
    {
        MenuManager.Instance.CameraHover.Unlock();
        TasksManager.Instance.TaskFinished(task);
        taskItem.gameObject.SetActive(false);

        GameManager.Instance.DeductCurrency(currentCost);
        //Destroy(taskItem.gameObject);
        InitTask();
    }
}
