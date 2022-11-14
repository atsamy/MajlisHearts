using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArabicSupport;
using UnityEngine.UI;
using TMPro;
using System;

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
    bool isComfirmed;

    public void Set(string name,bool isComfirmed)
    {
        ArabicFixerTool.useHinduNumbers = false;
        playerName.text = ArabicFixer.Fix(name,false,false);
        this.avatar.sprite = AvatarManager.Instance.GetPlayerAvatar(name);
        this.isComfirmed = isComfirmed;

        if (!isComfirmed)
        {
            for (int i = 0; i < statusText.Length; i++)
            {
                statusText[i].SetActive(i == 2);
            }
        }
    }

    public void SetOnline()
    {
        if (!isComfirmed)
            return;
        statusImage.sprite = statusSprites[1];

        statusText[0].SetActive(false);
        statusText[1].SetActive(true);

        InviteToggle.interactable = true;
    }

    public void SetOffline()
    {
        if (!isComfirmed)
            return;

        statusImage.sprite = statusSprites[0];

        statusText[0].SetActive(true);
        statusText[1].SetActive(false);

        InviteToggle.interactable = false;
    }

    internal void Confirm()
    {
        isComfirmed = true;
        SetOnline();
    }
}
