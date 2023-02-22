using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class ProjectsPanel : MonoBehaviour
{
    [SerializeField]
    GameObject fourhunderedBtn;

    [SerializeField]
    TextMeshProUGUI[] ProjectsCount;

    public event Action<Projects, int> OnProjectAdded;

    int[] counts;
    public void Show(BalootGameType type)
    {
        gameObject.SetActive(true);
        fourhunderedBtn.SetActive(type != BalootGameType.Hokum);

        counts = new int[4];

        foreach (var item in ProjectsCount) item.text = "0";
    }

    public void AddProject(int index)
    {
        IncrementProject(index, index < 2 ? 3 : 2);
    }

    private void IncrementProject(int index, int max)
    {
        counts[index]++;
        counts[index] %= max;

        ProjectsCount[index].text = counts[index].ToString();

        OnProjectAdded?.Invoke((Projects)index, counts[index]);
    }
}
