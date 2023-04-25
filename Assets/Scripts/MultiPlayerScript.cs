using System.Collections.Generic;
using Photon.Pun.UtilityScripts;
using ExitGames.Client.Photon;
using System.Threading.Tasks;
using Photon.Realtime;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using GooglePlayGames.BasicApi;
using System;

public class MultiPlayerScript : IPunTurnManagerCallbacks, IOnEventCallback, IInRoomCallbacks
{
    float turnDuration = 40;
    PunTurnManager turnManager;

    public Dictionary<int, int> LookUpActors;

    GameScriptBase gameScript;

    public delegate void messageRecieved(int playerIndex, object message);
    public messageRecieved OnMessageRecieved;

    public delegate void NetworkEvent(EventData photonEvent);
    public event NetworkEvent OnNetworkEvent;

    PlayerBase[] Players => gameScript.Players;

    const int trickFinishedCode = 41;
    const int setGameReadyCode = 42;
    const int startGameCode = 43;
    const int roundFinishedCode = 44;
    const int messageCode = 47;
    const int playerTurnCode = 49;

    int turnNumbers;

    int lastIndex;
    int nextIndex;
    int beginIndex;

    public MultiPlayerScript(PunTurnManager turnManager, GameScriptBase gameScript,int turns)
    {
        PhotonNetwork.AddCallbackTarget(this);


        this.turnManager = turnManager;
        this.gameScript = gameScript;
        turnNumbers = turns;
        //turnManager = gameObject.AddComponent<PunTurnManager>();
        turnManager.TurnManagerListener = this;
        turnManager.TurnDuration = turnDuration;

        gameScript.Players = new PlayerBase[4];
        //playerNumbers = PhotonNetwork.PlayerList.Length;

        LookUpActors = new Dictionary<int, int>();

        //Dictionary<int, string> lookUpAvatar = new Dictionary<int, string>();

        string[] playersOrder;

        if (GameManager.Instance.IsTeam && GameManager.Instance.GameType == GameType.Friends || GameManager.Instance.GameType == GameType.Online)
        {
            playersOrder = (string[])PhotonNetwork.CurrentRoom.CustomProperties["players"];

            for (int i = 0; i < playersOrder.Length; i++)
            {
                for (int j = 0; j < PhotonNetwork.PlayerList.Length; j++)
                {
                    if (playersOrder[i] == PhotonNetwork.PlayerList[j].NickName)
                    {
                        LookUpActors.Add(i, PhotonNetwork.PlayerList[j].ActorNumber);
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
                    LookUpActors.Add(i, PhotonNetwork.PlayerList[i].ActorNumber);
                }
                else
                {
                    playersOrder[i] = "AI";
                }
            }
        }

        for (int i = 0; i < 4; i++)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (i == 0)
                {
                    Players[i] = gameScript.CreateMainPlayer(0);
                    Players[i].Name = playersOrder[i];
                    Players[i].Avatar = AvatarManager.Instance.playerAvatar;//lookUpAvatar[i];
                }
                else if (LookUpActors.ContainsKey(i))
                {
                    Players[i] = gameScript.CreatePlayer(i);
                    Players[i].Name = playersOrder[i];
                    Players[i].Avatar = AvatarManager.Instance.GetPlayerAvatar(playersOrder[i]);
                }
                else
                {
                    Players[i] = gameScript.CreateAIPlayer(i);
                    Players[i].Name = playersOrder[i];
                    Players[i].Avatar = AvatarManager.Instance.GetPlayerAvatar(playersOrder[i]);
                }
            }
            else
            {
                if (i < playersOrder.Length && playersOrder[i] == GameManager.Instance.MyPlayer.Name)
                {
                    Players[i] = gameScript.CreateMainPlayer(i);
                    Players[i].Avatar = AvatarManager.Instance.playerAvatar;
                    Players[i].Name = playersOrder[i];
                }
                else
                {
                    Players[i] = gameScript.CreatePlayer(i);
                    Players[i].Avatar = AvatarManager.Instance.GetPlayerAvatar(playersOrder[i]);
                    Players[i].Name = playersOrder[i];
                }
            }

            Players[i].OnCardReady += GameScript_OnCardReady;
            Players[i].OnPlayerTurn += GameScript_OnPlayerTurn;
        }

        //myPlayer.OnPlayerTurn += MainPlayerTurn;

        //((RoundScriptHeats)RoundScript).SetPlayers(Players);
        //gameScript.RoundScript.OnEvent += Deal_OnEvent;

