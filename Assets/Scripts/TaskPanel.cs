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

        taskItem.Set(LanguageManager.Instance.GetString(currentTask.ID), currentTask.Cost, () =>
        {
            MajlisScript.Instance.ExecuteTask(currentTask);
        });
    }

    public void Open()
    {
        taskPanel.SetActive(true);
        MenuManager.Instance.CloseMain();
    }

    public void Close()
    {
        MenuManager.Instance.CameraHover.Unlock();
        taskPanel.SetActive(false);
        MenuManager.Instance.OpenMain();
    }

    internal void OpenEditPanel(EditableItem editableItem,string target)
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
        //Destroy(taskItem.gameObject);
        InitTask();
    }
}
