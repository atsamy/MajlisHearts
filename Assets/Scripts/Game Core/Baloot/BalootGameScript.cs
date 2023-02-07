using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BalootGameScript;

public class BalootGameScript : GameScriptBase
{
    public static BalootGameScript Instance;
    BalootMainPlayer myPlayer;

    public delegate void StartCardsReady(Card balootCard);
    public event StartCardsReady OnStartCardsReady;

    private void Awake()
    {
        Instance = this;
        RoundScript = new BalootRoundScript();
    }

    void Start()
    {
        ((BalootRoundScript)RoundScript).OnEvent += Deal_OnEvent;

        Players = new BalootPlayer[4];

        for (int i = 0; i < 4; i++)
        {
            if (i == 0)
            {
                Players[i] = new BalootMainPlayer(i);
                //Players[i].Avatar = AvatarManager.Instance.playerAvatar;
                Players[i].Name = GameManager.Instance.MyPlayer.Name;
            }
            else
            {
                Players[i] = new BalootAIPlayer(i);
                //Players[i].Avatar = AvatarManager.Instance.RobotAvatar;
                Players[i].Name = "Player " + i;
            }

            ((BalootPlayer)Players[i]).OnTypeSelected += PlayerSelectedType;
            Players[i].OnCardReady += GameScript_OnCardReady;

        }

        myPlayer = (BalootMainPlayer)Players[0];
        //myPlayer.OnPlayerTurn += MainPlayerTurn;

        RoundScript.SetPlayers(Players);

        //SetEnvironment(GameManager.Instance.EquippedItem["TableTop"],
        //     GameManager.Instance.EquippedItem["CardBack"]);

        StartGame();
    }

    public void PlayerSelectedType(int index,BalootGameType type)
    {
        switch (type) 
        {
            case BalootGameType.Sun:
                ((BalootRoundScript)RoundScript).SetGameType(index,type);
                break;
            case BalootGameType.Hukom:
                break;
            case BalootGameType.Ashkal: 
                break;
            case BalootGameType.Pass:
                break;
        }
    }

    private void GameScript_OnCardReady(int playerIndex, Card card)
    {
        RoundScript.OnCardReady(playerIndex, card);
    }

    private void Deal_OnEvent(EventTypeBaloot eventType)
    {
        switch (eventType)
        {
            case EventTypeBaloot.CardsDealtBegin:
                OnStartCardsReady?.Invoke(((BalootRoundScript)RoundScript).BalootCard);
                break;
            case EventTypeBaloot.CardsDealtFinished:
                SetCardsReady();
                break;
        }
    }
}
