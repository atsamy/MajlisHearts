using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Player
{
    public List<Card> OwnedCards { get; protected set; }

    int index;
    int dealScore;
    int totalScore;
    public Sprite Avatar;

    public int Index { get => index; }

    public int Score { get => dealScore; set => dealScore = value; }
    public int TotalScore { get => totalScore; protected set => totalScore = value; }

    protected Dictionary<CardShape, int> shapeCount;

    public Dictionary<CardShape, int> ShapeCount { get => shapeCount; }

    public string Name;

    public delegate void PassCardsReady(int playerIndex, List<Card> cards);
    public PassCardsReady OnPassCardsReady;

    public delegate void CardReady(int playerIndex, Card card);
    public CardReady OnCardReady;

    public delegate void DoubleCard(Card card, bool value,int playerIndex);
    public DoubleCard OnDoubleCard;

    protected bool isPlayer;

    public bool DidLead { get; protected set; }

    public bool IsPlayer { get => isPlayer; }

    public Player(int index)
    {
        shapeCount = new Dictionary<CardShape, int>();

        for (int i = 0; i < 4; i++)
        {
            shapeCount.Add((CardShape)i,0);
        }

        OwnedCards = new List<Card>();
        this.index = index;

        isPlayer = true;
    }

    public bool HasCard(Card card)
    {
        return OwnedCards.Contains(card);
    }

    public async void CheckForDoubleCards()
    {
        await System.Threading.Tasks.Task.Delay(2000);

        bool hasDoubleCard = false;

        if (HasCard(Card.QueenOfSpades))
        {
            CheckDoubleCards(Card.QueenOfSpades);
            hasDoubleCard = true;
        }
        if (HasCard(Card.TenOfDiamonds))
        {
            CheckDoubleCards(Card.TenOfDiamonds);
            hasDoubleCard = true;
        }

        if (!hasDoubleCard)
        {
            WaitForOthers();
        }
    }

    public int GetShapeCount(CardShape shape)
    {
        return shapeCount[shape];
    }

    public bool HasOnlyHearts()
    {
        return shapeCount[CardShape.Heart] == OwnedCards.Count;
    }

    public virtual void ChooseCard(Card card)
    {
        OwnedCards.Remove(card);
        shapeCount[card.Shape]--;
        ShowCard(card);
    }

    public void ShowCard(Card card)
    {
        OnCardReady?.Invoke(index, card);
    }

    public virtual void PassCards(List<Card> cards)
    {
        foreach (var item in cards)
        {
            OwnedCards.Remove(item);
            shapeCount[item.Shape]--;
        }

        OnPassCardsReady?.Invoke(index,cards);
    }

    public virtual void SelectPassCards()
    {

    }

    protected virtual void CheckDoubleCards(Card card)
    {

    }

    protected virtual void WaitForOthers()
    {

    }

    public void SetDoubleCard(Card card, bool value)
    {
        OnDoubleCard?.Invoke(card,value,index);
    }

    public virtual void SetTurn(DealInfo info)
    {
        
    }

    public bool HasShape(CardShape shape)
    {
        return shapeCount[shape] > 0;
    }


    public void AddCard(Card card)
    {
        OwnedCards.Add(card);
        shapeCount[card.Shape]++;
    }

    public virtual void AddPassCards(List<Card> cards)
    {
        foreach (var item in cards)
        {
            AddCard(item);
        }
    }

    public void IncrementScore(int score)
    {
        DidLead = true;
        dealScore += score;
    }

    public void SetTotalScore()
    {
        totalScore += dealScore;
        dealScore = 0;
    }

    public virtual void Reset()
    {
        DidLead = false;
        OwnedCards.Clear();

        for (int i = 0; i < 4; i++)
        {
            shapeCount[(CardShape)i] = 0;
        }
        //shapeCount.Clear();
    }

}

public enum PlayerState
{
    Waiting,
    ChoosePass,
    YourTurn
}
