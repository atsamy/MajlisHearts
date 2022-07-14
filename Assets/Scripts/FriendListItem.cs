using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FriendListItem : MonoBehaviour
{
    public Toggle InviteToggle;

    [SerializeField]
    TextMeshProUGUI playerName;
    [SerializeField]
    Image avatar;
    [SerializeField]
    TextMeshProUGUI statusText;
    [SerializeField]
    Image statusImage;
    [SerializeField]
    Image frameImage;
    [SerializeField]
    Sprite[] frameSprites;
    [SerializeField]
    Sprite[] statusSprites;


    public void Set(string name)
    {
        playerName.text = name;
        this.avatar.sprite = AvatarManager.Instance.GetPlayerAvatar(name);
    }

    public void SetOnline()
    {
        frameImage.sprite = frameSprites[1];
        statusImage.sprite = statusSprites[1];

        statusText.text = "ONLINE";
        InviteToggle.interactable = true;
    }

    public void SetOffline()
    {
        frameImage.sprite = frameSprites[0];
        statusImage.sprite = statusSprites[0];

        statusText.text = "OFFLINE";
        InviteToggle.interactable = false;
    }
}
