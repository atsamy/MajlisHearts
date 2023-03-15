using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardEntry : MonoBehaviour
{
    [SerializeField]
    Image avatar;

    [SerializeField]
    TextMeshProUGUI rankText;

    [SerializeField]
    Image rankImage;

    [SerializeField]
    Sprite[] rankSprites;

    [SerializeField]
    Image cup;

    [SerializeField]
    Sprite[] cupSprites;

    [SerializeField]
    Image frame;

    [SerializeField]
    Image[] innerFrames;

    [SerializeField]
    Sprite frameSprite;

    [SerializeField]
    Sprite innerFrameSprite;

    [SerializeField]
    TextMeshProUGUI nameText;

    [SerializeField]
    TextMeshProUGUI pointsText;

    public void Set(int rank,string name,int points,string avatarName)
    {
        nameText.text = name;
        pointsText.text = points.ToString();

        if (rank < 4)
        {
            cup.sprite = cupSprites[rank - 1];
            cup.gameObject.SetActive(true);
            rankImage.gameObject.SetActive(true);
            rankImage.sprite = rankSprites[rank - 1];

            rankText.gameObject.SetActive(false);
        }
        else 
        {
            cup.gameObject.SetActive(false);
            rankText.gameObject.SetActive(true);
            rankText.text = rank.ToString();
        }

        if (name == GameManager.Instance.MyPlayer.Name)
        {
            frame.sprite = frameSprite;

            foreach (var item in innerFrames)
            {
                item.sprite = innerFrameSprite;
            }

            avatar.sprite = AvatarManager.Instance.playerAvatar;
        }
        else
        {
            avatar.sprite = AvatarManager.Instance.GetAvatarSprite(avatarName);
        }
    }
}
