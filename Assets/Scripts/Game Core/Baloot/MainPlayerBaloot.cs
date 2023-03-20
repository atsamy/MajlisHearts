using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Services.Analytics;
using UnityEditor.SearchService;
using UnityEngine;

public class MainPlayerBaloot : PlayerBaloot
{
    public event Action WaitOthers;
    public event Action<int> OnCheckDouble;
    public event Action OnCancelDouble;

    public delegate void WaitSelectType(RoundScriptBaloot roundScript);
    public event WaitSelectType OnWaitSelectType;

    Dictionary<Projects, int> ProjectsCount;

    public MainPlayerBaloot(int index) : base(index)
    {
        isPlayer= false;
        ProjectsCount = new();
    }

    public override void Reset()
    {
        base.Reset();
        ProjectsCount.Clear();
    }

    public override void CheckDouble(int value)
    {
        OnCheckDouble?.Invoke(value);
    }



    public void AddProject(Projects project, int count)
    {
        if (count == 0)
        {
            ProjectsCount.Remove(project);
            return;
        }
        ProjectsCount[project] = count;
    }

    public override void ChooseProjects(BalootGameType type)
    {
        foreach (var item in ProjectsCount)
        {
            for (int i = 0; i < item.Value; i++)
            {
                CheckForProject(type, item.Key);
            }
        }
    }

    internal override void CancelDouble()
    {
        OnCancelDouble?.Invoke();
    }

    protected override void WaitForOthers()
    {
        WaitOthers?.Invoke();
    }

    public override void CheckGameType(RoundScriptBaloot roundScript)
    {
        OnWaitSelectType?.Invoke(roundScript);
    }

    internal void ForcePlay()
    {
        OnForcePlay?.Invoke();
    }

    public override void SetTurn(RoundInfo info)
    {
        base.SetTurn(info);
    }
}
