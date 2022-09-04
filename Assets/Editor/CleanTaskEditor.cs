using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CleanAnimation))]
public class CleanTaskEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        CleanAnimation item = ((CleanAnimation)target);


        GUILayout.Space(5);

        if (GUILayout.Button("Copy Task To Clipboard"))
        {
            item.CopyTaskToClipboard();
        }

        if (GUILayout.Button("Copy Language To Clipboard"))
        {
            item.CopyLanguageFieldToClipBoard();
        }
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