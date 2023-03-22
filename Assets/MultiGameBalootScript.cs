using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MultiGameScript;

public class MultiGameBalootScript : GameScriptBaloot
{
    PunTurnManager turnManager;
    MultiPlayerScript multiPlayer;

    const int trickFinishedCode = 41;
    const int dealFinishedCode = 44;

    const int cardsDealtCode = 40;

    const int passCardsCode = 42;
    const int gameReadyCode = 43;

    const int doubleCardCode = 45;
    const int checkDoubleCode = 46;
    const int messageCode = 47;
    //const int recievedCardsCode = 48;
    const int playerTurnCode = 49;

    void Start()
    {
        turnManager = gameObject.AddComponent<PunTurnManager>();
        multiPlayer = new MultiPlayerScript(turnManager,this);


        for (int i = 0; i < 4; i++)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (i == 0)
                {
                    Players[i] = new MainPlayerBaloot(i);
                    Players[i].Name = playersOrder[i];
                    Players[i].Avatar = AvatarManager.Instance.playerAvatar;//lookUpAvatar[i];
                }
                else if (lookUpActors.ContainsKey(i))
                {
                    Players[i] = new PlayerBaloot(i);
                    Players[i].Name = playersOrder[i];
                    Players[i].Avatar = AvatarManager.Instance.GetPlayerAvatar(playersOrder[i]);
                }
                else
                {
                    Players[i] = new AIPlayerBaloot(i);
                    Players[i].Name = playersOrder[i];
                    Players[i].Avatar = AvatarManager.Instance.GetPlayerAvatar(playersOrder[i]);

                    if (GameManager.Instance.GameType == GameType.Online)
                        ((AIPlayer)Players[i]).FakePlayer = true;
                }
            }
            else
            {
                if (i < playersOrder.Length && playersOrder[i] == GameManager.Instance.MyPlayer.Name)
                {
                    Players[i] = new MainPlayerBaloot(i);
                    MainPlayerIndex = i;
                    Players[i].Avatar = AvatarManager.Instance.playerAvatar;
                    Players[i].Name = playersOrder[i];
                }
                else
                {
                    Players[i] = new Player(i);
                    Players[i].Avatar = AvatarManager.Instance.GetPlayerAvatar(playersOrder[i]);
                    Players[i].Name = playersOrder[i];
                }
            }

            //((Player)Players[i]).OnPassCardsReady += GameScript_OnPassCardsReady;
            //Players[i].OnCardReady += GameScript_OnCardReady;
            //((Player)Players[i]).OnDoubleCard += GameScript_OnDoubleCard;
            //Players[i].OnPlayerTurn += GameScript_OnPlayerTurn;
        }


        RoundScript.SetPlayers(Players);
        ((RoundScriptBaloot)RoundScript).OnEvent += Deal_OnEvent;

        multiPlayer.OnNetworkEvent += MultiPlayer_OnNetworkEvent;
    }

    private void MultiPlayer_OnNetworkEvent(EventData photonEvent)
    {
        switch (photonEvent.Code)
        {
            case cardsDealtCode:
                List<Card> cards = Utils.DeSerializeListOfCards((int[])photonEvent.CustomData);
                ((RoundScriptHeats)RoundScript).RoundInfo = new RoundInfo();
                MyPlayer.Reset();

                foreach (var item in cards)
                {
                    MyPlayer.AddCard(item);
                }

                SetCardsReady();
                ((MainPlayer)MyPlayer).SelectPassCards();
                break;
        }
    }

    private void Deal_OnEvent(int eventIndex)
    {
        EventTypeBaloot eventType = (EventTypeBaloot)eventIndex;

        switch (eventType)
        {
            case EventTypeBaloot.CardsDealtBegin:
                break;
            case EventTypeBaloot.CardsDealtFinished:
                break;
            case EventTypeBaloot.RestartDeal:
                break;
            case EventTypeBaloot.TrickFinished:
                SetTrickFinished(RoundScript.PlayingIndex);

                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                PhotonNetwork.RaiseEvent(trickFinishedCode, RoundScript.PlayingIndex, raiseEventOptions, SendOptions.SendReliable);
                break;
            case EventTypeBaloot.DealFinished:
                RaiseEventOptions eventOptionsDeal = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
                PhotonNetwork.RaiseEvent(dealFinishedCode, null, eventOptionsDeal, SendOptions.SendReliable);

                SetDealFinished(true);
                break;
        }
    }
}
