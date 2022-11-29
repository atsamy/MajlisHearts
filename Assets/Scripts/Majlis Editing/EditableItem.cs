using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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
        if (effectType == EffectType.Glow)
        {
            ShowGlowEffect();
        }
        else
        {
            StartCoroutine(ShowSparkleEffect());
        }
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

    public void ShowGlowEffect()
    {
        effectMaterial.SetFloat("_Intensity", 0.25f);
        effectMaterial.DOFloat(1f, "_Progress", 1f).SetEase(Ease.Flash).OnComplete(() =>
        {
            effectMaterial.SetFloat("_Progress", 0f);
        });

        effectMaterial.DOFloat(0, "_Intensity", 0.5f).SetDelay(0.4f).SetEase(Ease.Flash).OnComplete(() =>
        {
            effectMaterial.SetFloat("_Intensity", 0.25f);
        });
    }

    public IEnumerator ShowSparkleEffect()
    {
        float timer = 0;
        while (timer < 1)
        {
            effectMaterial.SetFloat("_Intensity", timer / (effectType == EffectType.Glow ? 3 : 2));
            timer += Time.deltaTime * 4;
            yield return null;
        }

        yield return new WaitForSeconds(effectType == EffectType.Glow ? 0.25f : 0.3f);

        while (timer > 0)
        {
            effectMaterial.SetFloat("_Intensity", timer / (effectType == EffectType.Glow ? 4 : 2));
            timer -= Time.deltaTime * 2;
            yield return null;
        }

        effectMaterial.SetFloat("_Intensity", 0);
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
