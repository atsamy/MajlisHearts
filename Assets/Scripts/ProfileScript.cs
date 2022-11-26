using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ArabicSupport;

public class ProfileScript : MenuScene
{
    [SerializeField]
    TextMeshProUGUI playerName;
    [SerializeField]
    TextMeshProUGUI majlisName;
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
    [SerializeField]
    Image cardBack;
    [SerializeField]
    Image tableTop;



    private void Start()
    {
        playerName.text = ArabicFixer.Fix(GameManager.Instance.MyPlayer.Name,false,false);
        majlisName.text = ArabicFixer.Fix(GameManager.Instance.MyPlayer.MajlisName, false, false);
    }

    private void OnEnable()
    {
        avatar.sprite = AvatarManager.Instance.playerAvatar;
        cardBack.sprite = Resources.Load<Sprite>("CardBack/" + GameManager.Instance.EquippedItem["CardBack"]);
        tableTop.sprite = Resources.Load<Sprite>("TableTop/" + GameManager.Instance.EquippedItem["TableTop"]);
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
            headerUserName.text = ArabicFixer.Fix(name);
            playerName.text = ArabicFixer.Fix(name);
            GameManager.Instance.MyPlayer.Name = name;
        });
    }
}
