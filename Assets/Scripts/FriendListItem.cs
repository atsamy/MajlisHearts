using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArabicSupport;
using UnityEngine.UI;
using TMPro;

public class FriendListItem : MonoBehaviour
{
    public Toggle InviteToggle;

    [SerializeField]
    TextMeshProUGUI playerName;
    [SerializeField]
    Image avatar;
    [SerializeField]
    GameObject[] statusText;
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
        //playerName.font = LanguageManager.Instance.GetFont();

        this.avatar.sprite = AvatarManager.Instance.GetPlayerAvatar(name);
    }

    public void SetOnline()
    {
        statusImage.sprite = statusSprites[1];

        statusText[0].SetActive(false);
        statusText[1].SetActive(true);

        InviteToggle.interactable = true;
    }

    public void SetOffline()
    {
        statusImage.sprite = statusSprites[0];

        statusText[0].SetActive(true);
        statusText[1].SetActive(false);

        InviteToggle.interactable = false;
    }
}
