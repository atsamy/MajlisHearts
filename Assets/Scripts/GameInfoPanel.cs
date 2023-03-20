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
    TextMeshProUGUI doubleText;
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

    public void ShowHokum(CardShape cardShape,int doubler)
    {
        //print(doubler);
        Show();
        gameTypeText.text = "Hokum";
        hokumShape.sprite = shapes[(int)cardShape];
        hokumShape.gameObject.SetActive(true);

        switch (doubler) 
        {
            case 0:
                doubleText.text = "";
                break;
                case 2:
                doubleText.text = "x2";
                break;
                case 3:
                doubleText.text = "x3";
                break;
                case 4:
                doubleText.text = "x4";
                break;
                case 5:
                doubleText.text = "Qahwa";
                break;
        }
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
