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
    Text totalPoints;

    [SerializeField]
    Text newPoints;

    [SerializeField]
    TextMeshProUGUI levelText;

    //Action nextPressed;

    public void Open(int dealscore, Action finished)
    {
        gameObject.SetActive(true);
        //this.nextPressed = nextPressed;
        int startPoints = GameManager.Instance.MyPlayer.Points;
        totalPoints.text = startPoints.ToString();

        int score = GetScore(dealscore);

        newPoints.text = score.ToString();

        levelText.text = GameManager.Instance.MyPlayer.Level.ToString();
        float currentProgress = GameManager.Instance.MyPlayer.CurrentPogress;
        levelProgress.fillAmount = currentProgress;

        float progress;

        GameSFXManager.Instance.PlayClip("Count");
        if (GameManager.Instance.AddPoints(score, out progress))
        {
            StartCoroutine(CountNumbers(startPoints, score, 0.5f));
            levelProgress.DOFillAmount(1, 0.5f).OnComplete(() =>
            {
                //celebrate
                levelText.text = GameManager.Instance.MyPlayer.Level.ToString();
                GameSFXManager.Instance.PlayClip("LevelUp");

                StartCoroutine(Finish(1.5f, finished));
            });
        }
        else
        {
            StartCoroutine(CountNumbers(startPoints, score, 1));
            levelProgress.DOFillAmount(currentProgress + progress, 1).OnComplete(() =>
             {
                 StartCoroutine(Finish(1, finished));
                 //nextButton.SetActive(true);
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
            totalPoints.text = (Mathf.Round(startPoints + (points * (timer / time)))).ToString();
            newPoints.text = (Mathf.Round(points * (1 - (timer / time)))).ToString();

            timer += Time.deltaTime;
            yield return null;
        }

        totalPoints.text = (startPoints + points).ToString();
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
