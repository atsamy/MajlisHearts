using ArabicSupport;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HeaderScript : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI currencyText;
    [SerializeField]
    Text levelText;
    [SerializeField]
    Text userNameText;
    [SerializeField]
    Image avatar;
    void Start()
    {
        currencyText.text = (GameManager.Instance.Currency.ToString());
        levelText.text = GameManager.Instance.MyPlayer.Level.ToString();
        userNameText.text = ArabicFixer.Fix(GameManager.Instance.MyPlayer.Name,false,false);
    }

    public void SetAvatar()
    {
        AvatarManager.Instance.SetPlayerAvatar(GameManager.Instance.MyPlayer.Avatar);
        avatar.sprite = AvatarManager.Instance.playerAvatar;
    }
}
