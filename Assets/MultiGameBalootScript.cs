using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static MultiGameScript;

public class MultiGameBalootScript : GameScriptBaloot
{
    PunTurnManager turnManager;
    MultiPlayerScript multiPlayer;

    const int trickFinishedCode = 41;
    const int dealFinishedCode = 44;

    const int cardsDealtCode = 40;

    const int doubleCardCode = 45;
    const int checkDoubleCode = 46;

    const int checkTypeCode = 50;
    const int dealBeginCode = 51;
    const int playerSelectedTypeCode = 52;
    const int typeSelectedCode = 53;
    const int doubleSelectedCode = 54;
    const int changedShapeCode = 55;
    const int projectCode = 56;

    int projectsCount = 0;
    void Start()
    {
        turnManager = gameObject.AddComponent<PunTurnManager>();
        multiPlayer = new MultiPlayerScript(turnManager, this);


        for (int i = 0; i < 4; i++)
        {
            ((PlayerBaloot)Players[i]).OnTypeSelected += Players_SelectedType;
            ((PlayerBaloot)Players[i]).OnDoubleSelected += GameScriptBaloot_OnDoubleSelected;
            ((PlayerBaloot)Players[i]).OnChangedHokumShape += MultiGameBalootScript_OnChangedHokumShape;
        }


        RoundScript.SetPlayers(Players);

        balootRoundScript.OnEvent += Deal_OnEvent;
        balootRoundScript.OnGameTypeSelected += BalootRoundScript_OnGameTypeSelected;
        multiPlayer.OnNetworkEvent += MultiPlayer_OnNetworkEvent;
    }

    private void MultiGameBalootScript_OnChangedHokumShape(CardShape shape)
    {
        RaiseEventOptions eventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent(changedShapeCode, shape, eventOptions, SendOptions.SendReliable);
    }

    //protected override void BalootRoundScript_OnGameTypeSelected(int index, BalootGameType gameType)
    //{
    //    //revise
    //    if (PhotonNetwork.IsMasterClient)
    //    {
    //        base.BalootRoundScript_OnGameTypeSelected(index, gameType);
    //    }
    //    else
    //    {
    //        int[] data = new int[] { index, (int)gameType, (int)balootRoundScript.balootRoundInfo.HokumShape };
    //        RaiseEventOptions eventOptionsDeal = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };
    //        PhotonNetwork.RaiseEvent(typeSelectedCode, data, eventOptionsDeal, SendOptions.SendReliable);
    //    }
    //    //base.BalootRoundScript_OnGameTypeSelected(index, gameType);
    //}

    public override void Players_SelectedType(int index, BalootGameType type)
    {
        //if (PhotonNetwork.IsMasterClient)
        //{
        base.Players_SelectedType(index, type);

        RaiseEventOptions eventOptionsDeal = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent(playerSelectedTypeCode, new int[] { index, (int)type },
            eventOptionsDeal, SendOptions.SendReliable);
        //}
        //else
        //{
        //    RaiseEventOptions eventOptionsDeal = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };
        //    PhotonNetwork.RaiseEvent(playerSelectedTypeCode, new int[] { index, (int)type },
        //        eventOptionsDeal, SendOptions.SendReliable);
        //}
    }

