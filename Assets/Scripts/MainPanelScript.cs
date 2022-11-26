using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using ArabicSupport;
using System;

public class MainPanelScript : MonoBehaviour
{
    [SerializeField]
    GameObject header;
    [SerializeField]
    GameObject main;
    [SerializeField]
    GameObject coins;
    [SerializeField]
    GameObject gems;
    [SerializeField]
    ChangeNumber coinsText;
    [SerializeField]
    ChangeNumber gemsText;
    [SerializeField]
    TextMeshProUGUI levelText;
    [SerializeField]
    TextMeshProUGUI userNameText;
    [SerializeField]
    Image avatar;
    [SerializeField]
    MenuScene Profile;
    [SerializeField]
    Image fillLevel;

    RectTransform coinsRect;

    public bool IsOnMain { get; private set; }

    private void Awake()
    {
        coinsRect = coins.GetComponent<RectTransform>();
        IsOnMain = true;
    }

    private void OnEnable()
    {
        fillLevel.fillAmount = (GameManager.Instance.MyPlayer.CurrentPogress + 1) / 2;
    }

    void Start()
    {
        coinsText.setNumber(GameManager.Instance.Coins);
        gemsText.setNumber(GameManager.Instance.Gems);

        levelText.text = GameManager.Instance.MyPlayer.Level.ToString();
        userNameText.text = ArabicFixer.Fix(GameManager.Instance.MyPlayer.Name, false, false);

        GameManager.Instance.OnCoinsChanged += OnCurrencyChanged;
        GameManager.Instance.OnGemsChanged += OnGemsChanged;
    }

    private void OnCurrencyChanged(int value)
    {
        coinsText.Change(value);
    }

    private void OnGemsChanged(int value)
    {
        gemsText.Change(value);
    }

    public void SetAvatar()
    {
        AvatarManager.Instance.SetPlayerAvatar(GameManager.Instance.MyPlayer.Avatar);
        avatar.sprite = AvatarManager.Instance.playerAvatar;
    }

    public void HideHeader(bool hideGems, bool hideCoins)
    {
        header.SetActive(false);
        main.SetActive(false);
        if (!hideCoins)
        {
            coinsRect.DOAnchorPos(new Vector2(300, -105), 0.25f);
        }
        else
        {
            coins.SetActive(false);
        }

        if (hideGems)
        {
            gems.SetActive(false);
        }

        IsOnMain = false;
    }

    public void ShowHeader()
    {
        header.SetActive(true);
        main.SetActive(true);

        if (coins.activeSelf)
        {
            coinsRect.DOAnchorPos(new Vector2(900, -105), 0.25f);
        }
        else
        {
            coins.SetActive(true);
            coinsRect.anchoredPosition = new Vector2(900, -105);
        }

        gems.SetActive(true);
        IsOnMain = true;
    }

    public void OpenProfile()
    {
        MenuManager.Instance.HideMain(true, true);
        Profile.Open();
    }

    private void OnDisable()
    {
        GameManager.Instance.OnCoinsChanged -= OnCurrencyChanged;
        GameManager.Instance.OnGemsChanged -= OnGemsChanged;
    }
}
