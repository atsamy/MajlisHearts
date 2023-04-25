using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBase
{
    public List<Card> OwnedCards { get; protected set; }

    protected int index;
    protected int dealScore;
    protected int totalScore;
    public Sprite Avatar;

    public delegate void CardReady(int playerIndex, Card card);
    public CardReady OnCardReady;

    public delegate void PlayerTurn(int index, RoundInfo info);
    public PlayerTurn OnPlayerTurn;
    public int Index { get => index; }
    public int Score { get => dealScore; set => dealScore = value; }
    public int TotalScore { get => totalScore; protected set => totalScore = value; }

    protected Dictionary<CardShape, int> shapeCount;

    public Dictionary<CardShape, int> ShapeCount { get => shapeCount; }

    public string Name;

    protected bool isPlayer;

    public int TricksCount { get;protected set; }

    public bool IsPlayer { get => isPlayer; }
    public Action OnForcePlay;

    public PlayerBase(int index)
    {
        shapeCount = new Dictionary<CardShape, int>();

        for (int i = 0; i < 4; i++)
        {
            shapeCount.Add((CardShape)i, 0);
        }

        OwnedCards = new List<Card>();
        this.index = index;

        isPlayer = true;
    }

    public virtual void ChooseCard(Card card)
    {
        Debug.Log("choose card:" + card);
        OwnedCards.Remove(card);
        shapeCount[card.Shape]--;
        ShowCard(card);
    }

    public bool HasCard(Card card)
    {
        return OwnedCards.Contains(card);
    }

    public void ShowCard(Card card)
    {
        OnCardReady?.Invoke(index, card);
    }

    public virtual void SetTurn(RoundInfo info)
    {
        OnPlayerTurn?.Invoke(index, info);
    }

    public bool HasShape(CardShape shape)
    {
        return shapeCount[shape] > 0;
    }

    public void AddCard(Card card)
    {
        if (OwnedCards.Contains(card))
        {
            Debug.LogError("player already has card");
            return;
        }

        OwnedCards.Add(card);
        shapeCount[card.Shape]++;
    }

    public virtual void Reset()
    {
        OwnedCards.Clear();
        TricksCount = 0;
        for (int i = 0; i < 4; i++)
        {
            shapeCount[(CardShape)i] = 0;
        }
    }

    public virtual void SetTotalScore()
    {

    }

    public virtual void IncrementScore(int score)
    {
        dealScore += score;
    }

    public void IncrementTricks()
    {
        TricksCount++;
    }


    protected virtual void WaitForOthers()
    {

    }

    public int GetShapeCount(CardShape shape)
    {
        return shapeCount[shape];
    }

    internal void ForcePlay()
    {
        OnForcePlay?.Invoke();
    }
}
