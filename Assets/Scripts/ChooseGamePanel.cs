using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseGamePanel : MenuScene
{
    public void SetGameType(int type)
    {
        GameManager.Instance.Game = (Game)type;
        MenuManager.Instance.OpenGameMode();
        gameObject.SetActive(false);
    }
}
