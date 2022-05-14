using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiGameScript : GameScript, IPunTurnManagerCallbacks, IOnEventCallback, IInRoomCallbacks
{
    public float TurnDuration = 40;

    PunTurnManager turnManager;
    int passedCardsNo;
    int playerNumbers;

    int lastIndex;
    int nextIndex;
    int beginIndex;

    const int cardsDealtCode = 40;
    const int trickFinishedCode = 41;
    const int passCardsCode = 42;
    const int gameReadyCode = 43;
    const int dealFinishedCode = 44;
    const int doubleCardCode = 45;
    const int checkDoubleCode = 46;
    const int messageCode = 47;

    public delegate void messageRecieved(int playerIndex, object message);
    public static messageRecieved OnMessageRecieved;

    Dictionary<int, int> lookUpActors;
    void Start()
    {
        turnManager = gameObject.AddComponent<PunTurnManager>();
        turnManager.TurnManagerListener = this;
        turnManager.TurnDuration = TurnDuration;

        passedCardsNo = 0;
        Players = new Player[4];
        playerNumbers = PhotonNetwork.PlayerList.Length;

        lookUpActors = new Dictionary<int, int>();
        Dictionary<int, string> lookUpAvatar = new Dictionary<int, string>();

        string[] playersOrder;

        if (GameManager.Instance.IsTeam && GameManager.Instance.GameType == GameType.Friends)
        {
            playersOrder = (string[])PhotonNetwork.CurrentRoom.CustomProperties["players"];

            for (int i = 0; i < playersOrder.Length; i++)
            {
                for (int j = 0; j < PhotonNetwork.PlayerList.Length; j++)
                {
                    if (playersOrder[i] == PhotonNetwork.PlayerList[j].NickName)
                    {
                        lookUpActors.Add(i, PhotonNetwork.PlayerList[j].ActorNumber);
                        lookUpAvatar.Add(i, PhotonNetwork.PlayerList[j].CustomProperties["avatar"].ToString());
                        break;
                    }
                }
            }
        }
        else
        {
            playersOrder = new string[4];

            for (int i = 0; i < playersOrder.Length; i++)
            {
                if (i < PhotonNetwork.PlayerList.Length)
                {
                    playersOrder[i] = PhotonNetwork.PlayerList[i].NickName;
                    lookUpActors.Add(i, PhotonNetwork.PlayerList[i].ActorNumber);
                    lookUpAvatar.Add(i, PhotonNetwork.PlayerList[i].CustomProperties["avatar"].ToString());
                }
                else
                {
                    playersOrder[i] = "AI " + i;
                    lookUpActors.Add(i, i + 1);
                }
            }
        }

        for (int i = 0; i < 4; i++)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (i == 0)
                {
                    myPlayer = new MainPlayer(i);
                    Players[i] = myPlayer;
                    Players[i].Name = playersOrder[i];
                    Players[i].Avatar = lookUpAvatar[i];
                }
                else if (lookUpAvatar.ContainsKey(i))
                {
                    Players[i] = new Player(i);
                    Players[i].Name = playersOrder[i];
                    Players[i].Avatar = lookUpAvatar[i];
                }
                else
                {
                    Players[i] = new AIPlayer(i);
                    Players[i].Name = "AI " + i;
                }
            }
            else
            {
                if (i < playersOrder.Length && playersOrder[i] == GameManager.Instance.MyPlayer.Name)
                {
                    myPlayer = new MainPlayer(i);
                    Players[i] = myPlayer;
                    MainPlayerIndex = i;
                    Players[i].Avatar = lookUpAvatar[i];
                }
                else
                {
                    Players[i] = new Player(i);
                }

                if (lookUpAvatar.ContainsKey(i))
                {
                    Players[i].Name = playersOrder[i];
                    Players[i].Avatar = lookUpAvatar[i];
                }
                else
                {
                    Players[i].Name = "AI" + i;
                }
            }

            Players[i].OnPassCardsReady += GameScript_OnPassCardsReady;
            Players[i].OnCardReady += GameScript_OnCardReady;
            Players[i].OnDoubleCard += GameScript_OnDoubleCard;
        }

        myPlayer.OnPlayerTurn += MainPlayerTurn;

        Deal.SetPlayers(Players);
        Deal.OnEvent += Deal_OnEvent;

        ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
        hash.Add("LoadedGame", 1);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

        if (PhotonNetwork.IsMasterClient)
            StartCoroutine(WaitForOthers());
    }

    private void MainPlayerTurn(DealInfo info)
    {
        playerTimer = StartCoroutine(StartTimer());
    }

    private void GameScript_OnDoubleCard(Card card, bool value, int index)
    {
        RaiseEventOptions eventOptionsCards = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(doubleCardCode, Utils.SerializeCardValueAndIndex(card, value, index),
            eventOptionsCards, SendOptions.SendReliable);
    }

    IEnumerator WaitForOthers()
    {
        bool waitForPlayers = true;

        while (waitForPlayers)
        {
            waitForPlayers = false;

            foreach (var item in PhotonNetwork.PlayerList)
            {
                if (!item.CustomProperties.ContainsKey("LoadedGame"))
                {
                    waitForPlayers = true;
                    break;
                }
            }

            yield return null;
        }

        StartGame();
    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    internal void SendMessageToOthers(object message)
    {
        RaiseEventOptions eventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent(messageCode, message, eventOptions, SendOptions.SendReliable);
    }

    private void Deal_OnEvent(EventType eventType)
    {
        switch (eventType)
        {
            case EventType.CardsDealt:

                foreach (var item in lookUpActors)
                {
                    if (item.Key == 0)
                        continue;

                    RaiseEventOptions eventOptionsCards = new RaiseEventOptions { TargetActors = new int[] { item.Value } };
                    PhotonNetwork.RaiseEvent(cardsDealtCode, Utils.SerializeListOfCards(Players[item.Key].OwnedCards),
                        eventOptionsCards, SendOptions.SendReliable);
                }

                SetCardsReady();
                break;
            case EventType.CardsPassed:
                SetCardsPassed();
                GetDoubleCards();
                break;
            case EventType.TrickFinished:
                SetTrickFinished(Deal.PlayingIndex);

                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                PhotonNetwork.RaiseEvent(trickFinishedCode, Deal.PlayingIndex, raiseEventOptions, SendOptions.SendReliable);
                break;
            case EventType.DoubleCardsFinished:
                Debug.Log("double card finished");

                if (PhotonNetwork.IsMasterClient)
                {
                    RaiseEventOptions eventReadyOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                    PhotonNetwork.RaiseEvent(gameReadyCode, Deal.PlayingIndex, eventReadyOptions, SendOptions.SendReliable);
                }
                break;
            case EventType.DealFinished:
                passedCardsNo = 0;

                RaiseEventOptions eventOptionsDeal = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
                PhotonNetwork.RaiseEvent(dealFinishedCode, null, eventOptionsDeal, SendOptions.SendReliable);

                SetDealFinished(true);
                break;
        }
    }

    public void OnEvent(EventData photonEvent)
    {
        switch (photonEvent.Code)
        {
            case cardsDealtCode:
                List<Card> cards = Utils.DeSerializeListOfCards((int[])photonEvent.CustomData);
                Deal.DealInfo = new DealInfo();
                myPlayer.Reset();

                foreach (var item in cards)
                {
                    myPlayer.AddCard(item);
                }

                SetCardsReady();
                myPlayer.SelectPassCards();
                break;
            case passCardsCode:
                List<Card> passedCards = Utils.DeSerializeListOfCards((int[])photonEvent.CustomData);

                if (PhotonNetwork.IsMasterClient)
                {
                    int senderIndex = lookUpActors.First(x => x.Value == photonEvent.Sender).Key;

                    int recieverIndex = (senderIndex + 1) % 4;

                    print("reciever: " + recieverIndex + " senderIndex " + senderIndex);

                    if (Players[senderIndex].IsPlayer)
                    {
                        Players[senderIndex].PassCards(passedCards);
                    }

                    Players[recieverIndex].AddPassCards(passedCards);

                    InrementPassedCards();
                }
                else
                {
                    myPlayer.AddPassCards(passedCards);
                }
                break;
            case gameReadyCode:
                beginIndex = (int)photonEvent.CustomData;

                SetStartGame(true);

                if (PhotonNetwork.IsMasterClient)
                    BeginTurn();

                break;
            case trickFinishedCode:
                beginIndex = (int)photonEvent.CustomData;
                BeginTurn();
                SetTrickFinished(beginIndex);
                break;
            case dealFinishedCode:
                if (!PhotonNetwork.IsMasterClient)
                {
                    passedCardsNo = 0;
                    SetDealFinished(false);
                }
                break;
            case doubleCardCode:
                int playerIndex;
                KeyValuePair<bool, Card> cardValue = Utils.DeSerializeCardvalueAndIndex((int[])photonEvent.CustomData, out playerIndex);
                Deal.DoubleCard(cardValue.Value, cardValue.Key);

                SetCardDoubled(cardValue.Value, cardValue.Key, playerIndex);
                break;
            case checkDoubleCode:
                SetCardsPassed();
                myPlayer.CheckForDoubleCards();
                break;
            case messageCode:
                int index = lookUpActors.First(x => x.Value == photonEvent.Sender).Key;
                OnMessageRecieved?.Invoke(index, photonEvent.CustomData);
                break;
        }
    }

    private void BeginTurn()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (Deal.DealInfo.roundNumber < 13)
                turnManager.BeginTurn();
        }
    }

    private void GetDoubleCards()
    {
        for (int i = 0; i < Players.Length; i++)
        {
            if (Players[i].IsPlayer)
            {
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { TargetActors = new int[] { lookUpActors[i] } };
                PhotonNetwork.RaiseEvent(checkDoubleCode, null, raiseEventOptions, SendOptions.SendReliable);
            }
            else
            {
                Players[i].CheckForDoubleCards();
            }
        }
    }

    private void InrementPassedCards()
    {
        passedCardsNo++;

        if (passedCardsNo == 4)
        {
            Deal.PassingCardsDone();
        }
    }

    public void OnPlayerMove(Photon.Realtime.Player player, int turn, object move)
    {
        PlayerMove(move, false);
    }

    public void OnPlayerFinished(Photon.Realtime.Player player, int turn, object move)
    {
        PlayerMove(move, true);
    }

    private void PlayerMove(object move, bool finished)
    {
        KeyValuePair<int, Card> hand = Utils.DeSerializeCardAndPlayer((int[])move);

        lastIndex = hand.Key;
        nextIndex = (lastIndex + 1) % 4;

        if (PhotonNetwork.IsMasterClient)
        {
            if (Players[hand.Key].IsPlayer)
                Players[hand.Key].ChooseCard(hand.Value);
        }
        else if (lastIndex != MainPlayerIndex)
        {
            Deal.UpdateDealInfo(lastIndex, hand.Value);
            Players[hand.Key].ShowCard(hand.Value);

            if (nextIndex == MainPlayerIndex && !finished)
            {
                myPlayer.SetTurn(Deal.DealInfo);
            }
        }
    }

    public void OnTurnBegins(int turn)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            if (beginIndex == MainPlayerIndex)
            {
                myPlayer.SetTurn(Deal.DealInfo);
            }
        }
        else if (!Players[beginIndex].IsPlayer)
        {
            Deal.SetTurn();
        }
    }

    public void OnTurnCompleted(int turn)
    {

    }

    public void OnTurnTimeEnds(int turn)
    {

    }

    private void GameScript_OnCardReady(int playerIndex, Card card)
    {
        int finishIndex = beginIndex - 1;
        finishIndex = (finishIndex < 0 ? 3 : finishIndex);

        if (playerIndex == MainPlayerIndex)
            StopCoroutine(playerTimer);

        if (PhotonNetwork.IsMasterClient)
        {
            if (!Players[playerIndex].IsPlayer)
                turnManager.SendMove(Utils.SerializeCardAndPlayer(card, playerIndex), playerIndex == finishIndex);

            Deal.GameScript_OnCardReady(playerIndex, card);
        }
        else if (playerIndex == MainPlayerIndex)
        {
            turnManager.SendMove(Utils.SerializeCardAndPlayer(card, playerIndex), playerIndex == finishIndex);
            Deal.UpdateDealInfo(playerIndex, card);
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

            if (lookUpActors.ContainsKey(recieverIndex))
                targetActors.Add(lookUpActors[recieverIndex]);

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { TargetActors = targetActors.ToArray() };
            PhotonNetwork.RaiseEvent(passCardsCode, Utils.SerializeListOfCards(cards), raiseEventOptions, SendOptions.SendReliable);

            return;
        }

        if (Players[recieverIndex].IsPlayer)
        {
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { TargetActors = new int[] { lookUpActors[recieverIndex] } };
            PhotonNetwork.RaiseEvent(passCardsCode, Utils.SerializeListOfCards(cards), raiseEventOptions, SendOptions.SendReliable);
        }
        else
        {
            Players[recieverIndex].AddPassCards(cards);
        }

        InrementPassedCards();
    }

    public void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {

    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {

        if (PhotonNetwork.IsMasterClient)
        {
            int index = otherPlayer.ActorNumber - 1;

            Player oldPlayer = Players[index];

            AIPlayer aiPlayer = new AIPlayer(index);
            Players[index] = aiPlayer;
            aiPlayer.MergeFromPlayer(oldPlayer);

            if (nextIndex == index)
            {
                aiPlayer.SetTurn(Deal.DealInfo);
            }
        }
    }

    public void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {

    }

    public void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {

    }

    public void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {

    }
}
