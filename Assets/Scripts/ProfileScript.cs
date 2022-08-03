using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ArabicSupport;

public class ProfileScript : MenuScene
{
    [SerializeField]
    Text playerName;
    //[SerializeField]
    //TextMeshProUGUI score;
    [SerializeField]
    Image levelFill;
    [SerializeField]
    TextMeshProUGUI level;
    [SerializeField]
    UsernamePanel changeUserName;
    [SerializeField]
    Text headerUserName;
    [SerializeField]
    Image avatar;



    private void Start()
    {
        playerName.text = ArabicFixer.Fix(GameManager.Instance.MyPlayer.Name);
        playerName.font = LanguageManager.Instance.GetFont();

        //score.text = GameManager.Instance.MyPlayer.Score.ToString();
        levelFill.fillAmount = GameManager.Instance.MyPlayer.CurrentPogress;
        level.text = GameManager.Instance.MyPlayer.Level.ToString();
    }

    private void OnEnable()
    {
        avatar.sprite = AvatarManager.Instance.playerAvatar;
    }

    public void OpenStore(int index)
    {
        Close();
        MenuManager.Instance.OpenStore(index);
        //SFXManager.Instance.PlayClip("Click");
    }

    public void EditName()
    {
        SFXManager.Instance.PlayClip("Click");

        changeUserName.Show((name)=>
        {
            headerUserName.text = name;
            playerName.text = name;
            GameManager.Instance.MyPlayer.Name = name;
        });
    }
}
