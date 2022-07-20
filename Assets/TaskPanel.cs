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
    GameObject panel;


    private void Awake()
    {
        TaskItemScript task = Instantiate(taskItem, content).GetComponent<TaskItemScript>();
        TaskData currentTask = TasksManager.Instance.CurrentTask;
        
        task.Set(LanguageManager.Instance.GetString(currentTask.ID), currentTask.Cost, () => 
        {
            TasksManager.Instance.ExcuteTask();
        });
    }

    public void Open()
    {
        panel.SetActive(true);   
    }
}
