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

        string taskName = LanguageManager.Instance.GetString(currentTask.ID.Split("_")[0]);
        if (currentTask.ActionType == TaskAction.Clean)
        {
            taskName = taskName.Replace("%R", LanguageManager.Instance.GetString(currentTask.Target));
        }
        else
        {
            string[] data = currentTask.Target.Split("_");
            taskName = taskName.Replace("%R", LanguageManager.Instance.GetString(data[0]))
                .Replace("%I", LanguageManager.Instance.GetString(data[1]));
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

    internal void OpenEditPanel(EditableItem editableItem,string target,Action taskFinished)
    {
        taskPanel.SetActive(false);

        editPanel.Show(ref editableItem, (index) =>
        {
            FinishedTask task = new FinishedTask()
            {
                ActionType = TaskAction.Change,
                Target = target + "_" + index
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
