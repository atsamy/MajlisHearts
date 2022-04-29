using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDetails : MonoBehaviour
{
    [SerializeField]
    Image avatarImage;
    [SerializeField]
    Text scoreText;
    [SerializeField]
    Text nameText;
    [SerializeField]
    Text levelText;
    [SerializeField]
    GameObject[] DoubleCards;
    public void SetPlayer(string avatar,string name,int level)
    {
        avatarImage.sprite = Resources.Load<Sprite>("Avatar/Face/" + avatar);
        nameText.text = name;
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
}
