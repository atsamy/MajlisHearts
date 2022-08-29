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

        EditableItemInterface.ShowInterface(item);
    }
}
