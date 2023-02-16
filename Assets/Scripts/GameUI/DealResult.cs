using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using TMPro;

public class RoundResult : MonoBehaviour
{
    [SerializeField]
    protected PlayerDealResult[] singlePlayers;
    [SerializeField]
    protected TextMeshProUGUI[] ranks;
    [SerializeField]
    protected GameObject Footer;
    //[SerializeField]
    //protected TextMeshProUGUI buttonText;
    protected Action<int> PanelClosed;
    [SerializeField]
    protected GameObject timerButton;
    [SerializeField]
    protected Image timerFill;
    [SerializeField]
    protected TextMeshProUGUI timerText;

    protected int rank = 0;
    public virtual void ShowRound(GameScriptBase game, bool inGame, bool gameOver, Action<int> OnPanelClosed)
    {
        gameObject.SetActive(true);

        if (gameOver)
        {
            Footer.SetActive(true);
            //buttonText.text = LanguageManager.Instance.GetString("next");
            timerButton.SetActive(false);
        }
        else if (inGame)
        {
            //Footer.SetActive(true);
            //buttonText.text = LanguageManager.Instance.GetString("close");
            timerButton.SetActive(false);
        }
        else
        {
            //Footer.SetActive(false);
            //buttonText.text = LanguageManager.Instance.GetString("nextround");
            timerButton.SetActive(true);
            StartCoroutine(CountNextRound(5));
        }
        this.PanelClosed = OnPanelClosed;
    }

    protected IEnumerator CountNextRound(int v)
    {
        float timer = v;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            int time = Mathf.CeilToInt(timer);
            timerText.text = time.ToString();

            if (time % 2 == 0)
            {
                timerFill.fillClockwise = false;
                timerFill.fillAmount = (1 - (time - timer));
            }
            else
            {
                timerFill.fillClockwise = true;
                timerFill.fillAmount = time - timer;
            }
            yield return null;
        }

        if (PanelClosed != null)
        {
            gameObject.SetActive(false);
            PanelClosed?.Invoke(rank);
        }
    }

    public void ShowInGame(GameScriptBase game)
    {
        ShowRound(game, true, false, (rank) =>
          {
              gameObject.SetActive(false);
          });
    }

    public void Pressed()
    {
        PanelClosed?.Invoke(rank);
        gameObject.SetActive(false);
        GameSFXManager.Instance.PlayClip("Click");
    }
}
