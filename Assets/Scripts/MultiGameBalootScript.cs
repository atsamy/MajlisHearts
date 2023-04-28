using ExitGames.Client.Photon;
using GooglePlayGames.BasicApi;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MultiGameBalootScript : GameScriptBaloot, ILeaveRoom,ISendMessage
{
    PunTurnManager turnManager;
    MultiPlayerScript multiPlayer;

    const int trickFinishedCode = 41;
    const int roundFinishedCode = 44;

    //const int cardsDealtCode = 40;

    //const int doubleCardCode = 45;
    //const int checkDoubleCode = 46;

    const int checkTypeCode = 50;
    const int dealBeginCode = 51;
    const int dealFinishCode = 52;
    const int playerSelectedTypeCode = 53;
    //const int typeSelectedCode = 54;
    const int doubleSelectedCode = 55;
    const int changedShapeCode = 56;
    const int checkProjectsCode = 57;
    const int projectCode = 58;
    const int allProjectCode = 59;
    const int continueDealCode = 60;
    const int checkDoubleCode = 61;
    const int RemaingCardsCode = 62;
    const int roundFinsihedDataCode = 63;
    //const int RestartDealCode = 61;
    //const int checkTypeCode = 61;

    int projectsCount = 0;
    void Start()
    {
        turnManager = gameObject.AddComponent<PunTurnManager>();
        multiPlayer = new MultiPlayerScript(turnManager, this, 8);

        foreach (PlayerBaloot player in Players)
        {
            player.OnTypeSelected += Players_SelectedType;
            player.OnDoubleSelected += GameScriptBaloot_OnDoubleSelected;
            player.OnChangedHokumShape += MultiGameBalootScript_OnChangedHokumShape;
            player.OnCardReady += GameScript_OnCardReady;

            if (player.IsPlayer)
            {
                player.OnCheckType += Player_OnCheckType;
                player.OnCheckDouble += Player_OnCheckDouble;
            }
        }


        RoundScript.SetPlayers(Players);

        balootRoundScript.OnEvent += Deal_OnEvent;
        balootRoundScript.OnGameTypeSelected += BalootRoundScript_OnGameTypeSelected;
        multiPlayer.OnNetworkEvent += MultiPlayer_OnNetworkEvent;

        OnStartPlaying?.Invoke(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S) && (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsEditor))
        {
            //foreach (var item in Players)
            //{
            //    item.Score = 140;
            //}
            TeamsTotalScore[0] = 150;
            TeamsTotalScore[1] = 150;

            multiPlayer.RaiseEventToOthers(73, null);
        }
    }

    private void Player_OnCheckDouble(int index, int value)
    {
        if (multiPlayer.LookUpActors.ContainsKey(index))
        {
            RaiseEventOptions eventOptions = new RaiseEventOptions { TargetActors = new int[] { multiPlayer.LookUpActors[index] } };
            PhotonNetwork.RaiseEvent(checkDoubleCode, new int[] { index, value }
            , eventOptions, SendOptions.SendReliable);
        }
        else
        {
            multiPlayer.RaiseEventToMaster(checkDoubleCode, new int[] { index, value });
        }
    }

    private void Player_OnCheckType(int index)
    {
        //RaiseEventCheckType(index);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    protected override void BalootRoundScript_OnGameTypeSelected(int index, BalootGameType gameType)
    {
        DeclarerIndex = index;
        doublerIndex = -1;
        DoubleValue = 0;

        if (index != MainPlayerIndex && !(PhotonNetwork.IsMasterClient && Players[index] is AIPlayerBaloot))
            return;

        if (gameType == BalootGameType.Hokum)
        {
            int[] indeces = new int[] { (index + 1) % 4, (index + 3) % 4 };

            foreach (var item in indeces)
            {
                if (multiPlayer.LookUpActors.ContainsKey(item))
                {
                    RaiseEventOptions eventOptions = new RaiseEventOptions { TargetActors = new int[] { multiPlayer.LookUpActors[item] } };
                    PhotonNetwork.RaiseEvent(checkDoubleCode, new int[] { item, 0 }, eventOptions, SendOptions.SendReliable);
                }
                else
                {
                    if (PhotonNetwork.IsMasterClient)
                    {
                        ((PlayerBaloot)Players[item]).CheckDouble(0);
                    }
                    else
                    {
                        multiPlayer.RaiseEventToMaster(checkDoubleCode, new int[] { item, 0 });
                    }
                }
            }
        }
        else
        {
            if (PhotonNetwork.IsMasterClient)
            {
                balootRoundScript.DealContinue(index);
            }
            else
            {
                multiPlayer.RaiseEventToMaster(continueDealCode, index);
            }
        }
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
            multiPlayer.RaiseEventToMaster(doubleSelectedCode, data);
        }
    }

    private void GameScript_OnCardReady(int playerIndex, Card card)
    {
        //print(string.Format("player index:{0} trick number:{1}", playerIndex, RoundScript.RoundInfo.TrickNumber));
        if (RoundScript.RoundInfo.TrickNumber == 0 && playerIndex == MainPlayerIndex)
        {
            OnHideProject?.Invoke();
        }

        if (RoundScript.RoundInfo.TrickNumber == 1)
        {
            if (((PlayerBaloot)Players[playerIndex]).PlayerProjects.Count > 0)
            {
                //print("project:" + playerIndex);
                RevealProject(playerIndex);
            }
        }
    }

    private void MultiGameBalootScript_OnChangedHokumShape(CardShape shape)
    {
        RaiseEventOptions eventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent(changedShapeCode, shape, eventOptions, SendOptions.SendReliable);
    }

    public override void Players_SelectedType(int index, BalootGameType type)
    {
        //if(photon)
        base.Players_SelectedType(index, type);

        RaiseEventOptions eventOptionsDeal = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent(playerSelectedTypeCode, new int[] { index, (int)type },
            eventOptionsDeal, SendOptions.SendReliable);
    }

    private void MultiPlayer_OnNetworkEvent(EventData photonEvent)
    {
        //print(photonEvent.Code);
        switch (photonEvent.Code)
        {
            case 73:
                TeamsTotalScore[0] = 150;
                TeamsTotalScore[1] = 150;
                break;
            case RemaingCardsCode:
                List<Card> ownedCards = Utils.DeSerializeListOfCardsAndValue((int[])photonEvent.CustomData,out DoubleValue);
                for (int i = 5; i < ownedCards.Count; i++)
                {
                    MyPlayer.AddCard(ownedCards[i]);
                }

                WaitCardDealing();
                break;
            case checkDoubleCode:
                int[] doubleData = (int[])photonEvent.CustomData;
                ((PlayerBaloot)Players[doubleData[0]]).CheckDouble(doubleData[1]);
                break;
            case continueDealCode:
                balootRoundScript.DealContinue((int)photonEvent.CustomData);
                break;
            case changedShapeCode:
                print("change: "+(CardShape)photonEvent.CustomData);
                balootRoundScript.balootRoundInfo.HokumShape = (CardShape)photonEvent.CustomData;
                break;
            case checkTypeCode:
                int[] typeData = (int[])photonEvent.CustomData;
                balootRoundScript.HokumIndex = typeData[0];
                balootRoundScript.BiddingRound = typeData[1];

                ((PlayerBaloot)Players[typeData[2]]).CheckGameType(balootRoundScript);
                break;
            case playerSelectedTypeCode:
                int[] data = (int[])photonEvent.CustomData;
                //use a function to save data not send events
                base.Players_SelectedType(data[0], (BalootGameType)data[1]);
                break;
            case doubleSelectedCode:
                int[] doubleSelectedData = (int[])photonEvent.CustomData;
                base.GameScriptBaloot_OnDoubleSelected(doubleSelectedData[0],
                    (doubleSelectedData[1] == 1), doubleSelectedData[2]);
                break;
            case dealBeginCode:
                foreach (var item in Players)
                {
                    item.Reset();
                }
                //fix this
                
                balootRoundScript.balootRoundInfo.TrickNumber = 0;

                List<Card> allCards = Utils.DeSerializeListOfCards((int[])photonEvent.CustomData);
                balootRoundScript.BalootCard = allCards.First();
                balootRoundScript.balootRoundInfo.HokumShape = balootRoundScript.BalootCard.Shape;

                balootRoundScript.IncrementStartIndex();
                balootRoundScript.ResetValues();

                allCards.RemoveAt(0);

                foreach (var item in allCards)
                {
                    MyPlayer.AddCard(item);
                }
                DealCards();
                break;
            case dealFinishCode:
                DealCardsThenStartGame();
                break;
            case checkProjectsCode:
                MainPlayerBaloot player = (MainPlayerBaloot)MyPlayer;
                player.ChooseProjects(balootRoundScript.RoundType);

                if (player.PlayerProjects.Count > 0)
                {
                    //send my projects
                    multiPlayer.RaiseEventToMaster(projectCode, Utils.SerializeProjects(player.PlayerProjects,
                        player.ProjectPower, player.ProjectScore));
                }
                else
                {
                    multiPlayer.RaiseEventToMaster(projectCode, null);
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
            case allProjectCode:
                Dictionary<int, Dictionary<List<Card>, Projects>> allProjects = Utils.DeserializePlayersProjects((int[])photonEvent.CustomData);

                for (int i = 0; i < Players.Length; i++)
                {
                    if (allProjects.ContainsKey(i))
                    {
                        ((PlayerBaloot)Players[i]).SetProjects(allProjects[i]);
                    }
                    else
                    {
                        ((PlayerBaloot)Players[i]).RemoveProjects();
                    }
                }

                break;

            case roundFinsihedDataCode:
                int[] roundFinsihedData = (int[])photonEvent.CustomData;
                balootRoundScript.SetRoundData(roundFinsihedData[1]);

                for (int i = 0; i < 4; i++)
                {
                    ((PlayerBaloot)Players[i]).SetScores(roundFinsihedData[i + 2], roundFinsihedData[i + 6],roundFinsihedData[i + 10]);
                }

                RoundScript.RoundInfo.ClearCards();
                RoundFinished(roundFinsihedData[0], false);
                break;
        }
    }

    protected override void CompareProjects()
    {
        base.CompareProjects();

        int[] data = Utils.SerializePlayersProjects(Players);
        if (data.Length > 0)
        {
            print("projects count: " + data.Length);
            multiPlayer.RaiseEventToOthers(allProjectCode, data);
        }
        //send final project to all other players
    }

    public override async void DealCardsThenStartGame()
    {
        await DealRemaingCards();
        multiPlayer.StartGame(balootRoundScript.StartIndex);
    }

    public async void WaitCardDealing()
    {
        await DealRemaingCards();

        //do more stuff
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
                foreach (var item in multiPlayer.LookUpActors)
                {
                    if (item.Key == 0)
                        continue;

                    RaiseEventOptions eventOptionsCards = new RaiseEventOptions { TargetActors = new int[] { item.Value } };
                    PhotonNetwork.RaiseEvent(RemaingCardsCode, Utils.SerializeListOfCardsAndValue(Players[item.Key].OwnedCards, DoubleValue),
                        eventOptionsCards, SendOptions.SendReliable);
                }
                DealCardsThenStartGame();
                break;
            case EventTypeBaloot.RestartDeal:
                if (PhotonNetwork.IsMasterClient)
                {
                    RestartGame();
                }
                else
                {
                    RestartEvent();
                }
                //multiPlayer.RaiseEventToOthers(RestartDealCode, null);
                break;
            case EventTypeBaloot.TrickFinished:
                if (balootRoundScript.balootRoundInfo.TrickNumber == 0)
                {
                    ((MainPlayerBaloot)MyPlayer).ChooseProjects(balootRoundScript.RoundType);
                    projectsCount = 1;

                    foreach (PlayerBaloot item in Players)
                    {
                        if (item is AIPlayerBaloot)
                        {
                            item.ChooseProjects(balootRoundScript.RoundType);
                            projectsCount ++;
                        }
                    }

                    if (projectsCount == 4)
                    {
                        CompareProjects();
                    }
                    else
                    {
                        multiPlayer.RaiseEventToOthers(checkProjectsCode, null);
                    }
                }

                multiPlayer.TrickFinishedSequence(RoundScript.PlayingIndex);
                //multiPlayer.RaiseEventToAll(trickFinishedCode, RoundScript.PlayingIndex);
                //multiPlayer.BeginTurn(RoundScript.PlayingIndex);
                //SetTrickFinished(RoundScript.PlayingIndex);
                break;
            case EventTypeBaloot.DealFinished:

                //roundPoints[i].text = balootGame.RoundScore[i].ToString();
                //teamTotalPoints[i].text = balootGame.TeamsTotalScore[i].ToString();
                //totalPoints[i].text = balootGame.TeamsScore[i].ToString();
                //ProjectPoints[i].text = (((PlayerBaloot)game.Players[i + 0]).ProjectScore +
                //    ((PlayerBaloot)game.Players[i + 2]).ProjectScore).ToString();
                //FloorPoints[i].text = balootGame.balootRoundScript.FloorPoints == i ? "10" : "0";
                //teamCrown[i].SetActive(balootGame.WinningTeam == i);
                //?????
                //find me
                RoundFinished(RoundScript.PlayingIndex, true);

                int[] data = new int[14];

                data[0] = RoundScript.PlayingIndex;
                data[1] = balootRoundScript.FloorPoints;

                for (int i = 0; i < 4; i++)
                {
                    data[i + 2] = Players[i].TricksCount;
                    data[i + 6] = Players[i].Score;
                    data[i + 10] = ((PlayerBaloot)Players[i]).ProjectScore;
                }

                RaiseEventOptions eventOptionsDeal = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
                PhotonNetwork.RaiseEvent(roundFinsihedDataCode, data, eventOptionsDeal, SendOptions.SendReliable);


                break;
        }
    }

    private void OnDisable()
    {
        multiPlayer.OnDisable();
    }

    public void SendMessage(object message)
    {
        multiPlayer.SendMessageToOthers(message);
    }
}
