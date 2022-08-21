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
    GameObject Reward;

    [SerializeField]
    TextMeshProUGUI totalPoints;

    [SerializeField]
    TextMeshProUGUI newPoints;

    [SerializeField]
    TextMeshProUGUI coinsText;

    [SerializeField]
    TextMeshProUGUI levelText;

    [SerializeField]
    GameObject levelUp;

    [SerializeField]
    GameObject nextButton;

    int nextLevelPoints = 1;
    Action nextPressed;

    public void Open(int rank, Action next)
    {
        nextLevelPoints = GameManager.Instance.MyPlayer.LevelPoints;

        nextPressed = next;

        gameObject.SetActive(true);
        //this.nextPressed = nextPressed;
        int startPoints = GameManager.Instance.MyPlayer.Points;
        totalPoints.text = startPoints.ToString() + "/" + nextLevelPoints;

        int score = GetScore(rank);

        newPoints.text = score + "+";

        levelText.text = GameManager.Instance.MyPlayer.Level.ToString();
        float currentProgress = GameManager.Instance.MyPlayer.CurrentPogress;
        levelProgress.fillAmount = currentProgress;
        newLevelProgress.fillAmount = currentProgress;


        GameSFXManager.Instance.PlayClip("Count");

        int reward = GameManager.Instance.GetRewardAndSave(rank);

        StartCoroutine(CountNumbers(startPoints, score, reward, 1f));

        float progress = GameManager.Instance.AddPoints(score);
        float totalProgress = MathF.Min(1, currentProgress + progress);
        bool isNewLevel = currentProgress + progress >= 1;

        newLevelProgress.DOFillAmount(totalProgress, 1f).OnComplete(() =>
        {
            levelProgress.DOFillAmount(totalProgress, 0.2f);
                //celebrate
            if (isNewLevel)
            {
                StartCoroutine(ShowLevelUp());
            }
            else
            {
                nextButton.SetActive(true);
            }
        });

    }

    private IEnumerator ShowLevelUp()
    {
        levelText.text = GameManager.Instance.MyPlayer.Level.ToString();
        GameSFXManager.Instance.PlayClip("LevelUp");
        levelUp.SetActive(true);
        yield return new WaitForSeconds(1);
        nextButton.SetActive(true);
    }

    public IEnumerator CountNumbers(int startPoints, int points, int reward, float time)
    {
        float timer = 0;

        while (timer < time)
        {
            totalPoints.text = (Mathf.Round(startPoints + (points * (timer / time)))) + "/" + nextLevelPoints; ;
            newPoints.text = (Mathf.Round(points * (1 - (timer / time)))).ToString();

            coinsText.text = Mathf.Round(Mathf.Lerp(GameManager.Instance.Currency,
                GameManager.Instance.Currency + reward, time)) + " + " +
                Mathf.Round(Mathf.Lerp(reward, 0, timer));

            timer += Time.deltaTime;
            yield return null;
        }

        totalPoints.text = (startPoints + points) + "/" + nextLevelPoints;
        newPoints.text = "0";


    }

    int GetScore(int rank) => rank switch
    {
        0 => 200,
        1 => 100,
        2 => 50,
        _ => 20
    };

    public void NextPressed()
    {
        nextPressed?.Invoke();
    }
}
