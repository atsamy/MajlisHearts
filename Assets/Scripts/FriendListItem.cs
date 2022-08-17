using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArabicSupport;
using UnityEngine.UI;

public class FriendListItem : MonoBehaviour
{
    public Toggle InviteToggle;

    [SerializeField]
    Text playerName;
    [SerializeField]
    Image avatar;
    [SerializeField]
    Text statusText;
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
        ArabicFixerTool.useHinduNumbers = false;

        playerName.text = ArabicFixer.Fix(name,false,false);
        playerName.font = LanguageManager.Instance.GetFont();

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
