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

        EditableItemInterface.ShowInterface(item);

    }
}
