using ArabicSupport;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class HeaderScript : MonoBehaviour
{
    [SerializeField]
    Text currencyText;
    [SerializeField]
    Text levelText;
    [SerializeField]
    Text userNameText;
    [SerializeField]
    Image avatar;
    void Start()
    {
        currencyText.text = (GameManager.Instance.Currency.ToString());
        levelText.text = GameManager.Instance.MyPlayer.GetLevel().ToString();
        userNameText.text = ArabicFixer.Fix(GameManager.Instance.MyPlayer.Name);

        GameManager.Instance.OnCurrencyChanged += OnCurrencyChanged;
    }

    public void SetAvatar()
    {
        AvatarManager.Instance.SetPlayerAvatar(GameManager.Instance.MyPlayer.Avatar);
        avatar.sprite = AvatarManager.Instance.playerAvatar;
    }

    void OnCurrencyChanged(int value)
    {
        currencyText.text = (value.ToString());
    }
}
