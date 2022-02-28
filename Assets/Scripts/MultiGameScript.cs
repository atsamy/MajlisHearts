using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiGameScript : GameScript, IPunTurnManagerCallbacks
{

    PunTurnManager turnManager;

    // Start is called before the first frame update
    void Start()
    {
        turnManager = gameObject.AddComponent<PunTurnManager>();
        turnManager.TurnManagerListener = this;

        Deal.OnDealFinished += Deal_OnDealFinished;


        Players = new Player[4];

        int playerNumbers = PhotonNetwork.CountOfPlayersInRooms;

        //PhotonNetwork.LocalPlayer.
        //PhotonNetwork.PlayerList
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
                if (PhotonNetwork.PlayerList[i].IsLocal)
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
    }

    private void Deal_OnCardsDealt(bool waitPass)
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        Dictionary<int, List<Card>> playerCards = new Dictionary<int, List<Card>>();

        for (int i = 0; i < Players.Length; i++)
        {
            if (Players[i] is Player)
            {
                playerCards.Add(i,Players[i].OwnedCards);
            }
        }

        PhotonNetwork.RaiseEvent(0, playerCards, raiseEventOptions,SendOptions.SendReliable);
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == 0)
        {
            Players[MainPlayerIndex].OwnedCards = ((Dictionary<int, List<Card>>)photonEvent.CustomData)[MainPlayerIndex];

            //show UI
            //Deal.OnCardsDealt.
            // 
        }
        else if (photonEvent.Code == 1)
        {
            
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
        //if()
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

        if (PhotonNetwork.IsMasterClient)
        {
            Deal.GameScript_OnPassCardsReady(playerIndex, cards);
        }
        else
        {
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
            PhotonNetwork.RaiseEvent(1, cards, raiseEventOptions, SendOptions.SendReliable);
        }
        //turnManager.SendMessage();
    }

    private void Deal_OnDealFinished()
    {
        //turnManager
    }

    // Update is called once per frame
    void Update()
    {

    }
}
