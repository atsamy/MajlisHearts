using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TasksManager : MonoBehaviour
{
    public static TasksManager Instance;
    [HideInInspector]
    public TaskData[] Tasks;
    [HideInInspector]
    public List<FinishedTask> FinishedTasks;
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

    public void TaskFinished(FinishedTask task)
    {
        FinishedTasks.Add(task);
        currentIndex++;
        Dictionary<string, string> data = new Dictionary<string, string>();
        data.Add("TaskIndex", currentIndex.ToString());

        Wrapper<FinishedTask> wrappedCustomization = new Wrapper<FinishedTask>();
        wrappedCustomization.array = FinishedTasks.ToArray();

        data.Add("Customization", JsonUtility.ToJson(wrappedCustomization));
        //PlayfabManager.instance.SetPlayerData(data);
    }
}

[Serializable]
public class FinishedTask
{
    public TaskAction ActionType;
    public string Target;
}

[Serializable]
public class TaskData
{
    public string ID;
    public int Cost;
    public TaskAction ActionType;
    public string Target;
}

public enum TaskAction
{
    Clean = 0,
    Change = 1
}
