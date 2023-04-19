using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
//using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiGameScript : GameScript, ILeaveRoom, ISendMessage
{
    PunTurnManager turnManager;
    MultiPlayerScript multiPlayer;

    int passedCardsNo;

    const int cardsDealtCode = 40;
    const int trickFinishedCode = 41;
    const int roundFinishedCode = 44;

    const int startGameCode = 43;
    const int gameReadyCode = 52;
    const int passCardsCode = 50;

    const int doubleCardCode = 45;
    const int checkDoubleCode = 46;

    public delegate void messageRecieved(int playerIndex, object message);
    public static messageRecieved OnMessageRecieved;

    //Dictionary<int, int> lookUpActors;

    void Start()
    {
        turnManager = gameObject.AddComponent<PunTurnManager>();
        multiPlayer = new MultiPlayerScript(turnManager, this, 8);

        passedCardsNo = 0;
        //Players = new Player[4];
        //playerNumbers = PhotonNetwork.PlayerList.Length;

        //lookUpActors = new Dictionary<int, int>();
        //Dictionary<int, string> lookUpAvatar = new Dictionary<int, string>();

        //RoundScript.SetPlayers(Players);
        //((RoundScriptHearts)RoundScript).OnEvent += Deal_OnEvent;

        for (int i = 0; i < 4; i++)
        {
            ((Player)Players[i]).OnPassCardsReady += GameScript_OnPassCardsReady;
            ((Player)Players[i]).OnDoubleCard += GameScript_OnDoubleCard;
        }

        RoundScript.SetPlayers(Players);

        RoundScript.OnEvent += Deal_OnEvent;
        multiPlayer.OnNetworkEvent += OnEvent;

        //OnStartPlaying?.Invoke(false);
    }


    private void GameScript_OnDoubleCard(Card card, bool value, int index)
    {
        RaiseEventOptions eventOptionsCards = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(doubleCardCode, Utils.SerializeCardValueAndIndex(card, value, index),
            eventOptionsCards, SendOptions.SendReliable);
    }

    private void OnDisable()
    {
        multiPlayer.OnDisable();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S) && (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsEditor))
        {
            foreach (var item in Players)
            {
                item.Score = 140;
            }
        }
    }

    //private void MultiPlayer_OnNetworkEvent(EventData photonEvent)
    //{
    //    //print(photonEvent.Code);
    //    switch (photonEvent.Code)
    //    {
    //        case 73:
    //            TeamsTotalScore[0] = 150;
    //            TeamsTotalScore[1] = 150;
    //            break;
    //        case RemaingCardsCode:
    //            List<Card> ownedCards = Utils.DeSerializeListOfCards((int[])photonEvent.CustomData);
    //            for (int i = 5; i < ownedCards.Count; i++)
    //            {
    //                MyPlayer.AddCard(ownedCards[i]);
    //            }

    //            WaitCardDealing();
    //            break;
    //        case checkDoubleCode:
    //            int[] doubleData = (int[])photonEvent.CustomData;
    //            ((PlayerBaloot)Players[doubleData[0]]).CheckDouble(doubleData[1]);
    //            break;
    //        case continueDealCode:
    //            balootRoundScript.DealContinue((int)photonEvent.CustomData);
    //            break;
    //        case changedShapeCode:
    //            balootRoundScript.balootRoundInfo.HokumShape = (CardShape)photonEvent.CustomData;
    //            break;
    //        case checkTypeCode:
    //            int[] typeData = (int[])photonEvent.CustomData;
    //            balootRoundScript.HokumIndex = typeData[0];
    //            balootRoundScript.BiddingRound = typeData[1];

    //            ((PlayerBaloot)Players[typeData[2]]).CheckGameType(balootRoundScript);
    //            break;
    //        case playerSelectedTypeCode:
    //            int[] data = (int[])photonEvent.CustomData;
    //            //use a function to save data not send events
    //            base.Players_SelectedType(data[0], (BalootGameType)data[1]);
    //            break;
    //        case doubleSelectedCode:
    //            int[] doubleSelectedData = (int[])photonEvent.CustomData;
    //            base.GameScriptBaloot_OnDoubleSelected(doubleSelectedData[0],
    //                (doubleSelectedData[1] == 1), doubleSelectedData[2]);
    //            break;
    //        case dealBeginCode:
    //            foreach (var item in Players)
    //            {
    //                item.Reset();
    //            }

    //            List<Card> allCards = Utils.DeSerializeListOfCards((int[])photonEvent.CustomData);
    //            balootRoundScript.BalootCard = allCards.First();
    //            balootRoundScript.IncrementStartIndex();
    //            balootRoundScript.ResetValues();

    //            allCards.RemoveAt(0);

    //            foreach (var item in allCards)
    //            {
    //                MyPlayer.AddCard(item);
    //            }
    //            DealCards();
    //            break;
    //        case dealFinishCode:
    //            DealCardsThenStartGame();
    //            break;
    //        case checkProjectsCode:
    //            MainPlayerBaloot player = (MainPlayerBaloot)MyPlayer;
    //            player.ChooseProjects(balootRoundScript.RoundType);

    //            if (player.PlayerProjects.Count > 0)
    //            {
    //                //send my projects
    //                multiPlayer.RaiseEventToMaster(projectCode, Utils.SerializeProjects(player.PlayerProjects,
    //                    player.ProjectPower, player.ProjectScore));
    //            }
    //            else
    //            {
    //                multiPlayer.RaiseEventToMaster(projectCode, null);
    //            }
    //            break;
    //        case projectCode:
    //            if (photonEvent.CustomData != null)
    //            {
    //                int power;
    //                int score;
    //                Dictionary<List<Card>, Projects> projects = Utils.DeserializeProjects((int[])photonEvent.CustomData, out power, out score);
    //                int index = multiPlayer.LookUpActors.First(x => x.Value == photonEvent.Sender).Key;
    //                ((PlayerBaloot)Players[index]).SetProjects(projects, power, score);
    //            }
    //            projectsCount++;
    //            if (projectsCount == 4)
    //                CompareProjects();
    //            break;
    //        case allProjectCode:
    //            Dictionary<int, Dictionary<List<Card>, Projects>> allProjects = Utils.DeserializePlayersProjects((int[])photonEvent.CustomData);

    //            for (int i = 0; i < Players.Length; i++)
    //            {
    //                if (allProjects.ContainsKey(i))
    //                {
    //                    ((PlayerBaloot)Players[i]).SetProjects(allProjects[i]);
    //                }
    //                else
    //                {
    //                    ((PlayerBaloot)Players[i]).RemoveProjects();
    //                }
    //            }

    //            break;
    //    }
    //}


    private void Deal_OnEvent(int eventIndex)
    {
        EventType eventType = (EventType)eventIndex;
        switch (eventType)
        {
            case EventType.CardsDealt:

                foreach (var item in multiPlayer.LookUpActors)
                {
                    if (item.Key == 0)
                        continue;

                    RaiseEventOptions eventOptionsCards = new RaiseEventOptions { TargetActors = new int[] { item.Value } };
                    PhotonNetwork.RaiseEvent(cardsDealtCode, Utils.SerializeListOfCards(Players[item.Key].OwnedCards),
                        eventOptionsCards, SendOptions.SendReliable);
                }

                SetCard();
                break;
            case EventType.CardsPassed:
                SetCardsPassed();
                GetDoubleCards();
                break;
            case EventType.TrickFinished:
                SetTrickFinished(RoundScript.PlayingIndex);

                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                PhotonNetwork.RaiseEvent(trickFinishedCode, RoundScript.PlayingIndex, raiseEventOptions, SendOptions.SendReliable);
                break;
            case EventType.DoubleCardsFinished:
                Debug.Log("double card finished");

                if (PhotonNetwork.IsMasterClient)
                {
                    print("sup");
                    multiPlayer.StartGame(RoundScript.PlayingIndex);
                }
                //if (PhotonNetwork.IsMasterClient)
                //{
                //    RaiseEventOptions eventReadyOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                //    PhotonNetwork.RaiseEvent(gameReadyCode, RoundScript.PlayingIndex, eventReadyOptions, SendOptions.SendReliable);
                //}
                break;
            case EventType.DealFinished:
                passedCardsNo = 0;

                RaiseEventOptions eventOptionsDeal = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
                PhotonNetwork.RaiseEvent(roundFinishedCode, null, eventOptionsDeal, SendOptions.SendReliable);

                SetDealFinished(true);
                break;
        }
    }

    public void OnEvent(EventData photonEvent)
    {
        switch (photonEvent.Code)
        {
            case cardsDealtCode:
                passedCardsNo = 0;
                List<Card> cards = Utils.DeSerializeListOfCards((int[])photonEvent.CustomData);
                ((RoundScriptHearts)RoundScript).RoundInfo = new RoundInfo();
                MyPlayer.Reset();

                foreach (var item in cards)
                {
                    MyPlayer.AddCard(item);
                }

                SetCard();
                ((MainPlayer)MyPlayer).SelectPassCards();
                break;
            //case gameReadyCode:
            //    beginIndex = (int)photonEvent.CustomData;
            //    SetStartGame(true);

            //    if (PhotonNetwork.IsMasterClient)
            //        BeginTurn();
            //    break;
            case passCardsCode:
                List<Card> passedCards = Utils.DeSerializeListOfCards((int[])photonEvent.CustomData);

                if (PhotonNetwork.IsMasterClient)
                {
                    int senderIndex = multiPlayer.LookUpActors.First(x => x.Value == photonEvent.Sender).Key;
                    int recieverIndex = (senderIndex + 1) % 4;

                    if (Players[senderIndex].IsPlayer)
                    {
                        ((Player)Players[senderIndex]).PassCards(passedCards);
                    }

                    ((Player)Players[recieverIndex]).AddPassCards(passedCards);

                    InrementPassedCards();
                }
                else
                {
                    ((MainPlayer)MyPlayer).AddPassCards(passedCards);
                }
                break;
            case doubleCardCode:
                //look into double card index
                int playerIndex;// = lookUpActors.First(x => x.Value == photonEvent.Sender).Key;
                KeyValuePair<bool, Card> cardValue = Utils.DeSerializeCardvalueAndIndex((int[])photonEvent.CustomData, out playerIndex);
                ((RoundScriptHearts)RoundScript).DoubleCard(cardValue.Value, cardValue.Key);
                SetCardDoubled(cardValue.Value, cardValue.Key, playerIndex);
                break;
            case checkDoubleCode:
                //make sure people recieved passed cards first
                SetCardsPassed();
                ((MainPlayer)MyPlayer).CheckForDoubleCards();
                break;
        }
    }

    //public override void SetStartGame(bool isMulti)
    //{
    //    base.SetStartGame(isMulti);
    //    OnStartPlaying?.Invoke(isMulti);
    //}

    private void GetDoubleCards()
    {
        for (int i = 0; i < Players.Length; i++)
        {
            if (Players[i].IsPlayer)
            {
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { TargetActors = new int[] { multiPlayer.LookUpActors[i] } };
                PhotonNetwork.RaiseEvent(checkDoubleCode, null, raiseEventOptions, SendOptions.SendReliable);
            }
            else
            {
                ((Player)Players[i]).CheckForDoubleCards();
            }
        }
    }

    async void SetCard()
    {
        await SetCardsReady();
    }

    private void InrementPassedCards()
    {
        passedCardsNo++;

        if (passedCardsNo == 4)
        {
            ((RoundScriptHearts)RoundScript).PassingCardsDone();
        }
    }

    private void GameScript_OnPassCardsReady(int playerIndex, List<Card> cards)
    {
        int recieverIndex = (playerIndex + 1) % 4;

        if (Players[playerIndex].IsPlayer)
        {
            return;
        }

        if (!PhotonNetwork.IsMasterClient)
        {
            List<int> targetActors = new List<int>();
            targetActors.Add(1);

            if (multiPlayer.LookUpActors.ContainsKey(recieverIndex))
                targetActors.Add(multiPlayer.LookUpActors[recieverIndex]);

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { TargetActors = targetActors.ToArray() };
            PhotonNetwork.RaiseEvent(passCardsCode, Utils.SerializeListOfCards(cards), raiseEventOptions, SendOptions.SendReliable);

            return;
        }

        if (Players[recieverIndex].IsPlayer)
        {
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { TargetActors = new int[] { multiPlayer.LookUpActors[recieverIndex] } };
            PhotonNetwork.RaiseEvent(passCardsCode, Utils.SerializeListOfCards(cards), raiseEventOptions, SendOptions.SendReliable);
        }
        else
        {
            ((Player)Players[recieverIndex]).AddPassCards(cards);
        }

        InrementPassedCards();
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void SendMessage(object message)
    {
        multiPlayer.SendMessageToOthers(message);
    }
}

public interface ILeaveRoom
{
    public void LeaveRoom();
}

public interface ISendMessage
{
    public void SendMessage(object message);
}
