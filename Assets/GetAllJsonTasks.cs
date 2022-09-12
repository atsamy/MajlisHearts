using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetAllJsonTasks : MonoBehaviour
{
   public void GetAllTasks()
   {
        string allTasks = "";
        foreach (Transform item in transform)
        {
            IJsonTask task;
            if ((task = item.GetComponent<IJsonTask>()) != null)
            {
                allTasks += task.GetTaskJson() + ",";
            }
        }

        GUIUtility.systemCopyBuffer = allTasks;
   }
}
