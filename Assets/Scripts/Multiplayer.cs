using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ArabicSupport;
using System;

public class Multiplayer : MonoBehaviour
{
    [SerializeField]
    Image playerAvatar;
    [SerializeField]
    Image playerFrame;
    [SerializeField]
    TextMeshProUGUI playerName;
    [SerializeField]
    TextMeshProUGUI status;
    [SerializeField]
    Material readyMaterial;
    [SerializeField]
    Material hostMaterial;
    [SerializeField]
    Material waitingMaterial;
    Sprite defaultSprite;
    [SerializeField]
    Sprite[] frameSprites;

    public string PlayerName { get; private set; }
    public bool IsSet { get; private set; }

    private void Start()
    {
        status.text = LanguageManager.Instance.GetString("waiting");
        defaultSprite = playerAvatar.sprite;
    }

    public void Set(string name, bool isMe, bool isHost)
    {
        PlayerName = name;
        IsSet = true;

        playerName.text = ArabicFixer.Fix(name,false,false);
        gameObject.SetActive(true);

        playerFrame.sprite = frameSprites[isMe ? 0 : 1];

        if (isHost)
        {
            status.text = LanguageManager.Instance.GetString("host");
            status.fontMaterial = hostMaterial;
        }
        else
        {
            status.text = LanguageManager.Instance.GetString("ready");
            status.fontMaterial = readyMaterial;
        }

        playerAvatar.sprite = isMe ? AvatarManager.Instance.playerAvatar : AvatarManager.Instance.GetPlayerAvatar(name);
    }

    internal void Reset()
    {
        status.text = LanguageManager.Instance.GetString("waiting");
        status.fontMaterial = waitingMaterial;
        playerAvatar.sprite = defaultSprite;
        playerName.text = string.Empty;
        playerFrame.sprite = frameSprites[1];
    }
}
