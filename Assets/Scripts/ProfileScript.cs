using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ProfileScript : MenuScene
{
    [SerializeField]
    TextMeshProUGUI playerName;
    [SerializeField]
    TextMeshProUGUI score;
    [SerializeField]
    Image levelFill;
    [SerializeField]
    TextMeshProUGUI level;
    [SerializeField]
    UsernamePanel changeUserName;
    [SerializeField]
    TextMeshProUGUI headerUserName;
    [SerializeField]
    Image avatar;



    private void Start()
    {
        playerName.text = GameManager.Instance.MyPlayer.Name;
        score.text = GameManager.Instance.MyPlayer.Score.ToString();
        levelFill.fillAmount = GameManager.Instance.MyPlayer.LevelProgress();
        level.text = GameManager.Instance.MyPlayer.GetLevel().ToString();
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
