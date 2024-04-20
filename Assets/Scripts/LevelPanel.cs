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
    ChangeNumber totalCoinsText;
    [SerializeField]
    ChangeNumber totalGemsText;
    [SerializeField]
    TextMeshProUGUI addCoinsText;
    [SerializeField]
    TextMeshProUGUI addGemsText;

    [SerializeField]
    GameObject levelUp;
    [SerializeField]
    GameObject nextButton;

    Action nextPressed;

    int reward;
    int gems;

    public void Open(int rank, int totalScore, Action next)
    {
        playerName.text = ArabicFixer.Fix(GameManager.Instance.MyPlayer.Name, false, false);

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

        totalCoinsText.setNumber(startCoins);
        totalGemsText.setNumber(startGems);

        reward = GameManager.Instance.GetRewardAndSave(rank);
        gems = GameManager.Instance.GetGemsAndSave(rank);

        coinsText.text = reward.ToString();
        gemsText.text = gems.ToString();

        float progress = GameManager.Instance.AddPoints(score, GetPoints(rank),
            GameManager.Instance.Game == Game.Hearts ? "HeartsPoints" : "BalootPoints");
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
            if (currency == "coins")
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

    private void ShowAddedAmount(TextMeshProUGUI currencyText, int gems)
    {
        Vector3 originalPosition = currencyText.transform.position;
        currencyText.text = "+" + gems;
        currencyText.DOFade(0, 2f);
        currencyText.transform.DOMoveY(originalPosition.y + 10, 2.1f).OnComplete(() =>
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

    int GetPoints(int rank) => rank switch
    {
        0 => 3,
        1 => 2,
        2 => 1,
        _ => 0
    };

    public void NextPressed()
    {
        StartCoroutine(NextSeq());
    }

    IEnumerator NextSeq()
    {
        totalCoinsText.Change(GameManager.Instance.Coins);
        totalGemsText.Change(GameManager.Instance.Gems);

        ShowAddedAmount(addCoinsText, reward);
        ShowAddedAmount(addGemsText, gems);

        GameSFXManager.Instance.PlayClip("Coins");

        mainPanel.SetActive(false);

        yield return new WaitForSeconds(0.5f);

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
