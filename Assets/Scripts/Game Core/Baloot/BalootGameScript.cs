using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BalootGameScript;

public class BalootGameScript : GameScriptBase
{
    public static BalootGameScript Instance;
    BalootMainPlayer myPlayer => (BalootMainPlayer)Players[0];

    public delegate void StartCardsReady(Card balootCard);
    public event StartCardsReady OnStartCardsReady;

    public delegate void PlayerSelectedType(int index, BalootGameType type);
    public event PlayerSelectedType OnPlayerSelectedType;

    public BalootRoundScript balootRoundScript => (BalootRoundScript)RoundScript;

    private void Awake()
    {
        Instance = this;
        RoundScript = new BalootRoundScript();
    }

    void Start()
    {
        balootRoundScript.OnEvent += Deal_OnEvent;

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

            ((BalootPlayer)Players[i]).OnTypeSelected += Players_SelectedType;
            Players[i].OnCardReady += GameScript_OnCardReady;

        }


        RoundScript.SetPlayers(Players);

        //SetEnvironment(GameManager.Instance.EquippedItem["TableTop"],
        //     GameManager.Instance.EquippedItem["CardBack"]);

        StartGame();
    }

    public void Players_SelectedType(int index, BalootGameType type)
    {
        OnPlayerSelectedType?.Invoke(index, type);
        switch (type)
        {
            case BalootGameType.Sun:
                balootRoundScript.SetGameType(index, type);
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

    private void Deal_OnEvent(int eventIndex)
    {
        EventTypeBaloot eventType = (EventTypeBaloot)eventIndex;
        switch (eventType)
        {
            case EventTypeBaloot.CardsDealtBegin:
                OnStartCardsReady?.Invoke(balootRoundScript.BalootCard);
                ((BalootPlayer)Players[0]).CheckGameType();
                break;
            case EventTypeBaloot.CardsDealtFinished:
                SetCardsReady();
                break;
            case EventTypeBaloot.TrickFinished:
                Deal_OnTrickFinished(RoundScript.PlayingIndex);
                break;
            case EventTypeBaloot.DealFinished:
                Deal_OnDealFinished();
                break;
        }
    }

}
