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
    // Start is called before the first frame update
    void Start()
    {
        turnManager = gameObject.AddComponent<PunTurnManager>();
        turnManager.TurnManagerListener = this;

        Deal.OnDealFinished += Deal_OnDealFinished;

        passedCardsNo = 0;

        Players = new Player[4];

        int playerNumbers = PhotonNetwork.PlayerList.Length;

        for (int i = 0; i < 4; i++)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (i == 0)
                    Players[i] = new MainPlayer(i);
                else if (playerNumbers > i)
                    Players[i] = new Player(i);
                else
                    Players[i] = new AIPlayer(i);
            }
            else
            {
                if (i < PhotonNetwork.PlayerList.Length && PhotonNetwork.PlayerList[i].IsLocal)
                {
                    Players[i] = new MainPlayer(i);
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

        if (PhotonNetwork.IsMasterClient)
            StartGame();

        UIManager.Instance.Debug("player: " + PhotonNetwork.LocalPlayer.ActorNumber);
    }

    private void Deal_OnCardsDealt(bool waitPass)
    {
        //fix
        //send to all players in game

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { TargetActors = new int[] { 2 } };

        PhotonNetwork.RaiseEvent(0, Utils.SerializeListOfCards(Players[1].OwnedCards), raiseEventOptions, SendOptions.SendReliable);

        SetCardsReady();
    }

    public void OnEvent(EventData photonEvent)
    {
        UIManager.Instance.Debug("event code: " + photonEvent.Code);

        if (photonEvent.Code == 0)
        {
            Players[MainPlayerIndex].OwnedCards = Utils.DeSerializeListOfCards((int[])photonEvent.CustomData);
            SetCardsReady();

            Players[MainPlayerIndex].SelectPassCards();
        }
        else if (photonEvent.Code == 1)
        {
            KeyValuePair<int, List<Card>> passedCards = (KeyValuePair<int, List<Card>>)photonEvent.CustomData;

            int passIndex = passedCards.Key;
            passIndex++;
            passIndex %= 4;

            if (passIndex == MainPlayerIndex)
            {
                Players[MainPlayerIndex].AddPassCards(passedCards.Value);
            }

            if (PhotonNetwork.IsMasterClient)
            {
                passedCardsNo++;

                if (Players[passIndex] is AIPlayer)
                {
                    Players[passIndex].AddPassCards(passedCards.Value);
                }

                if (passedCardsNo == 4)
                {
                    Deal.PassingCardsDone();

                    turnManager.BeginTurn();
                }
            }
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

    public void OnPlayerFinished(Photon.Realtime.Player player, int turn, object move)
    {
        KeyValuePair<int, Card> hand = (KeyValuePair<int, Card>)move;
        Players[hand.Key].ChooseCard(hand.Value);
    }

    public void OnPlayerMove(Photon.Realtime.Player player, int turn, object move)
    {

    }

    public void OnTurnBegins(int turn)
    {
        if (turn == 0 && !PhotonNetwork.IsMasterClient)
        {
            if (Players[MainPlayerIndex].HasTwoClub())
            {
                Players[MainPlayerIndex].SetTurn(Deal.DealInfo,0);
            }
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
        if (playerIndex == MainPlayerIndex)
            turnManager.SendMove(card, true);

        if (PhotonNetwork.IsMasterClient)
        {
            if (Players[playerIndex] is AIPlayer)
            {
                turnManager.SendMove(new KeyValuePair<int, Card>(playerIndex, card), true);
            }
        }
        //turnManager.BeginTurn();

        Deal.GameScript_OnCardReady(playerIndex, card);
    }

    private void GameScript_OnPassCardsReady(int playerIndex, List<Card> cards)
    {
        //fix
        //if (PhotonNetwork.IsMasterClient)
        //{
        //Deal.GameScript_OnPassCardsReady(playerIndex, cards);
        //}
        //else
        //{
        //only send to real players
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { TargetActors = new int[] { playerIndex + 1 } };
        PhotonNetwork.RaiseEvent(1, Utils.SerializeListOfCards(cards), raiseEventOptions, SendOptions.SendReliable);
        //}
        //turnManager.SendMessage();
    }

    private void Deal_OnDealFinished()
    {
        //turnManager
    }
}
