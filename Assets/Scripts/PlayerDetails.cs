using System.Collections;
using System.Collections.Generic;
using ArabicSupport;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class PlayerDetails : MonoBehaviour
{
    [SerializeField]
    Image avatarImage;
    [SerializeField]
    TextMeshProUGUI scoreText;
    [SerializeField]
    TextMeshProUGUI nameText;
    [SerializeField]
    TextMeshProUGUI levelText;
    [SerializeField]
    GameObject[] DoubleCards;
    [SerializeField]
    Image timerFill;
    [SerializeField]
    TextMeshProUGUI timerText;

    Coroutine timerRoutine;

    public void SetPlayer(Sprite avatar, string name, int level)
    {
        if (avatar != null)
            avatarImage.sprite = avatar;

        nameText.text = ArabicFixer.Fix(name, false, false);
        levelText.text = level.ToString();
        scoreText.text = "0";
    }

    public void SetScore(int score)
    {
        scoreText.text = score.ToString();
    }

    public void ShowDouble(int index)
    {
        DoubleCards[index].SetActive(true);
    }

    public void HideDouble(int index)
    {
        DoubleCards[index].SetActive(false);
    }

    public void StartTimer(int timer)
    {
        timerRoutine = StartCoroutine(TimerRoutine(timer));
    }

    public void StopTimer()
    {
        if(timerRoutine != null)
            StopCoroutine(timerRoutine);

        timerFill.DOFillAmount(0, 0.15f);
        timerText.text = "";
    }

    IEnumerator TimerRoutine(float time)
    {
        float timer = time;


        while (timer > 0)
        {
            timer -= Time.deltaTime;
            timerFill.fillAmount = timer / time;
            timerText.text = Mathf.Round(timer).ToString();
            yield return null;
        }

        timerText.text = "";
    }
}
