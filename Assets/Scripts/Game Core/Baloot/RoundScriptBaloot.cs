using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoundScriptBaloot : RoundScriptBase
{
    public int StartIndex;
    BalootRoundInfo balootRoundInfo => (BalootRoundInfo)RoundInfo;

    public Card BalootCard;
    List<Card> AllCards;

    public BalootGameType RoundType => balootRoundInfo.BalootRoundType;

    public int BidingTeam { get; internal set; }

    public RoundScriptBaloot()
    {
        cardsOnDeck = new Dictionary<int, Card>();
        RoundInfo = new BalootRoundInfo();
        playingIndex = 0;
        StartIndex = -1;
    }

    public override void Deal()
    {
        AllCards = GetAllCards();

        for (int i = 0; i < players.Length; i++)
        {
            players[i].Reset();

            for (int j = 0; j < 5; j++)
            {
                int getRandom = Random.Range(0, AllCards.Count);

                players[i].AddCard(AllCards[getRandom]);
                AllCards.RemoveAt(getRandom);
            }
        }

        BalootCard = AllCards[Random.Range(0, AllCards.Count)];
        AllCards.Remove(BalootCard);
    }

    private void IncrementStartIndex()
    {
        StartIndex++;
        StartIndex %= 4;
    }

    public override int EvaluateDeck(out int value)
    {
        Card winningCard = cardsOnDeck.ElementAt(0).Value;
        int index = cardsOnDeck.ElementAt(0).Key;

        value = 0;

        value += CardHelper.GetCardValue(balootRoundInfo.BalootRoundType, winningCard);// GetValue(winningCard);

        for (int i = 1; i < 4; i++)
        {
            Card currentCard = cardsOnDeck.ElementAt(i).Value;
            value += CardHelper.GetCardValue(balootRoundInfo.BalootRoundType, currentCard);
            if (currentCard.Shape == winningCard.Shape)
            {
                if (CardHelper.GetCardRank(balootRoundInfo.BalootRoundType,currentCard) > CardHelper.GetCardRank(balootRoundInfo.BalootRoundType, winningCard))
                {
                    index = cardsOnDeck.ElementAt(i).Key;
                    winningCard = currentCard;
                }
            }
        }

        return index;
    }

    public override void OnCardReady(int playerIndex, Card card)
    {
        cardsOnDeck.Add(playerIndex, card);
        RoundInfo.CardsOntable.Add(card);
        RoundInfo.ShapesOnGround[card.Shape]++;

        if (cardsOnDeck.Count == 1)
        {
            RoundInfo.TrickShape = card.Shape;
        }

        if (cardsOnDeck.Count == 4)
        {
            int value = 0;
            int winningHand = EvaluateDeck(out value);
            cardsOnDeck.Clear();
            players[winningHand].IncrementScore(value);

            playingIndex = winningHand;

            RoundInfo.DrawCards();

            if (RoundInfo.TrickNumber < 8)
            {
                TrickFinished((int)EventTypeBaloot.TrickFinished);
            }
            else
            {
                players[winningHand].IncrementScore(10);
                DealFinished((int)EventTypeBaloot.TrickFinished, (int)EventTypeBaloot.DealFinished);
            }
        }
        else
        {
            playingIndex++;
            playingIndex %= 4;

            //OnNextTurn?.Invoke();
            players[playingIndex].SetTurn(RoundInfo);
        }
    }

    public void DealContinue(int playerIndex)
    {
        players[playerIndex].AddCard(BalootCard);

        int index = playerIndex;

        while (AllCards.Count > 0)
        {
            index++;
            index %= 4;
            int getRandom = Random.Range(0, AllCards.Count);
            players[index].AddCard(AllCards[getRandom]);
            AllCards.RemoveAt(getRandom);
        }

        OnEvent?.Invoke((int)EventTypeBaloot.CardsDealtFinished);
    }

    public override void StartNewRound()
    {
        IncrementStartIndex();

        Deal();
        RoundInfo = new BalootRoundInfo();

        OnEvent?.Invoke((int)EventTypeBaloot.CardsDealtBegin);
    }

    private List<Card> GetAllCards()
    {
        List<Card> cards = new List<Card>();

        for (int i = 0; i < 4; i++)
        {
            for (int j = 5; j < 13; j++)
            {
                cards.Add(new Card((CardShape)i, (CardRank)j));
            }
        }

        return cards;
    }

    internal void SetGameType(int index, BalootGameType type)
    {
        balootRoundInfo.BalootRoundType = type;
        DealContinue(index);
        players[StartIndex].SetTurn(RoundInfo);
        playingIndex = StartIndex;

        BidingTeam = (index == 0 || index == 2) ? 0 : 1;
    }

    int typeRound;
    bool hukomSelected;
    internal void PlayerSelectedType(int index, BalootGameType type)
    {
        if (index == StartIndex)
            typeRound++;

        switch (type)
        {
            case BalootGameType.Sun:
                SetGameType(index, type);
                break;
            case BalootGameType.Hukom:
                break;
            case BalootGameType.Ashkal:
                SetGameType((index + 2) % 4, BalootGameType.Sun);
                break;
            case BalootGameType.Pass:
                int nextIndex = (index + 1) % 4;

                if (StartIndex == nextIndex && typeRound == 2 && !hukomSelected)
                {
                    //start new deal
                    typeRound = 0;
                    OnEvent?.Invoke((int)EventTypeBaloot.RestartDeal);
                }
                else if (StartIndex == nextIndex && typeRound == 2)
                {
                    SetGameType(index, BalootGameType.Hukom);
                }
                else
                {
                    ((BalootPlayer)players[nextIndex]).CheckGameType();
                }
                break;
        }
    }
}

public class BalootRoundInfo: RoundInfo
{
    public BalootGameType BalootRoundType;

    public BalootRoundInfo():base()
    {

    }
}

public enum EventTypeBaloot
{
    CardsDealtBegin,
    CardsDealtFinished,
    RestartDeal,
    TrickFinished,
    DealFinished
}
