using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class CleanAnimation : MonoBehaviour,IJsonTask
{
    public string Code;
    public ActionName ActionName;

    [SerializeField]
    Sprite taskIcon;

    public void Clean(Action taskFinished)
    {
        

        if (transform.childCount > 0)
        {
            StartCoroutine(CleanRoutine(taskFinished));
        }
        else
        {
            MajlisScript.Instance.DustParticles.transform.position = transform.position + new Vector3(0,0,-3);
            MajlisScript.Instance.DustParticles.Emit(10);
            SFXManager.Instance.PlayClip("Woosh");
            transform.DOScale(0, 0.5f).SetEase(Ease.InOutCubic).OnComplete(()=>
            {
                taskFinished?.Invoke();
            });
        }
    }

    IEnumerator CleanRoutine(Action taskFinished)
    {
        foreach (Transform item in transform)
        {
            MajlisScript.Instance.DustParticles.transform.position = item.position;
            MajlisScript.Instance.DustParticles.Emit(UnityEngine.Random.Range(6,10));

            SFXManager.Instance.PlayClip("Woosh");
            item.DOScale(0, 0.5f).SetEase(Ease.InOutCubic);
            yield return new WaitForSeconds(0.2f);
        }

        yield return new WaitForSeconds(0.3f);
        taskFinished?.Invoke();
        gameObject.SetActive(false);
    }

    internal void Reset()
    {
        foreach (Transform item in transform)
        {
            item.localScale = Vector3.one;
        }

        gameObject.SetActive(true);
    }

    public void CopyTaskToClipboard()
    {

        GUIUtility.systemCopyBuffer = GetTaskJson();
    }

    public void CopyLanguageFieldToClipBoard()
    {
        GUIUtility.systemCopyBuffer = "<string name =\"" + Code + "\">" + Code + "</string>";
    }

    public string GetTaskJson()
    {
        TaskData data = new TaskData()
        {
            ActionType = ActionType.Clean,
            ActionName = ActionName.ToString(),
            Cost = 40,
            TargetItem = Code,
            TargetArea = transform.parent.name
        };

        return JsonUtility.ToJson(data);
    }

    public Sprite GetTaskIcon()
    {
        return taskIcon;
    }
}
