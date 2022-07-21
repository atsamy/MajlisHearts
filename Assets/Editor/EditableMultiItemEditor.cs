using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MultiEditableItems))]
public class EditableMultiItemEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MultiEditableItems item = ((MultiEditableItems)target);
        item.Init();

        EditorGUILayout.LabelField("Test");
        
        if (GUILayout.Button("Show 1"))
        {
            item.ChangeItem(0);
        }

        if (GUILayout.Button("Show 2"))
        {
            item.ChangeItem(1);
        }

        if (GUILayout.Button("Show 3"))
        {
            item.ChangeItem(2);
        }

        if (GUILayout.Button("Show Original"))
        {
            item.ResetToOriginal();
        }
    }
}
