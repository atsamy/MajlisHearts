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

        EditableItemInterface.ShowInterface(item);
        //EditorGUILayout.LabelField("Test");

        //for (int i = 0; i < item.VarientsCount; i++)
        //{
        //    if (GUILayout.Button("Show " + (i + 1)))
        //    {
        //        item.ChangeItem(i);
        //    }
        //}

        //if (GUILayout.Button("Show Original"))
        //{
        //    item.ResetToOriginal();
        //}

        //GUILayout.Space(5);

        //if (GUILayout.Button("Copy Task To Clipboard"))
        //{
        //    item.CopyTaskToClipboard();
        //}

        //if (GUILayout.Button("Copy Language To Clipboard"))
        //{
        //    item.CopyLanguageFieldToClipBoard();
        //}

    }
}

public class EditableItemInterface
{
    public static void ShowInterface(EditableItem item)
    {
        //var newItem = (type)item;

        EditorGUILayout.LabelField("Test");

        for (int i = 0; i < item.GetVarientsCount(); i++)
        {
            if (GUILayout.Button("Show " + (i + 1)))
            {
                item.ChangeItem(i,false);
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

        if (GUILayout.Button("Copy Language To Clipboard"))
        {
            item.CopyLanguageFieldToClipBoard();
        }
    }
}
