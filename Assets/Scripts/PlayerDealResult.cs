using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ArabicSupport;
public class PlayerDealResult : MonoBehaviour
{
    [SerializeField]
    Image playerAvatar;
    [SerializeField]
    TextMeshProUGUI playerName;
    [SerializeField]
    TextMeshProUGUI score;
    [SerializeField]
    TextMeshProUGUI addedScore;
    //[SerializeField]
    //Image scoreFrame;
    //[SerializeField]
    //Image playerFrame;
    //[SerializeField]
    //GameObject winFrame;
    [SerializeField]
    GameObject[] teamBadge;
    //[SerializeField]
    //Sprite[] scoreFrames;
    //[SerializeField]
    //Sprite[] avatarFrames;

    public void Set(Player player, bool inGame)
    {
        playerName.text = ArabicFixer.Fix(player.Name, false, false);
        playerAvatar.sprite = player.Avatar;

        //if (inGame)
        //{
        this.score.text = player.TotalScore.ToString();
        //}
        //else
        //{
        //    score.text = endScore.ToString();
        //    //StartCoroutine(countScore(player.TotalScore - player.Score, player.TotalScore));
        //}

        SetPlayer(player.Name == GameManager.Instance.MyPlayer.Name);
    }

    //IEnumerator countScore(float startScore, float endScore)
    //{
    //    float timer = 1;

    //    score.text = startScore.ToString();
    //    addedScore.text = (endScore - startScore).ToString();
    //    yield return new WaitForSeconds(0.5f);

    //    while (timer > 0)
    //    {
    //        timer -= Time.deltaTime;

    //        score.text = Mathf.Round(Mathf.Lerp(startScore, endScore, timer)).ToString();
    //        addedScore.text = Mathf.Round(Mathf.Lerp(endScore - startScore, 0, timer)).ToString();

    //        yield return null;
    //    }

    //    //yield return new WaitForSeconds(1);

    //    score.text = endScore.ToString();
    //    addedScore.text = "";
    //}

    public void ShowTeamBadge(int teamIndex)
    {
        teamBadge[teamIndex].SetActive(true);
    }

    public void SetPlayer(bool value)
    {
        //scoreFrame.sprite = scoreFrames[value ? 1 : 0];
        //playerFrame.sprite = avatarFrames[value ? 1 : 0];
        //winFrame.SetActive(value);
    }
}
