using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TilableEditableItem))]
public class TilableEditableItemEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        TilableEditableItem item = ((TilableEditableItem)target);
        item.Init();

        EditorGUILayout.LabelField("Test");

        for (int i = 0; i < item.VarientsCount; i++)
        {
            if (GUILayout.Button("Show " + (i + 1)))
            {
                item.ChangeItem(i);
            }
        }

        if (GUILayout.Button("Show Original"))
        {
            item.ResetToOriginal();
        }

        GUILayout.Space(5);

        if (GUILayout.Button("Copy Task To Clipboard"))
        {
            item.CopyTaskToClipboard();
        }
    }
}
