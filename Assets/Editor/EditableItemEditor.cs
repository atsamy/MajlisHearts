using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SingleEditableItem))]
public class EditableItemEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SingleEditableItem item = ((SingleEditableItem)target);
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
