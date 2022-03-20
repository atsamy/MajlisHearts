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

    const int cardsdealtCode = 40;
    const int trickFinishedCode = 41;
    const int passCardsCode = 42;
    const int gameReadyCode = 43;
    const int dealFinishedCode = 44;
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

        ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
        hash.Add("LoadedGame",1);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

        if (PhotonNetwork.IsMasterClient)
            StartCoroutine(WaitForOthers());
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

    private void Deal_OnEvent(EventType eventType)
    {

        switch (eventType)
        {
            case EventType.CardsDealt:
                for (int i = 1; i < playerNumbers; i++)
                {
                    RaiseEventOptions eventOptionsCards = new RaiseEventOptions { TargetActors = new int[] { i + 1 } };
                    PhotonNetwork.RaiseEvent(cardsdealtCode, Utils.SerializeListOfCards(Players[i].OwnedCards), eventOptionsCards, SendOptions.SendReliable);
                }

                SetCardsReady();
                break;
            case EventType.CardsPassed:
                break;
            case EventType.TrickFinished:
                SetTrickFinished(Deal.PlayingIndex);

                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                PhotonNetwork.RaiseEvent(trickFinishedCode, Deal.PlayingIndex, raiseEventOptions, SendOptions.SendReliable);
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
            case cardsdealtCode:
                List<Card> cards = Utils.DeSerializeListOfCards((int[])photonEvent.CustomData);
                
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

                    if (Players[senderIndex].IsPlayer)
                    {
                        Players[senderIndex].PassCards(passedCards);
                    }

                    Players[recieverIndex].AddPassCards(passedCards);
                }
                else
                {
                    myPlayer.AddPassCards(passedCards);
                }
                break;
            case gameReadyCode:
                beginIndex = (int)photonEvent.CustomData;

                BeginTurn();
                SetStartPlaying();
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
            default:
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

    private void InrementPassedCards()
    {
        passedCardsNo++;

        if (passedCardsNo == 4)
        {
            Deal.PassingCardsDone();

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent(gameReadyCode, Deal.PlayingIndex, raiseEventOptions, SendOptions.SendReliable);
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

        if (Players[playerIndex].IsPlayer)
        {
            InrementPassedCards();
            return;
        }
        if (!PhotonNetwork.IsMasterClient)
        {
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { TargetActors = new int[] { 1 } };
            PhotonNetwork.RaiseEvent(passCardsCode, Utils.SerializeListOfCards(cards), raiseEventOptions, SendOptions.SendReliable);

            InrementPassedCards();
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

}
