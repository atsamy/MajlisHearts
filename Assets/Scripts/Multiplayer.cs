using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ArabicSupport;

public class Multiplayer : MonoBehaviour
{
    [SerializeField]
    Image playerAvatar;
    [SerializeField]
    Image playerFrame;
    [SerializeField]
    TextMeshProUGUI playerName;
    [SerializeField]
    GameObject[] label;
    [SerializeField]
    Sprite[] frameSprites;


    public void Set(string name, bool isMe, bool isHost)
    {
        playerName.text = ArabicFixer.Fix(name,false,false);
        gameObject.SetActive(true);
        //playerName.font = LanguageManager.Instance.GetFont();

        playerFrame.sprite = frameSprites[isMe ? 0 : 1];
        label[isHost ? 0 : 1].SetActive(true);
        label[isHost ? 1 : 0].SetActive(false);

        playerAvatar.sprite = isMe ? AvatarManager.Instance.playerAvatar : AvatarManager.Instance.GetPlayerAvatar(name);
    }
}
