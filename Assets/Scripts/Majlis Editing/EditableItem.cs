using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditableItem : MonoBehaviour, IJsonTask
{
    public string Code;
    int counter = 0;

    [SerializeField]
    Sprite[] varientIcons;

    public Sprite[] VarientIcons { get => varientIcons; }

    [SerializeField]
    ActionName actionName;
    [SerializeField]
    ActionType actionType;

    protected int selectedIndex = 0;
    //public bool Modified { set => modified = value; }

    bool modified;

    private void Awake()
    {
        Init();
    }

    private void OnMouseDown()
    {
        if (counter == 1)
        {
            if (modified)
            {
                TaskData task = new TaskData()
                {
                    TargetArea = transform.parent.name,
                    TargetItem = Code,
                    ActionType = ActionType.Change
                };
                MajlisScript.Instance.ShowEditableItem(task,false, selectedIndex);
            }
            //float itemPos = Camera.main.WorldToScreenPoint(transform.position).x / Screen.width;
            //print(itemPos);
            //EditorUI.Instance.ShowItems(Code, itemPos);
        }
        else
            counter = 1;

        StartCoroutine(resetTimer());
    }

    IEnumerator resetTimer()
    {
        yield return new WaitForSeconds(0.5f);
        counter = 0;
    }

    public virtual void ResetToOriginal()
    {

    }

    public virtual int GetVarientsCount()
    {
        return 0;
    }

    public virtual void ChangeItem(int index)
    {

    }

    public virtual void ChangeItem(int index,float time)
    {

    }

    public virtual void SetOriginal()
    {

    }

    protected void SetModified(int index)
    {
        selectedIndex = index;
        modified = true;
    }

    public virtual void Init()
    {

    }

    public void CopyTaskToClipboard()
    {

        GUIUtility.systemCopyBuffer = GetTaskJson();
    }

    public void CopyLanguageFieldToClipBoard()
    {
        GUIUtility.systemCopyBuffer = "<string name =\"" + Code + "\">"+ Code + "</string>";
    }

    public string GetTaskJson()
    {
        TaskData data = new TaskData()
        {
            ActionType = actionType,
            ActionName = actionName.ToString(),
            Cost = 40,
            TargetArea = transform.parent.name,
            TargetItem = Code
        };

        return JsonUtility.ToJson(data);
    }

    public Sprite GetTaskIcon()
    {
        return varientIcons[0];
    }
}

public interface IJsonTask
{
    public string GetTaskJson();
    public Sprite GetTaskIcon();
}
