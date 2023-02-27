using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Services.Analytics;
using UnityEngine;

public class MainPlayerBaloot : PlayerBaloot
{
    public event Action WaitOthers;
    public event Action<int> OnCheckDouble;
    public event Action OnCancelDouble;

    public delegate void WaitPassCards();
    public event WaitPassCards OnWaitSelectType;

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
        List<Project> allProjects = GetAllProjects(type);

        foreach (var project in allProjects)
        {
            if (ProjectsCount.ContainsKey(project.projectName))
            {
                ProjectsCount[project.projectName]--;
                PlayerProjects.Add(project.Cards, project.projectName);

                ProjectScore += project.Score;
                ProjectPower += project.Power;

                if (ProjectsCount[project.projectName] == 0)
                {
                    ProjectsCount.Remove(project.projectName);
                }
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

    public override void CheckGameType()
    {
        OnWaitSelectType?.Invoke();
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
