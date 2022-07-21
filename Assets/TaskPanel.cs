using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskPanel : MonoBehaviour
{
    [SerializeField]
    Transform content;
    [SerializeField]
    GameObject taskItem;
    [SerializeField]
    GameObject taskPanel;
    [SerializeField]
    ItemSelectPanel editPanel;


    private void Awake()
    {
        TaskItemScript task = Instantiate(taskItem, content).GetComponent<TaskItemScript>();
        TaskData currentTask = TasksManager.Instance.CurrentTask;
        
        task.Set(LanguageManager.Instance.GetString(currentTask.ID), currentTask.Cost, () => 
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
        MenuManager.Instance.OpenMain();
    }

    internal void OpenEditPanel(EditableItem editableItem)
    {
        taskPanel.SetActive(false);

        editPanel.Show(editableItem, (index) => 
        {
            //save edit
            MenuManager.Instance.OpenMain();
        }, () => 
        {
            MenuManager.Instance.OpenMain();
        });
    }
}
