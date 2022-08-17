using System.Collections;
using System.Collections.Generic;
using ArabicSupport;
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
}