    protected override void GameScriptBaloot_OnDoubleSelected(int playerIndex, bool isDouble, int value)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            base.GameScriptBaloot_OnDoubleSelected(playerIndex, isDouble, value);
        }
        else
        {
            int[] data = new int[] { playerIndex, isDouble ? 1 : 0, value };
            RaiseEventOptions eventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };
            PhotonNetwork.RaiseEvent(doubleSelectedCode, data, eventOptions, SendOptions.SendReliable);
        }
    }
    private void MultiPlayer_OnNetworkEvent(EventData photonEvent)
    {
        switch (photonEvent.Code)
        {
            case changedShapeCode:
                balootRoundScript.balootRoundInfo.HokumShape = (CardShape)photonEvent.CustomData;
                break;
            case checkTypeCode:
                int[] typeData = (int[])photonEvent.CustomData;
                balootRoundScript.HokumIndex = typeData[0];
                balootRoundScript.BiddingRound = typeData[1];

                ((PlayerBaloot)MyPlayer).CheckGameType(balootRoundScript);
                break;
            case playerSelectedTypeCode:
                int[] data = (int[])photonEvent.CustomData;
                base.Players_SelectedType(data[0], (BalootGameType)data[1]);

                break;
            //case typeSelectedCode:
            //    int[] typeSelectedData = (int[])photonEvent.CustomData;
            //    balootRoundScript.balootRoundInfo.HokumShape = (CardShape)typeSelectedData[2];
            //    base.BalootRoundScript_OnGameTypeSelected(typeSelectedData[0],
            //        (BalootGameType)typeSelectedData[1]);
            //    break;
            case doubleSelectedCode:
                int[] doubleSelectedData = (int[])photonEvent.CustomData;
                base.GameScriptBaloot_OnDoubleSelected(doubleSelectedData[0],
                    (doubleSelectedData[1] == 1), doubleSelectedData[2]);
                break;
            case cardsDealtCode:
                projectsCount = 0;
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
            case dealBeginCode:
                List<Card> allCards = Utils.DeSerializeListOfCards((int[])photonEvent.CustomData);
                balootRoundScript.BalootCard = allCards.First();
                balootRoundScript.StartIndex++;
                allCards.RemoveAt(0);
                MyPlayer.OwnedCards.AddRange(allCards);
                DealCards();
                break;
            case trickFinishedCode:
                if (balootRoundScript.RoundInfo.TrickNumber == 1)
                {
                    PlayerBaloot player = (PlayerBaloot)MyPlayer;
                    if (((PlayerBaloot)MyPlayer).PlayerProjects.Count > 0)
                    {
                        //send my projects
                        RaiseEventOptions eventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };
                        PhotonNetwork.RaiseEvent(projectCode, Utils.SerializeProjects(player.PlayerProjects,
                            player.ProjectPower, player.ProjectScore), eventOptions, SendOptions.SendReliable);
                    }
                    else 
                    {
                        RaiseEventOptions eventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };
                        PhotonNetwork.RaiseEvent(projectCode, null, eventOptions, SendOptions.SendReliable);
                    }
                }
                break;
            case projectCode:
                if (photonEvent.CustomData != null)
                {
                    int power;
                    int score;
                    Dictionary<List<Card>, Projects> projects = Utils.DeserializeProjects((int[])photonEvent.CustomData, out power, out score);
                    int index = multiPlayer.LookUpActors.First(x => x.Value == photonEvent.Sender).Key;
                    ((PlayerBaloot)Players[index]).SetProjects(projects, power, score);
                }
                projectsCount++;
                if (projectsCount == 4)
                    CompareProjects();
                break;
        }
    }

    private void Deal_OnEvent(int eventIndex)
    {
        EventTypeBaloot eventType = (EventTypeBaloot)eventIndex;

        switch (eventType)
        {
            case EventTypeBaloot.CardsDealtBegin:
                //called for master client only after initial cards are dealt
                DealCards();

                foreach (var item in multiPlayer.LookUpActors)
                {
                    if (item.Key == 0)
                        continue;

                    List<Card> allCards = new List<Card>
                    {
                        balootRoundScript.BalootCard
                    };

                    allCards.AddRange(Players[item.Key].OwnedCards);

                    RaiseEventOptions eventOptionsCards = new RaiseEventOptions { TargetActors = new int[] { item.Value } };
                    PhotonNetwork.RaiseEvent(dealBeginCode, Utils.SerializeListOfCards(allCards),
                        eventOptionsCards, SendOptions.SendReliable);
                }
                break;
            case EventTypeBaloot.CardsDealtFinished:
                DealRemaingCards();
                break;
            case EventTypeBaloot.RestartDeal:
                break;
            case EventTypeBaloot.TrickFinished:
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                PhotonNetwork.RaiseEvent(trickFinishedCode, RoundScript.PlayingIndex, raiseEventOptions, SendOptions.SendReliable);

                SetTrickFinished(RoundScript.PlayingIndex);
                break;
            case EventTypeBaloot.DealFinished:
                RaiseEventOptions eventOptionsDeal = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
                PhotonNetwork.RaiseEvent(dealFinishedCode, null, eventOptionsDeal, SendOptions.SendReliable);

                SetDealFinished(true);
                break;
        }
    }

    public override void CheckType()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (!Players[balootRoundScript.StartIndex].IsPlayer)
            {
                ((PlayerBaloot)Players[balootRoundScript.StartIndex]).CheckGameType(balootRoundScript);
            }
            else
            {
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { TargetActors = new int[] { multiPlayer.LookUpActors[balootRoundScript.StartIndex] } };
                //we need to send parameters to check type
                int[] data = new int[] { balootRoundScript.HokumIndex, balootRoundScript.BiddingRound };
                PhotonNetwork.RaiseEvent(checkTypeCode, data, raiseEventOptions, SendOptions.SendReliable);
            }
        }
    }
}
