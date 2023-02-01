using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using ArabicSupport;

public class FriendStatus : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI playerNameText;
    [SerializeField]
    TextMeshProUGUI levelText;
    [SerializeField]
    TextMeshProUGUI statusText;
    [SerializeField]
    Image avatar;

    [SerializeField]
    Material readyMaterial;
    [SerializeField]
    Material declinedMaterial;
    [SerializeField]
    Material waitingMaterial;

    internal playerStatus CurrentStatus;

    public string PlayerName { get; private set; }

    public void Set(string playerName,string avatar,int level,string status)
    {
        playerNameText.text = ArabicFixer.Fix(playerName,false,false);
        levelText.text = level.ToString();
        statusText.text = LanguageManager.Instance.GetString(status);

        if(playerName != GameManager.Instance.MyPlayer.Name)
            AvatarManager.Instance.SetPlayerAvatar(playerName,avatar);

        this.avatar.sprite = AvatarManager.Instance.GetPlayerAvatar(playerName);
        PlayerName = playerName;

        if (status == "waiting")
        {
            statusText.fontMaterial = waitingMaterial;
        }

        CurrentStatus = new playerStatus();
        CurrentStatus.Avatar = avatar;
        CurrentStatus.playerName = playerName;
        CurrentStatus.Status = status;
    }

    public void ChangeStatus(string status)
    {
        statusText.text = LanguageManager.Instance.GetString(status);
        statusText.fontMaterial = status == "ready" ? readyMaterial : declinedMaterial;

        CurrentStatus.Status = status;
    }

    internal void Set(playerStatus playerStatus)
    {
        playerNameText.text = ArabicFixer.Fix(playerStatus.playerName, false, false);
        statusText.text = LanguageManager.Instance.GetString(playerStatus.Status);

        AvatarManager.Instance.SetPlayerAvatar(playerStatus.playerName, playerStatus.Avatar);
        this.avatar.sprite = AvatarManager.Instance.GetPlayerAvatar(playerStatus.playerName);
        PlayerName = playerStatus.playerName;

        if (playerStatus.Status == "waiting")
        {
            statusText.fontMaterial = waitingMaterial;
        }
        else if (playerStatus.Status != "ready")
        {
            statusText.fontMaterial = declinedMaterial;
        }

        CurrentStatus = playerStatus;
    }

    internal void Reset()
    {
        playerNameText.text = "Bot";
        //levelText.text = level.ToString();
        statusText.text = LanguageManager.Instance.GetString("ready");
        this.avatar.sprite = AvatarManager.Instance.RobotAvatar;
        PlayerName = "";
        statusText.fontMaterial = readyMaterial;
    }

    public void SetLanguage()
    {
        statusText.text = LanguageManager.Instance.GetString("ready");
    }
}
