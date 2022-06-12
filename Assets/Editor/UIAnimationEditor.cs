using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(UIAnimation))]
public class UIAnimationEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        //SideMenu mySideMenu = target as SideMenu;
        UIAnimation animation = ((UIAnimation)target);
        //MenusManager mM = FindObjectOfType<MenusManager>();

        if (GUILayout.Button("Add Move"))
        {
            animation.AddMove();
        }

        foreach (var item in animation.MoveAnimations)
        {
            foreach (var moveItem in animation.MoveAnimations)
            {
                EditorGUILayout.LabelField("Move Animation:1");

                moveItem.Target = EditorGUILayout.Vector3Field("Target", moveItem.Target);
                moveItem.Duration = EditorGUILayout.FloatField("Duration", moveItem.Duration);
                //moveItem.Duration = EditorGUILayout.FloatField("Duration", moveItem.Duration);
                //moveItem.Reverse = EditorGUILayout.fi("Duration", moveItem.Duration);
            }
        }
    }

}
