using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameInfoPanel : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI team1TotalScore;
    [SerializeField]
    TextMeshProUGUI team2TotalScore;
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

    public void ShowSuns(GameScriptBaloot gameScriptBaloot)
    {
        Show(gameScriptBaloot);
        gameTypeText.text = LanguageManager.Instance.GetString("Sun");
        hokumShape.gameObject.SetActive(false);
    }

    public void ShowHokum(CardShape cardShape,int doubler, GameScriptBaloot gameScriptBaloot)
    {
        //print(doubler);
        Show(gameScriptBaloot);
        gameTypeText.text = LanguageManager.Instance.GetString("Hokum");
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
                doubleText.text = LanguageManager.Instance.GetString("Qahwa");
                break;
        }
    }

    private void Show(GameScriptBaloot gameScriptBaloot)
    {
        gameObject.SetActive(true);
        team1Score.text = "0";
        team2Score.text = "0";

        team1TotalScore.text = gameScriptBaloot.TeamsTotalScore[0].ToString();
        team2TotalScore.text = gameScriptBaloot.TeamsTotalScore[1].ToString();
    }

    public void UpdateScore(PlayerBase[] players)
    {
        team1Score.text = (players[0].Score + players[2].Score).ToString();
        team2Score.text = (players[1].Score + players[3].Score).ToString();
    }
}
