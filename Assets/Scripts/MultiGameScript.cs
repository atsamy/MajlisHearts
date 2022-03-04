using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiGameScript : GameScript, IPunTurnManagerCallbacks, IOnEventCallback
{
    public float TurnDuration = 10;

    PunTurnManager turnManager;
    int passedCardsNo;
    int playerNumbers;
    MainPlayer myPlayer;

    int lastIndex;
    int nextIndex;
    int beginIndex;
    int handIndex = 0;
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

            //Players[i].Name = "Player " + (i + 1);
        }

        Deal.SetPlayers(Players);

        Deal.OnEvent += Deal_OnEvent;

        if (PhotonNetwork.IsMasterClient)
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

    private void Deal_OnEvent(EventType eventType)
    {
        switch (eventType)
        {
            case EventType.CardsDealt:
                Deal_OnCardsDealt();
                break;
            case EventType.CardsPassed:
                break;
            case EventType.TrickFinished:
                DealTrickFinished(Deal.PlayingIndex);
                break;
            case EventType.DealFinished:
                Deal_OnDealFinished();
                break;
        }
    }

    private void DealTrickFinished(int winningHand)
    {
        SetTrickFinished(winningHand);

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(2, winningHand, raiseEventOptions, SendOptions.SendReliable);
    }

    private void Deal_OnCardsDealt()
    {

        for (int i = 1; i < playerNumbers; i++)
        {
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { TargetActors = new int[] { i + 1 } };
            PhotonNetwork.RaiseEvent(0, Utils.SerializeListOfCards(Players[i].OwnedCards), raiseEventOptions, SendOptions.SendReliable);
        }

        SetCardsReady();
    }

    public void OnEvent(EventData photonEvent)
    {
        //UIManager.Instance.Debug("event code: " + photonEvent.Code);

        if (photonEvent.Code == 0)
        {
            List<Card> cards = Utils.DeSerializeListOfCards((int[])photonEvent.CustomData);

            foreach (var item in cards)
            {
                myPlayer.AddCard(item);
            }

            SetCardsReady();

            myPlayer.SelectPassCards();
        }
        else if (photonEvent.Code == 1)
        {
            List<Card> passedCards = Utils.DeSerializeListOfCards((int[])photonEvent.CustomData);

            if (PhotonNetwork.IsMasterClient)
            {
                int recieverIndex = photonEvent.Sender % 4;
                int senderIndex = photonEvent.Sender - 1;

                if (Players[senderIndex].IsPlayer)
                {
                    Players[senderIndex].PassCards(passedCards);
                }

                Players[recieverIndex].AddPassCards(passedCards);

                //InrementPassedCards();
            }
            else
            {
                myPlayer.AddPassCards(passedCards);
            }
        }
        else if (photonEvent.Code == 2)
        {
            beginIndex = (int)photonEvent.CustomData;

            if (PhotonNetwork.IsMasterClient)
            {
                if (Deal.DealInfo.roundNumber < 13)
                    turnManager.BeginTurn();
            }
            else
            {
                SetTrickFinished(beginIndex);
            }
        }
        else if (photonEvent.Code == 3)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                SetDealFinished();
            }
        }
    }

    private void InrementPassedCards()
    {
        passedCardsNo++;

        if (passedCardsNo == 4)
        {
            Deal.PassingCardsDone();

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent(2, Deal.PlayingIndex, raiseEventOptions, SendOptions.SendReliable);
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
        handIndex++;

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
                myPlayer.SetTurn(Deal.DealInfo, handIndex);
            }
        }
    }

    public void OnTurnBegins(int turn)
    {
        if (turn == 1)
        {
            SetStartPlaying();
        }

        if (!PhotonNetwork.IsMasterClient)
        {
            UIManager.Instance.Debug("begin " + beginIndex + " my " + MainPlayerIndex);

            if (beginIndex == MainPlayerIndex)
            {
                myPlayer.SetTurn(Deal.DealInfo, 0);
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

        InrementPassedCards();

        if (Players[playerIndex].IsPlayer)
        {
            return;
        }
        if (!PhotonNetwork.IsMasterClient)
        {
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { TargetActors = new int[] { 1 } };
            PhotonNetwork.RaiseEvent(1, Utils.SerializeListOfCards(cards), raiseEventOptions, SendOptions.SendReliable);
            return;
        }

        if (Players[recieverIndex].IsPlayer)
        {
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { TargetActors = new int[] { recieverIndex + 1 } };
            PhotonNetwork.RaiseEvent(1, Utils.SerializeListOfCards(cards), raiseEventOptions, SendOptions.SendReliable);
        }
        else
        {
            Players[recieverIndex].AddPassCards(cards);
        }
    }

    private void Deal_OnDealFinished()
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent(3, null, raiseEventOptions, SendOptions.SendReliable);

        SetDealFinished();
    }
}
