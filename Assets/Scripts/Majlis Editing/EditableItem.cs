using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditableItem : MonoBehaviour
{
    public string Code;
    int counter = 0;

    [SerializeField]
    Sprite[] varientIcons;

    public Sprite[] VarientIcons { get => varientIcons; }

    [SerializeField]
    TaskAction taskName;
    [SerializeField]
    TaskAction taskAction;
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
                MajlisScript.Instance.ShowEditableItem(Code);
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

    protected void SetModified()
    {
        modified = true;
    }

    public virtual void Init()
    {

    }

    public void CopyTaskToClipboard()
    {
        TaskData data = new TaskData()
        {
            ActionType = taskAction,
            Cost = 40,
            ID = taskName + "_" + Code,
            Target = Code
        };

        JsonUtility.ToJson(data);
        GUIUtility.systemCopyBuffer = JsonUtility.ToJson(data);
    }

    public void CopyLanguageFieldToClipBoard()
    {
        string objectName = Code.Split("_")[1];
        GUIUtility.systemCopyBuffer = "<string name =\"" + objectName + "\">"+objectName+"</string>";
    }
}
