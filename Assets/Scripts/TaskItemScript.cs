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
    [SerializeField]
    Image taskImage;
    Action onPressedAction;

    public void Set(string taskName,int cost,Sprite taskImage,Action action)
    {
        gameObject.SetActive(true);
        this.taskName.text = taskName;
        this.costText.text = cost.ToString();
        this.taskImage.sprite = taskImage;

        onPressedAction = action;
    }

    public void OnPressed()
    {
        onPressedAction?.Invoke();
    }
}
