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
    SpinWheel spinnerWheel;
    [SerializeField]
    GameObject mainPanel;
    [SerializeField]
    TextMeshProUGUI playerName;
    [SerializeField]
    TextMeshProUGUI rankText;
    [SerializeField]
    TextMeshProUGUI gemsText;
    [SerializeField]
    TextMeshProUGUI scoreText;
    [SerializeField]
    TextMeshProUGUI coinsText;
    [SerializeField]
    TextMeshProUGUI levelText;
    [SerializeField]
    TextMeshProUGUI totalCoinsText;
    [SerializeField]
    TextMeshProUGUI totalGemsText;
    [SerializeField]
    TextMeshProUGUI addCoinsText;
    [SerializeField]
    TextMeshProUGUI addGemsText;

    [SerializeField]
    GameObject levelUp;
    [SerializeField]
    GameObject nextButton;

    Action nextPressed;

    public void Open(int rank,int totalScore, Action next)
    {
        playerName.text = ArabicFixer.Fix(GameManager.Instance.MyPlayer.Name);
        //nextLevelPoints = GameManager.Instance.MyPlayer.LevelPoints;
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
        int score = GetScore(rank);

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

        int startCoins = GameManager.Instance.Coins;
        int startGems = GameManager.Instance.Gems;

        totalCoinsText.text = startCoins.ToString();
        totalGemsText.text = startGems.ToString();

        int reward = GameManager.Instance.GetRewardAndSave(rank);
        int gems = GameManager.Instance.GetGemsAndSave(rank);

        coinsText.text = reward.ToString();
        gemsText.text = gems.ToString();

        StartCoroutine(CountNumbers(startGems,startCoins,gems, reward, 1f));

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

        spinnerWheel.onClaimReawrd += (currency, amount) =>
        {
            if (currency == "Coins")
            {
                GameManager.Instance.AddCoins(amount);
                totalCoinsText.GetComponent<ChangeNumber>().Change(GameManager.Instance.Coins);
                ShowAddedAmount(addCoinsText, amount);
            }
            else
            {
                GameManager.Instance.AddGems(amount);
                totalGemsText.GetComponent<ChangeNumber>().Change(GameManager.Instance.Gems);
                ShowAddedAmount(addGemsText, amount);
            }
        };
    }

    private IEnumerator ShowLevelUp()
    {
        levelText.text = GameManager.Instance.MyPlayer.Level.ToString();
        GameSFXManager.Instance.PlayClip("LevelUp");
        levelUp.SetActive(true);
        yield return new WaitForSeconds(1);
        nextButton.SetActive(true);
    }

    public IEnumerator CountNumbers(int startgems,int startcoins, int gems, int reward, float time)
    {
        float timer = 0;
        yield return new WaitForSeconds(2);
        GameSFXManager.Instance.PlayClip("Count");

        while (timer < time)
        {
            coinsText.text = Mathf.Round(Mathf.Lerp(reward, 0, timer)).ToString();
            totalCoinsText.text = Mathf.Round(Mathf.Lerp(startcoins, startcoins + reward, timer)).ToString();

            timer += Time.deltaTime;
            yield return null;
        }

        totalCoinsText.GetComponent<ChangeNumber>().setNumber(GameManager.Instance.Coins);
        coinsText.text = "0";
        ShowAddedAmount(addCoinsText, reward);

        timer = 0;
        GameSFXManager.Instance.PlayClip("Count");
        while (timer < time)
        {
            gemsText.text = Mathf.Round(Mathf.Lerp(gems, 0, timer)).ToString();
            totalGemsText.text = Mathf.Round(Mathf.Lerp(startgems, startgems + gems, timer)).ToString();

            timer += Time.deltaTime;
            yield return null;
        }

        totalGemsText.GetComponent<ChangeNumber>().setNumber(GameManager.Instance.Gems);
        gemsText.text = "0";
        ShowAddedAmount(addGemsText,gems);
    }

    private void ShowAddedAmount(TextMeshProUGUI currencyText,int gems)
    {
        Vector3 originalPosition = currencyText.transform.position;
        currencyText.text = "+" + gems;
        currencyText.DOFade(0, 2f);
        currencyText.transform.DOMoveY(originalPosition.y + 10, 2.1f).OnComplete(()=>
        {
            currencyText.text = "";
            currencyText.color = Color.white;
            currencyText.transform.position = originalPosition;
        });
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
        mainPanel.SetActive(false);
        if (AdsManager.Instance.IsVideoReady)
        {
            spinnerWheel.Open(() =>
            {
                nextPressed?.Invoke();
            });
        }
        else
        {
            nextPressed?.Invoke();
        }
    }
}
