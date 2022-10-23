using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System;
using ArabicSupport;

public class LevelPanel : MonoBehaviour
{
    [SerializeField]
    Image avatarImage;

    [SerializeField]
    Image levelProgress;

    [SerializeField]
    Image newLevelProgress;

    [SerializeField]
    TextMeshProUGUI playerName;

    [SerializeField]
    TextMeshProUGUI rankText;

    [SerializeField]
    TextMeshProUGUI gemsText;

    //[SerializeField]
    //TextMeshProUGUI newGemsText;

    [SerializeField]
    TextMeshProUGUI scoreText;

    //[SerializeField]
    //TextMeshProUGUI newPoints;

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

    public void Open(int rank,int totalScore, Action next)
    {
        playerName.text = ArabicFixer.Fix(GameManager.Instance.MyPlayer.Name);
        nextLevelPoints = GameManager.Instance.MyPlayer.LevelPoints;
        nextPressed = next;
        scoreText.text = totalScore.ToString();
        gameObject.SetActive(true);

        avatarImage.sprite = AvatarManager.Instance.playerAvatar;

        switch (rank)
        {
            case 0:
                rankText.text = LanguageManager.Instance.GetString("1st");
                break;
            case 1:
                rankText.text = LanguageManager.Instance.GetString("2nd");
                break;
            case 2:
                rankText.text = LanguageManager.Instance.GetString("3rd");
                break;
            case 3:
                rankText.text = LanguageManager.Instance.GetString("4th");
                break;
        }

        //this.nextPressed = nextPressed;
        int startPoints = GameManager.Instance.MyPlayer.Points;
        //totalPoints.text = startPoints.ToString() + "/" + nextLevelPoints;

        int score = GetScore(rank);

        //newPoints.text = score + "+";

        levelText.text = GameManager.Instance.MyPlayer.Level.ToString();
        float currentProgress = GameManager.Instance.MyPlayer.CurrentPogress;
        levelProgress.fillAmount = currentProgress;
        newLevelProgress.fillAmount = currentProgress;

        if (rank == 0)
        {
            GameSFXManager.Instance.PlayClip("Win");
        }
        else
        {
            GameSFXManager.Instance.PlayClip("Lose");
        }

        int reward = GameManager.Instance.GetRewardAndSave(rank);
        int gems = GameManager.Instance.GetGemsAndSave(rank);

        coinsText.text = reward.ToString();
        gemsText.text = gems.ToString();
        //StartCoroutine(CountNumbers(gems, reward, 1f));

        float progress = GameManager.Instance.AddPoints(score);
        float totalProgress = MathF.Min(1, currentProgress + progress);
        bool isNewLevel = currentProgress + progress >= 1;

        newLevelProgress.DOFillAmount(totalProgress, 1f).OnComplete(() =>
        {
            levelProgress.DOFillAmount(totalProgress, 0.2f);
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

    public IEnumerator CountNumbers(int gems, int reward, float time)
    {
        float timer = 0;
        yield return new WaitForSeconds(2);
        GameSFXManager.Instance.PlayClip("Count");

        while (timer < time)
        {
            coinsText.text = Mathf.Round(Mathf.Lerp(0,reward, Mathf.Min(1f, time * 1.5f))).ToString();
            //newCoinsText.text = "+ " + Mathf.Round(Mathf.Lerp(reward,0, time)).ToString();

            gemsText.text = Mathf.Round(Mathf.Lerp(0,gems, Mathf.Max(0, time * 1.5f - 0.5f))).ToString();
            //newGemsText.text = "+ " + Mathf.Round(Mathf.Lerp(gems, 0, time)).ToString();

            timer += Time.deltaTime;
            yield return null;
        }

        coinsText.text = reward.ToString();
        gemsText.text = gems.ToString();

        //newCoinsText.text = "+ 0";
        //newGemsText.text = "+ 0";


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
