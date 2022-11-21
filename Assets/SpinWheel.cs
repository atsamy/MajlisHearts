using System.Collections;
using System.Collections.Generic;
using EasyUI.PickerWheelUI;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SpinWheel : MonoBehaviour
{
    [SerializeField]
    Button spinBtn;
    [SerializeField]
    Button claimBtn;
    [SerializeField]
    Button closeBtn;
    [SerializeField]
    PickerWheel pickerWheel;
    [SerializeField]
    GameObject rewardPanel;
    [SerializeField]
    Image rewardImage;
    [SerializeField]
    TextMeshProUGUI rewardAmount;
    Action claimReawrd;
    Action panelClosed;


    public void Open(Action onClosed)
    {
        gameObject.SetActive(true);
        panelClosed = onClosed;
    }
    // Start is called before the first frame update
    void Start()
    {
        spinBtn.onClick.AddListener(()=>
        {
            AdsManager.Instance.ShowRewardedAd((result) =>
            {
                if (result)
                {
                    pickerWheel.Spin();
                    spinBtn.interactable = false;
                }
            });
        });

        claimBtn.onClick.AddListener(() =>
        {
            claimReawrd?.Invoke();
            Invoke("ClosePanel", 1);
            rewardPanel.SetActive(false);
            GameSFXManager.Instance.PlayClip("Coins");
        });

        closeBtn.onClick.AddListener(() =>
        {
            panelClosed?.Invoke();
        });

        pickerWheel.OnSpinEnd((prize) =>
        {
            rewardAmount.text = prize.Amount.ToString();
            rewardImage.sprite = prize.Icon;
            rewardPanel.SetActive(true);

            claimReawrd = () =>
            {
                if (prize.Label == "Coins")
                {
                    GameManager.Instance.AddCoins(prize.Amount);
                }
                else if (prize.Label == "Gems")
                {
                    GameManager.Instance.AddGems(prize.Amount);
                }
            };
        });
    }

    void ClosePanel()
    {
        panelClosed?.Invoke();
    }
}
