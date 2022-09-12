using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GetAllJsonTasks))]
public class GetAllJsonTasksEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GetAllJsonTasks item = ((GetAllJsonTasks)target);

        if (GUILayout.Button("Get Tasks Json"))
        {
            item.GetAllTasks();
        }

    }
}
