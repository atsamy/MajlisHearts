using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System;
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
    //int handIndex = 0;

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
    // Start is called before the first frame update
    void Start()
    {
        turnManager = gameObject.AddComponent<PunTurnManager>();
        turnManager.TurnManagerListener = this;
        turnManager.TurnDuration = TurnDuration;

        passedCardsNo = 0;
        Players = new Player[4];
        playerNumbers = PhotonNetwork.PlayerList.Length;

        for (int i = 0; i < 4; i++)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (i == 0)
                {
                    myPlayer = new MainPlayer(i);
                    Players[i] = myPlayer;
                    Players[i].Name = PhotonNetwork.PlayerList[i].NickName;
                }
                else if (playerNumbers > i)
                {
                    Players[i] = new Player(i);
                    Players[i].Name = PhotonNetwork.PlayerList[i].NickName;
                }
                else
                {
                    Players[i] = new AIPlayer(i);
                    Players[i].Name = "AI " + i;
                }
            }
            else
            {
                if (i < PhotonNetwork.PlayerList.Length && PhotonNetwork.PlayerList[i].IsLocal)
                {
                    myPlayer = new MainPlayer(i);
                    Players[i] = myPlayer;
                    MainPlayerIndex = i;
                }
                else
                {
                    Players[i] = new Player(i);
                }

                if (i < PhotonNetwork.PlayerList.Length)
                {
                    Players[i].Name = PhotonNetwork.PlayerList[i].NickName;
                }
                else
                {
                    Players[i].Name = "AI " + i;
                }
            }

            Players[i].OnPassCardsReady += GameScript_OnPassCardsReady;
            Players[i].OnCardReady += GameScript_OnCardReady;
            Players[i].OnDoubleCard += GameScript_OnDoubleCard;

             
            //Players[i].Name = "Player " + (i + 1);
        }

        myPlayer.OnPlayerTurn += MainPlayerTurn;

        Deal.SetPlayers(Players);

        Deal.OnEvent += Deal_OnEvent;

        ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
        hash.Add("LoadedGame",1);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

        if (PhotonNetwork.IsMasterClient)
            StartCoroutine(WaitForOthers());
    }

    private void MainPlayerTurn(DealInfo info)
    {
        playerTimer = StartCoroutine(StartTimer());
    }

    private void GameScript_OnDoubleCard(Card card, bool value,int index)
    {
        //Debug.Log("multi player " + index + " " + card.ToString());

        RaiseEventOptions eventOptionsCards = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(doubleCardCode, Utils.SerializeCardValueAndIndex(card, value,index), eventOptionsCards, SendOptions.SendReliable);

        //if (PhotonNetwork.IsMasterClient)
        //{
        //    Deal.DoubleCard(card, value);
        //}
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
        RaiseEventOptions eventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others};
        PhotonNetwork.RaiseEvent(messageCode, message, eventOptions, SendOptions.SendReliable);
    }

    private void Deal_OnEvent(EventType eventType)
    {
        switch (eventType)
        {
            case EventType.CardsDealt:
                for (int i = 1; i < playerNumbers; i++)
                {
                    RaiseEventOptions eventOptionsCards = new RaiseEventOptions { TargetActors = new int[] { i + 1 } };
                    PhotonNetwork.RaiseEvent(cardsDealtCode, Utils.SerializeListOfCards(Players[i].OwnedCards), eventOptionsCards, SendOptions.SendReliable);
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
                    int recieverIndex = photonEvent.Sender % 4;
                    int senderIndex = photonEvent.Sender - 1;

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

                if(PhotonNetwork.IsMasterClient)
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
                KeyValuePair<bool,Card> cardValue = Utils.DeSerializeCardvalueAndIndex((int[])photonEvent.CustomData,out playerIndex);
                Deal.DoubleCard(cardValue.Value, cardValue.Key);

                SetCardDoubled(cardValue.Value, cardValue.Key, playerIndex);
                break;
            case checkDoubleCode:
                SetCardsPassed();
                myPlayer.CheckForDoubleCards();
                break;
            case messageCode:
                OnMessageRecieved?.Invoke(photonEvent.Sender -1,photonEvent.CustomData);
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
                //if (Players[i].HasCard(Card.QueenOfSpades) || Players[i].HasCard(Card.QueenOfSpades))
                //{
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { TargetActors = new int[] { i + 1 } };
                PhotonNetwork.RaiseEvent(checkDoubleCode, null, raiseEventOptions, SendOptions.SendReliable);
                //}
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
        //handIndex++;

        if (PhotonNetwork.IsMasterClient)
        {
            if (Players[hand.Key].IsPlayer)
                Players[hand.Key].ChooseCard(hand.Value);
        }
        else if (lastIndex != MainPlayerIndex)
        {
            Deal.UpdateDealInfo(lastIndex, hand.Value);

            Players[hand.Key].ShowCard(hand.Value);

            //UIManager.Instance.Debug(nextIndex + " " + MainPlayerIndex);

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
            //UIManager.Instance.Debug("begin " + beginIndex + " my " + MainPlayerIndex);

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
        //if (nextIndex == MainPlayerIndex)
        //{
        //    myPlayer.ForcePlay();
        //}
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
            //InrementPassedCards();
            return;
        }

        if (!PhotonNetwork.IsMasterClient)
        {
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { TargetActors = new int[] { 1, recieverIndex + 1 } };
            PhotonNetwork.RaiseEvent(passCardsCode, Utils.SerializeListOfCards(cards), raiseEventOptions, SendOptions.SendReliable);

            //InrementPassedCards();
            return;
        }

        if (Players[recieverIndex].IsPlayer)
        {
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { TargetActors = new int[] { recieverIndex + 1 } };
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
