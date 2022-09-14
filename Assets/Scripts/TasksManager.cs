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

    public void TaskFinished(FinishedTask task,bool newTask)
    {
        FinishedTasks.Add(task);

        if(newTask)
            currentIndex++;

        Dictionary<string, string> data = new Dictionary<string, string>();
        data.Add("TaskIndex", currentIndex.ToString());

        Wrapper<FinishedTask> wrappedCustomization = new Wrapper<FinishedTask>();
        wrappedCustomization.array = FinishedTasks.ToArray();

        data.Add("Customization", JsonUtility.ToJson(wrappedCustomization));

        //toggle save data from here
        PlayfabManager.instance.SetPlayerData(data);
    }
}

[Serializable]
public class FinishedTask
{
    public ActionType ActionType;
    public string TargetItem;
    public string TargetArea;
    public int SelectedIndex;
}

[Serializable]
public class TaskData
{
    public string ActionName;
    public ActionType ActionType;
    public string TargetItem;
    public string TargetArea;
    public int Cost;
}

public enum ActionName
{
    Clean = 0,
    Change = 1,
    Fix = 2,
    Add = 3,
    Install = 4,
    Remove = 5,
    Hang = 6,
    Lay
}

public enum ActionType
{
    Clean = 0,
    Change = 1,
    Add = 2
}
