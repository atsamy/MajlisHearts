using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TasksManager : MonoBehaviour
{
    public static TasksManager Instance;
    [HideInInspector]
    public TaskData[] Tasks;
    [HideInInspector]
    public List<FinishedTask> FinishedTasks;
    //int currentIndex;
    public TaskData CurrentTask { get => Tasks[FinishedTasks.Count]; }
    public bool tasksCompleted { get => FinishedTasks.Count >= Tasks.Length; }

    private void Awake()
    {
        Instance = this;
    }

    public void TaskFinished(FinishedTask task,bool newTask)
    {
        if (newTask)
        {
            FinishedTasks.Add(task);
        }
        else
        {
            FinishedTasks.Find(a => a.SameTask(task)).SelectedIndex = task.SelectedIndex;
        }

        //if(newTask)
        //    currentIndex++;

        Dictionary<string, string> data = new Dictionary<string, string>();
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

    public bool SameTask(FinishedTask task)
    {
        return task.TargetItem == TargetItem && task.TargetArea == TargetArea;
    }
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
    Lay = 7
}

public enum ActionType
{
    Clean = 0,
    Change = 1,
    Add = 2
}
