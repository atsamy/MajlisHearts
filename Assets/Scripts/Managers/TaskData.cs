using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
