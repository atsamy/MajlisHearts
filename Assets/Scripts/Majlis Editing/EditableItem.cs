using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditableItem : MonoBehaviour, IJsonTask
{
    public string Code;
    //int counter = 0;

    [SerializeField]
    Sprite[] varientIcons;

    public Sprite[] VarientIcons { get => varientIcons; }

    [SerializeField]
    ActionName actionName;
    [SerializeField]
    ActionType actionType;

    protected int selectedIndex = 0;

    [SerializeField]
    protected bool disableAnimation;
    //public bool Modified { set => modified = value; }
    float timer = 0;
    protected bool modified;
    bool mouseDown;

    Vector3 clickPos;

    private void Awake()
    {
        Init();
    }

    private void OnMouseDown()
    {
        if (!MenuManager.Instance.MainPanel.IsOnMain)
            return;
        mouseDown = true;

        clickPos = Input.mousePosition;
        MenuManager.Instance.timerFill.transform.position = clickPos; //Camera.main.

        StartCoroutine(resetTimer());
    }

    private void OnMouseUp()
    {
        mouseDown = false;
    }


    IEnumerator resetTimer()
    {
        timer = 1;
        while (mouseDown)
        {
            if (Vector3.Distance(Input.mousePosition, clickPos) > 5)
                mouseDown = false;

            timer -= Time.deltaTime;
            MenuManager.Instance.timerFill.fillAmount = (1 - timer) / 1;
            if (timer <= 0)
            {
                TaskData task = new TaskData()
                {
                    TargetArea = transform.parent.name,
                    TargetItem = Code,
                    ActionType = ActionType.Change
                };
                MajlisScript.Instance.ShowEditableItem(task, false, selectedIndex);

                mouseDown = false;
            }

            yield return null;
        }

        MenuManager.Instance.timerFill.fillAmount = 0;
    }

    public virtual void ResetToOriginal()
    {

    }

    public virtual void Reset()
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

    //public virtual void SetOriginal()
    //{

    //}

    public virtual void SetModified(int index)
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
