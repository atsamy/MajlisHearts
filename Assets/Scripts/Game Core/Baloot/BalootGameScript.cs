using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalootGameScript : GameScriptBase
{
    public static BalootGameScript Instance;

    private void Awake()
    {
        Instance = this;
        RoundScript = new BalootRoundScript();
    }

    void Start()
    {
        //((BalootRoundScript)RoundScript).OnEvent += Deal_OnEvent;

        //Players = new Player[4];

        //for (int i = 0; i < 4; i++)
        //{
        //    if (i == 0)
        //    {
        //        Players[i] = new MainPlayer(i);
        //        Players[i].Avatar = AvatarManager.Instance.playerAvatar;
        //        Players[i].Name = GameManager.Instance.MyPlayer.Name;
        //    }
        //    else
        //    {
        //        Players[i] = new AIPlayer(i);
        //        Players[i].Avatar = AvatarManager.Instance.RobotAvatar;
        //        Players[i].Name = "Player " + i;
        //    }

        //    Players[i].OnPassCardsReady += GameScript_OnPassCardsReady;
        //    Players[i].OnCardReady += GameScript_OnCardReady;
        //    Players[i].OnDoubleCard += GameScript_OnDoubleCard;

        //}

        //myPlayer = (MainPlayer)Players[0];
        //myPlayer.OnPlayerTurn += MainPlayerTurn;

        //((RoundScript)RoundScript).SetPlayers(Players);

        SetEnvironment(GameManager.Instance.EquippedItem["TableTop"],
             GameManager.Instance.EquippedItem["CardBack"]);

        StartGame();
    }
}
