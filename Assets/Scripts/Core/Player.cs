using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Player
{
    public List<Card> OwnedCards { get; protected set; }

    int index;
    int dealScore;
    int totalScore;

    public int Score { get => dealScore; set => dealScore = value; }
    public int TotalScore { get => totalScore; }

    protected Dictionary<CardShape,int> shapeCount;

    public string Name;

    public delegate void PassCardsReady(int playerIndex,List<Card> cards);
    public event PassCardsReady OnPassCardsReady;

    public delegate void CardReady(int playerIndex, Card card);
    public event CardReady OnCardReady;

    public Player(int index)
    {
        shapeCount = new Dictionary<CardShape, int>();

        for (int i = 0; i < 4; i++)
        {
            shapeCount.Add((CardShape)i,0);
        }

        OwnedCards = new List<Card>();
        this.index = index;
    }

    public int GetShapeCount(CardShape shape)
    {
        return shapeCount[shape];
    }

    public virtual void ChooseCard(Card card)
    {
        OwnedCards.Remove(card);
        shapeCount[card.Shape]--;
        OnCardReady?.Invoke(index,card);
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

    public virtual void SetTurn(DealInfo info, int hand)
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
        dealScore += score;
    }

    public void SetTotalScore()
    {
        totalScore += dealScore;
        dealScore = 0;
    }

}

public enum PlayerState
{
    Waiting,
    ChoosePass,
    YourTurn
}
