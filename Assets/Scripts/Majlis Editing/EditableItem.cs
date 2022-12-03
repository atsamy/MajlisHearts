using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.Mathematics;

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

    [SerializeField]
    float effectTime = 1;
    //[SerializeField]
    //bool showEffect;
    protected Material effectMaterial;

    protected int selectedIndex = 0;

    [SerializeField]
    protected bool disableAnimation;
    [SerializeField]
    protected EffectType effectType;
    //public bool Modified { set => modified = value; }
    float timer = 0;
    protected bool modified;
    bool mouseDown;

    Vector3 clickPos;

    private void Awake()
    {
        Init();
    }

    public void SetEffectType(EffectType effectType)
    {
        this.effectType = effectType;
    }

    private void OnMouseDown()
    {
        if (!MenuManager.Instance.MainPanel.IsOnMain)
            return;

        mouseDown = true;
        clickPos = Input.mousePosition;
        MenuManager.Instance.EditTimer.SetPosition(clickPos);
        //MenuManager.Instance.timerFrame.transform.position = clickPos + Vector3.up * 150f; //Camera.main.
        //MenuManager.Instance.timerFrame.gameObject.SetActive(true);

        //MenuManager.Instance.timerFrame.color = new Color(1,1,1,0);
        //MenuManager.Instance.timerFrame.DOFade(1, 0.25f);
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
            //MenuManager.Instance.timerFill.fillAmount = (1 - timer) / 1;
            MenuManager.Instance.EditTimer.Fill((1 - timer));
            if (timer <= 0)
            {
                TaskData task = new TaskData()
                {
                    TargetArea = transform.parent.name,
                    TargetItem = Code,
                    ActionType = ActionType.Change
                };
                MenuManager.Instance.EditTimer.Hide();
                //MenuManager.Instance.timerFrame.gameObject.SetActive(false);
                MajlisScript.Instance.ShowEditableItem(task, false, selectedIndex);
                mouseDown = false;
            }

            yield return null;
        }
        MenuManager.Instance.EditTimer.Hide();
        //MenuManager.Instance.timerFill.fillAmount = 0;
        //MenuManager.Instance.timerFrame.gameObject.SetActive(false);
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

    public virtual void ChangeItem(int index,bool showEffect)
    {

    }

    public virtual void ChangeItem(int index,float time)
    {

    }

    public virtual void SetModified(int index,bool userModify)
    {
        selectedIndex = index;
        modified = true;

        if (userModify && effectMaterial != null)
        {
            ShowEffect();
        }
    }

    protected void ShowEffect()
    {
        if(effectType == EffectType.Glow)
            effectMaterial.SetInt("_Shine", 1);

        effectMaterial.SetFloat("_Intensity", 0.25f);
        effectMaterial.DOFloat(1f, "_Progress", effectTime / (int)effectType).SetEase(Ease.Flash).OnComplete(() =>
        {
            effectMaterial.SetFloat("_Progress", 0f);
        });

        effectMaterial.DOFloat(0, "_Intensity", 0.3f).SetDelay(0.4f).SetEase(Ease.Flash).OnComplete(() =>
        {
            effectMaterial.SetFloat("_Intensity", 0.25f);
        });
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

public enum EffectType
{
    None,
    Glow,
    Sparkle
}
