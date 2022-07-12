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
    UsernamePanel changeUserName;
    [SerializeField]
    TextMeshProUGUI headerUserName;



    private void Start()
    {
        playerName.text = GameManager.Instance.MyPlayer.Name;
        score.text = GameManager.Instance.MyPlayer.Score.ToString();
    }

    public void OpenStore(int index)
    {
        Close();
        MenuManager.Instance.OpenStore(index);
    }

    public void EditName()
    {
        changeUserName.Show((name)=>
        {
            headerUserName.text = name;
            playerName.text = name;
            GameManager.Instance.MyPlayer.Name = name;
        });
    }
}
