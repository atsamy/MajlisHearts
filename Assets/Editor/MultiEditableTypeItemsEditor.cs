using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MultiEditableTypeItems))]
public class MultiEditableTypeItemsEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MultiEditableTypeItems item = ((MultiEditableTypeItems)target);
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
