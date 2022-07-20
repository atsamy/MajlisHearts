using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TaskItemScript : MonoBehaviour
{
    [SerializeField]
    Text taskName;
    [SerializeField]
    TextMeshProUGUI costText;

    Action onPressedAction;

    public void Set(string taskName,int cost,Action action)
    {
        this.taskName.text = taskName;
        this.costText.text = cost.ToString();

        onPressedAction = action;
    }

    public void OnPressed()
    {
        onPressedAction?.Invoke();
    }
}
