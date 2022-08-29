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

        EditableItemInterface.ShowInterface(item);
    }
}
