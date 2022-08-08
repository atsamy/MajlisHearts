using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System;

public class LevelPanel : MonoBehaviour
{
    [SerializeField]
    Image levelProgress;

    [SerializeField]
    Image newLevelProgress;

    [SerializeField]
    TextMeshProUGUI totalPoints;

    [SerializeField]
    TextMeshProUGUI newPoints;

    [SerializeField]
    TextMeshProUGUI levelText;

    [SerializeField]
    GameObject levelUp;

    int nextLevelPoints = 1;
    //Action nextPressed;

    public void Open(int dealscore, Action finished)
    {
        nextLevelPoints = GameManager.Instance.MyPlayer.LevelPoints;

        gameObject.SetActive(true);
        //this.nextPressed = nextPressed;
        int startPoints = GameManager.Instance.MyPlayer.Points;
        totalPoints.text = startPoints.ToString() + "/" + nextLevelPoints;

        int score = GetScore(dealscore);

        newPoints.text = score + "+";

        levelText.text = GameManager.Instance.MyPlayer.Level.ToString();
        float currentProgress = GameManager.Instance.MyPlayer.CurrentPogress;
        levelProgress.fillAmount = currentProgress;
        newLevelProgress.fillAmount = currentProgress;

        float progress;

        GameSFXManager.Instance.PlayClip("Count");
        if (GameManager.Instance.AddPoints(score, out progress))
        {
            StartCoroutine(CountNumbers(startPoints, score, 1f));
            newLevelProgress.DOFillAmount(1, 1f).OnComplete(() =>
            {
                levelProgress.DOFillAmount(1, 0.2f);
                //celebrate
                levelText.text = GameManager.Instance.MyPlayer.Level.ToString();
                GameSFXManager.Instance.PlayClip("LevelUp");
                levelUp.SetActive(true);
                StartCoroutine(Finish(3f, finished));
            });
        }
        else
        {
            StartCoroutine(CountNumbers(startPoints, score, 1));
            newLevelProgress.DOFillAmount(currentProgress + progress, 1).OnComplete(() =>
             {
                 levelProgress.DOFillAmount(currentProgress + progress, 0.2f);
                 StartCoroutine(Finish(2, finished));
             });
        }
    }

    public IEnumerator Finish(float time, Action finished)
    {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
        finished?.Invoke();
    }

    public IEnumerator CountNumbers(int startPoints, int points, float time)
    {
        float timer = 0;

        while (timer < time)
        {
            totalPoints.text = (Mathf.Round(startPoints + (points * (timer / time)))) + "/" + nextLevelPoints; ;
            newPoints.text = (Mathf.Round(points * (1 - (timer / time)))).ToString();

            timer += Time.deltaTime;
            yield return null;
        }

        totalPoints.text = (startPoints + points) + "/" + nextLevelPoints;
        newPoints.text = "0";
    }

    int GetScore(int dealScore) => dealScore switch
    {
        0 => 40,
        < 6 => 30,
        < 11 => 20,
        _ => 10
    };
}
