using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameInfoPanel : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI team1Score;
    [SerializeField]
    TextMeshProUGUI team2Score;
    [SerializeField]
    TextMeshProUGUI gameTypeText;
    [SerializeField]
    Image hokumShape;

    [SerializeField]
    Sprite[] shapes;

    public void ShowSuns()
    {
        Show();
        gameTypeText.text = "Suns";
        hokumShape.gameObject.SetActive(false);
    }

    public void ShowHokum(CardShape cardShape)
    {
        print(cardShape);
        Show();
        gameTypeText.text = "Hokum";
        hokumShape.sprite = shapes[(int)cardShape];
        hokumShape.gameObject.SetActive(true);
    }

    private void Show()
    {
        gameObject.SetActive(true);
        team1Score.text = "0";
        team2Score.text = "0";
    }

    public void UpdateScore(PlayerBase[] players)
    {
        team1Score.text = (players[0].Score + players[2].Score).ToString();
        team2Score.text = (players[1].Score + players[3].Score).ToString();
    }
}
