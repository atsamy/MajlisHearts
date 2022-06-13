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

        UIAnimation animation = ((UIAnimation)target);

        EditorGUILayout.LabelField("Move");
        ShowTransformAnimation("Move", animation.MoveAnimations);
        AddMoveButtons(animation.MoveAnimations);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Scale");
        ShowTransformAnimation("Scale", animation.ScaleAnimation);
        AddScaleButtons(animation.ScaleAnimation);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Color");
        ShowColorAnimatio("Color", animation, animation.ColorAnimation);
    }

    void ShowTransformAnimation(string animationName, List<TransformAnimationBase> moveList)
    {
        EditorGUILayout.BeginVertical();

        if (moveList == null)
            moveList = new List<TransformAnimationBase>();

        for (int i = 0; i < moveList.Count; i++)
        {
            EditorGUILayout.LabelField(animationName + " Animation:" + i);

            moveList[i].Start = EditorGUILayout.Vector3Field("Start", moveList[i].Start);
            moveList[i].Duration = EditorGUILayout.FloatField("Duration", moveList[i].Duration);
            moveList[i].StartAfter = EditorGUILayout.FloatField("Start After", moveList[i].StartAfter);
            moveList[i].Reverse = EditorGUILayout.Toggle("Reverse", moveList[i].Reverse);
        }
    }

    void AddMoveButtons(List<TransformAnimationBase> moveList)
    {
        if (GUILayout.Button("Add Move"))
        {
            moveList.Add(new MoveAnimation());
        }
        if (GUILayout.Button("Remove Move"))
        {
            moveList.RemoveAt(moveList.Count - 1);
        }
    }

    void AddScaleButtons(List<TransformAnimationBase> moveList)
    {
        if (GUILayout.Button("Add Scale"))
        {
            moveList.Add(new ScaleAnimation());
        }
        if (GUILayout.Button("Remove Scale"))
        {
            moveList.RemoveAt(moveList.Count - 1);
        }
    }

    void ShowColorAnimatio(string animationName, UIAnimation animation, List<ColorAnimation> colorList)
    {
        //EditorGUILayout.BeginVertical();

        if (colorList == null)
            colorList = new List<ColorAnimation>();

        for (int i = 0; i < colorList.Count; i++)
        {
            EditorGUILayout.LabelField(animationName + " Animation:" + i);

            colorList[i].Target = EditorGUILayout.ColorField("Target", colorList[i].Target);
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

        //EditorGUILayout.EndVertical();
    }
}
