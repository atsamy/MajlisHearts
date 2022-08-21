using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ArabicSupport;
public class PlayerDealResult : MonoBehaviour
{
    [SerializeField]
    protected Image playerAvatar;
    [SerializeField]
    protected Text playerName;
    [SerializeField]
    protected TextMeshProUGUI score;
    [SerializeField]
    protected Image scoreFrame;
    [SerializeField]
    protected Image playerFrame;
    [SerializeField]
    GameObject winFrame;
    [SerializeField]
    GameObject[] teamBadge;
    [SerializeField]
    Sprite[] scoreFrames;
    [SerializeField]
    Sprite[] avatarFrames;

    public void Set(Player player,bool inGame)
    {
        playerName.text = ArabicFixer.Fix(player.Name,false,false);
        playerName.font = LanguageManager.Instance.GetFont();

        playerAvatar.sprite = player.Avatar;

        if (inGame)
        {
            this.score.text = score.ToString();
        }
        else
        {
            countScore(player.TotalScore - player.Score, player.TotalScore);
        }
    }

    IEnumerator countScore(float startScore,float endScore)
    {
        float timer = 1;

        score.text = startScore + " + <color=Green>" + endScore + "</color>";
        yield return new WaitForSeconds(0.5f);

        while (timer > 0)
        {
            timer -= Time.deltaTime;

            score.text = Mathf.Round(Mathf.Lerp(startScore,endScore,timer)).ToString() + " + <color=Green>" +
                Mathf.Round(Mathf.Lerp(endScore - startScore, 0, timer)).ToString() + "</color>";

            yield return null;
        }

        yield return new WaitForSeconds(1);

        score.text = endScore.ToString();
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
