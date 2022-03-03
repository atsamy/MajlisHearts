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

        Deal.OnDealFinished += Deal_OnDealFinished;

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
                }
                else if (playerNumbers > i)
                {
                    Players[i] = new Player(i);
                }
                else
                {
                    Players[i] = new AIPlayer(i);
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
            }

            Players[i].OnPassCardsReady += GameScript_OnPassCardsReady;
            Players[i].OnCardReady += GameScript_OnCardReady;

            Players[i].Name = "Player " + (i + 1);
        }

        Deal.SetPlayers(Players);
        Deal.OnCardsDealt += Deal_OnCardsDealt;
        Deal.OnTrickFinished += Deal_OnTrickFinished;
        //Deal.OnNextTurn += Deal_OnNextTurn;

        if (PhotonNetwork.IsMasterClient)
            StartGame();

        //UIManager.Instance.Debug("player: " + PhotonNetwork.LocalPlayer.ActorNumber);
    }

    //private void Deal_OnNextTurn()
    //{
    //    //throw new NotImplementedException();

    //    //turnManager.BeginTurn();
    //}

    private void Deal_OnTrickFinished(int winningHand)
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(2, winningHand, raiseEventOptions, SendOptions.SendReliable);
    }

    private void Deal_OnCardsDealt(bool waitPass)
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
            myPlayer.OwnedCards = Utils.DeSerializeListOfCards((int[])photonEvent.CustomData);
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
                turnManager.BeginTurn();
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

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void SendCards(List<Card> cards)
    {

    }

    public void OnPlayerMove(Photon.Realtime.Player player, int turn, object move)
    {
        PlayerMove(move);
    }

    public void OnPlayerFinished(Photon.Realtime.Player player, int turn, object move)
    {
        PlayerMove(move);
    }

    private void PlayerMove(object move)
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

            UIManager.Instance.Debug(nextIndex + " " + MainPlayerIndex);

            if (nextIndex == MainPlayerIndex)
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

            if (!PhotonNetwork.IsMasterClient)
            {
                Deal.DealInfo = new DealInfo();
            }
        }
        if (!PhotonNetwork.IsMasterClient)
        {
            if (beginIndex == MainPlayerIndex)
            {
                Deal.SetTurn();
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
        //turnManager
    }
}
