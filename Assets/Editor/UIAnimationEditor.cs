using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CanEditMultipleObjects]
[CustomEditor(typeof(UIAnimation))]
public class UIAnimationEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        UIAnimation animation = ((UIAnimation)target);

        //if (animation.MoveAnimations == null)
        //    animation.MoveAnimations = new List<MoveAnimation>();

        //if (animation.ScaleAnimation == null)
        //    animation.ScaleAnimation = new List<ScaleAnimation>();

        //if (animation.ColorAnimation == null)
        //    animation.ColorAnimation = new List<ColorAnimation>();

        EditorGUILayout.LabelField("Move");
        ShowMoveAnimation(animation.MoveAnimations);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Scale");
        ShowScaleAnimation(animation.ScaleAnimation);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Color");
        ShowColorAnimatio("Color", animation, animation.ColorAnimation);
    }

    void ShowMoveAnimation(List<MoveAnimation> moveList)
    {
        if (moveList == null)
            return;

        for (int i = 0; i < moveList.Count; i++)
        {
            EditorGUILayout.LabelField("Move Animation:" + i);

            moveList[i].Start = EditorGUILayout.Vector3Field("Start", moveList[i].Start);
            moveList[i].Duration = EditorGUILayout.FloatField("Duration", moveList[i].Duration);
            moveList[i].StartAfter = EditorGUILayout.FloatField("Start After", moveList[i].StartAfter);
            moveList[i].Reverse = EditorGUILayout.Toggle("Reverse", moveList[i].Reverse);
        }

        if (GUILayout.Button("Add Move"))
        {
            moveList.Add(new MoveAnimation());
        }
        if (GUILayout.Button("Remove Move"))
        {
            moveList.RemoveAt(moveList.Count - 1);
        }
    }

    void ShowScaleAnimation(List<ScaleAnimation> scaleList)
    {
        if (scaleList == null)
            return;

        for (int i = 0; i < scaleList.Count; i++)
        {
            EditorGUILayout.LabelField("Scale Animation:" + i);

            scaleList[i].Start = EditorGUILayout.Vector3Field("Start", scaleList[i].Start);
            scaleList[i].Duration = EditorGUILayout.FloatField("Duration", scaleList[i].Duration);
            scaleList[i].StartAfter = EditorGUILayout.FloatField("Start After", scaleList[i].StartAfter);
            scaleList[i].Reverse = EditorGUILayout.Toggle("Reverse", scaleList[i].Reverse);
        }

        if (GUILayout.Button("Add Scale"))
        {
            scaleList.Add(new ScaleAnimation());
        }
        if (GUILayout.Button("Remove Scale"))
        {
            scaleList.RemoveAt(scaleList.Count - 1);
        }
    }

    void AddScaleButtons(UIAnimation animation)
    {
        if (GUILayout.Button("Add Scale"))
        {
            animation.ScaleAnimation.Add(new ScaleAnimation());
        }
        if (GUILayout.Button("Remove Scale"))
        {
            animation.ScaleAnimation.RemoveAt(animation.ScaleAnimation.Count - 1);
        }
    }

    void ShowColorAnimatio(string animationName, UIAnimation animation, List<ColorAnimation> colorList)
    {
        if (colorList == null)
            colorList = new List<ColorAnimation>();

        for (int i = 0; i < colorList.Count; i++)
        {
            EditorGUILayout.LabelField(animationName + " Animation:" + i);

            colorList[i].Start = EditorGUILayout.ColorField("Start", colorList[i].Start);
            colorList[i].Duration = EditorGUILayout.FloatField("Duration", colorList[i].Duration);
            colorList[i].StartAfter = EditorGUILayout.FloatField("Start After", colorList[i].StartAfter);
            colorList[i].Reverse = EditorGUILayout.Toggle("Reverse", colorList[i].Reverse);
        }

        if (GUILayout.Button("Add " + animationName))
        {
            animation.ColorAnimation.Add(new ColorAnimation());
        }
        if (GUILayout.Button("Remove " + animationName))
        {
            animation.ColorAnimation.RemoveAt(animation.ColorAnimation.Count - 1);
        }
    }
}
