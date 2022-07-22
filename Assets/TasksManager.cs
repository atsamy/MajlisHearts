using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TasksManager : MonoBehaviour
{
    public static TasksManager Instance;
    public TaskData[] Tasks;
    int currentIndex;

    public TaskData CurrentTask { get => Tasks[currentIndex]; }
    public bool tasksCompleted { get => currentIndex >= Tasks.Length; }
    private void Awake()
    {
        Instance = this;
    }

    internal void SetIndex(int value)
    {
        currentIndex = value;
    }

    public void TaskFinished()
    {
        currentIndex++;
        Dictionary<string, string> data = new Dictionary<string, string>();
        data.Add("TaskIndex", currentIndex.ToString());
        PlayfabManager.instance.SetPlayerData(data);
    }
}