        Hashtable hash = new Hashtable
        {
            { "LoadedGame", 1 }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

        if (PhotonNetwork.IsMasterClient)
            WaitForOthers();

        if (GameManager.Instance.GameType == GameType.Friends)
        {
            gameScript.SetEnvironment(PhotonNetwork.CurrentRoom.CustomProperties["TableTop"].ToString(),
                PhotonNetwork.CurrentRoom.CustomProperties["CardBack"].ToString());
        }
        else
        {
            gameScript.SetEnvironment(GameManager.Instance.EquippedItem["TableTop"], GameManager.Instance.EquippedItem["CardBack"]);
        }
    }

    async void WaitForOthers()
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

            await Task.Yield();
            //yield return null;
        }
        //bug here only called by master client
        //RaiseEventToOthers(setGameReadyCode, null);
        //gameScript.SetGameReady();
        //gameScript.StartGame();

        RaiseEventToAll(setGameReadyCode, null);
    }

    public void StartGame(int beginIndex)
    {
        RaiseEventToOthers(startGameCode, beginIndex);

        BeginTurn(beginIndex);
        gameScript.SetPlaying(true);
    }

    void GameScript_OnPlayerTurn(int index, RoundInfo info)
    {
        if (index == gameScript.MainPlayerIndex)
        {
            gameScript.StartTimer();
        }
        if (PhotonNetwork.IsMasterClient)
        {
            RaiseEventOptions eventOptionsCards = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
            PhotonNetwork.RaiseEvent(playerTurnCode, index, eventOptionsCards, SendOptions.SendReliable);
        }
    }

    public void RaiseEventToOthers(byte eventCode, object data)
    {
        RaiseEventOptions eventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent(eventCode, data, eventOptions, SendOptions.SendReliable);
    }

    public void RaiseEventToMaster(byte eventCode, object data)
    {
        RaiseEventOptions eventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };
        PhotonNetwork.RaiseEvent(eventCode, data, eventOptions, SendOptions.SendReliable);
    }

    public void RaiseEventToAll(byte eventCode, object data)
    {
        RaiseEventOptions eventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(eventCode, data, eventOptions, SendOptions.SendReliable);
    }

    public void BeginTurn(int index)
    {
        //Debug.Log("begin Index:" + index);
        beginIndex = index;
        if (PhotonNetwork.IsMasterClient)
        {
            //Debug.Log(gameScript.RoundScript.RoundInfo.TrickNumber + " " + turnNumbers);
            if (gameScript.RoundScript.RoundInfo.TrickNumber < turnNumbers)
                turnManager.BeginTurn();
        }
    }

    protected void PlayerMove(object move, bool finished)
    {
        KeyValuePair<int, Card> hand = Utils.DeSerializeCardAndPlayer((int[])move);

        lastIndex = hand.Key;
        nextIndex = (lastIndex + 1) % 4;

        if (PhotonNetwork.IsMasterClient)
        {
            if (Players[hand.Key].IsPlayer)
                Players[hand.Key].ChooseCard(hand.Value);
        }
        else if (lastIndex != gameScript.MainPlayerIndex)
        {
            gameScript.RoundScript.UpdateDealInfo(lastIndex, hand.Value);
            //Debug.Log("PlayerMove:" + hand.Value);
            Players[hand.Key].ShowCard(hand.Value);

            if (nextIndex == gameScript.MainPlayerIndex && !finished)
            {
                gameScript.MyPlayer.SetTurn(gameScript.RoundScript.RoundInfo);
            }
        }
    }

    private void GameScript_OnCardReady(int playerIndex, Card card)
    {
        //Debug.Log(string.Format("card ready {0} {1}", playerIndex, card));
        int finishIndex = beginIndex - 1;
        finishIndex = (finishIndex < 0 ? 3 : finishIndex);

        if (playerIndex == gameScript.MainPlayerIndex)
            gameScript.StopPlayerTimer();

        if (PhotonNetwork.IsMasterClient)
        {
            if (!Players[playerIndex].IsPlayer)
                turnManager.SendMove(Utils.SerializeCardAndPlayer(card, playerIndex), playerIndex == finishIndex);

            gameScript.RoundScript.OnCardReady(playerIndex, card);
        }
        else if (playerIndex == gameScript.MainPlayerIndex)
        {
            turnManager.SendMove(Utils.SerializeCardAndPlayer(card, playerIndex), playerIndex == finishIndex);
            gameScript.RoundScript.UpdateDealInfo(playerIndex, card);
        }
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void OnEvent(EventData photonEvent)
    {
        switch (photonEvent.Code)
        {
            case setGameReadyCode:
                gameScript.SetGameReady();

                if (PhotonNetwork.IsMasterClient)
                {
                    gameScript.StartGame();
                }
                break;
            //case gameReadyCode:
            //    beginIndex = (int)photonEvent.CustomData;
            //    gameScript.SetStartGame();

            //    if (PhotonNetwork.IsMasterClient)
            //        BeginTurn(beginIndex);
            //    break;
            case startGameCode:
                beginIndex = (int)photonEvent.CustomData;
                gameScript.SetPlaying(true);
                break;
            case trickFinishedCode:
                beginIndex = (int)photonEvent.CustomData;
                gameScript.SetTrickFinished(beginIndex);
                gameScript.RoundScript.RoundInfo.DrawCards();
                break;
            case roundFinishedCode:
                if (!PhotonNetwork.IsMasterClient)
                {
                    gameScript.RoundScript.RoundInfo.ClearCards();
                    gameScript.RoundFinished((int)photonEvent.CustomData, false);
                    //gameScript.SetDealFinished(false);
                }
                break;
            case messageCode:
                int index = LookUpActors.First(x => x.Value == photonEvent.Sender).Key;
                OnMessageRecieved?.Invoke(index, photonEvent.CustomData);
                break;
            case playerTurnCode:
                int turnIndex = int.Parse(photonEvent.CustomData.ToString());
                if (turnIndex != gameScript.MainPlayerIndex)
                    Players[turnIndex].SetTurn(gameScript.RoundScript.RoundInfo);
                break;
        }

        OnNetworkEvent?.Invoke(photonEvent);
    }

    public async void TrickFinishedSequence(int playIndex)
    {
        RaiseEventToOthers(trickFinishedCode, playIndex);
        gameScript.SetTrickFinished(playIndex);
        await System.Threading.Tasks.Task.Delay(100);
        BeginTurn(playIndex);
    }

    public void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {

    }

    public void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {

    }

    public void OnPlayerFinished(Photon.Realtime.Player player, int turn, object move)
    {
        PlayerMove(move, true);
    }

    public void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        if (otherPlayer.ActorNumber == 1)
        {
            //game script should not call uimanager straight fix later
            UIManager.Instance.HostLeft();
        }
        else if (PhotonNetwork.IsMasterClient)
        {
            int index = otherPlayer.ActorNumber - 1;

            PlayerBase oldPlayer = Players[index];

            PlayerBase aiPlayer = gameScript.CreateAIPlayer(index); //new AIPlayer(index);
            Players[index] = aiPlayer;
            ((IMergePlayer)aiPlayer).Merge(oldPlayer);

            if (nextIndex == index)
            {
                aiPlayer.SetTurn(gameScript.RoundScript.RoundInfo);
            }
        }
    }

    public void OnPlayerMove(Photon.Realtime.Player player, int turn, object move)
    {
        PlayerMove(move, false);
    }

    public void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
    {

    }

    public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {

    }

    public void OnTurnBegins(int turn)
    {
        //if (!PhotonNetwork.IsMasterClient && Players[beginIndex] is AIPlayer)
        //    return;

        //Players[beginIndex].SetTurn(Deal.DealInfo);
        //Debug.Log("on turn begin Index:" + beginIndex);
        if (!PhotonNetwork.IsMasterClient)
        {
            //Debug.Log(beginIndex + " " + gameScript.MainPlayerIndex);
            if (beginIndex == gameScript.MainPlayerIndex)
            {
                gameScript.MyPlayer.SetTurn(gameScript.RoundScript.RoundInfo);
            }
        }
        else if (!Players[beginIndex].IsPlayer)
        {
            gameScript.RoundScript.SetTurn();

            RaiseEventOptions eventOptionsCards = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
            PhotonNetwork.RaiseEvent(playerTurnCode, beginIndex, eventOptionsCards, SendOptions.SendReliable);
        }
    }

    public void OnTurnCompleted(int turn)
    {

    }

    public void OnTurnTimeEnds(int turn)
    {

    }

    public void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    internal void SendMessageToOthers(object message)
    {
        RaiseEventOptions eventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent(messageCode, message, eventOptions, SendOptions.SendReliable);
    }
}
