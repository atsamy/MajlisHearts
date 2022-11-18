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
    string playfabID;

    public bool IsConfirmed => isComfirmed;

    public void Set(string name,bool isComfirmed,string playfabID,Action<bool> OnValueChanged)
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

        InviteToggle.onValueChanged.AddListener((value)=>
        {
            OnValueChanged?.Invoke(value);
        });

        this.playfabID = playfabID;
    }

    public void SetOnline()
    {
        if (!isComfirmed)
            return;
        statusImage.sprite = statusSprites[1];

        statusText[0].SetActive(false);
        statusText[1].SetActive(true);
        statusText[2].SetActive(false);

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

    public void Remove()
    {
        MenuManager.Instance.OpenPopup("deletefriend",true,true,()=>
        {
            PlayfabManager.instance.DenyFriendRequest(playfabID,(result) =>
            {
                if (result)
                {
                    Destroy(gameObject);
                }
                else
                {
                    MenuManager.Instance.OpenPopup(("somethingwrong"),true,true);
                }
            });
        });

    }

    internal void Confirm()
    {
        isComfirmed = true;
        SetOnline();
    }
}
