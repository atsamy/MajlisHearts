using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDealResult : MonoBehaviour
{
    [SerializeField]
    Image playerAvatar;
    [SerializeField]
    Text playerName;
    [SerializeField]
    Text score;
    [SerializeField]
    Image scoreFrame;
    [SerializeField]
    Image playerFrame;
    [SerializeField]
    GameObject winFrame;
    [SerializeField]
    GameObject[] teamBadge;
    [SerializeField]
    Sprite[] scoreFrames;
    [SerializeField]
    Sprite[] avatarFrames;

    public void Set(string name,string avatar)
    {
        print("avatar: " + avatar);

        playerName.text = name;
        playerAvatar.sprite = Resources.Load<Sprite>("Avatar/Face/" + avatar);
    }

    public void SetScore(int score)
    {
        this.score.text = score.ToString();
    }

    public void ShowTeamBadge(int teamIndex)
    {
        teamBadge[teamIndex].SetActive(true);
    }

    public void SetWinner(bool value)
    {
        scoreFrame.sprite = scoreFrames[value ? 1 : 0];
        playerFrame.sprite = avatarFrames[value ? 1 : 0];
        winFrame.SetActive(value);
    }
}
