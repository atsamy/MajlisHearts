using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MajlisScript))]
public class MajlisScriptEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MajlisScript item = ((MajlisScript)target);
        //item.Init();

        //EditorGUILayout.LabelField("Test");
        
        if (GUILayout.Button("retrieve Items"))
        {
            item.RetrieveItems();
        }
    }
}
