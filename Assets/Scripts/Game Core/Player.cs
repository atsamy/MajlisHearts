using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Player:PlayerBase
{
    public delegate void PassCardsReady(int playerIndex, List<Card> cards);
    public PassCardsReady OnPassCardsReady;

    public delegate void DoubleCard(Card card, bool value,int playerIndex);
    public DoubleCard OnDoubleCard;

    public bool DidLead { get; protected set; }


    public Player(int index):base(index)
    {

    }

    public override void SetTotalScore()
    {
        totalScore += dealScore;
        dealScore = 0;
    }

    public async void CheckForDoubleCards()
    {
        await System.Threading.Tasks.Task.Delay(2000);

        bool hasDoubleCard = false;

        if (HasCard(CardHelper.QueenOfSpades))
        {
            CheckDoubleCards(CardHelper.QueenOfSpades);
            hasDoubleCard = true;
        }
        if (HasCard(CardHelper.TenOfDiamonds))
        {
            CheckDoubleCards(CardHelper.TenOfDiamonds);
            hasDoubleCard = true;
        }

        if (!hasDoubleCard)
        {
            WaitForOthers();
        }
    }

    public bool HasOnlyHearts()
    {
        return shapeCount[CardShape.Heart] == OwnedCards.Count;
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

    public void OrderCards()
    {
        OwnedCards = OwnedCards.OrderBy(a => a.Shape).ToList();
    }

    public virtual void SelectPassCards()
    {

    }

    protected virtual void CheckDoubleCards(Card card)
    {

    }

    public void SetDoubleCard(Card card, bool value)
    {
        OnDoubleCard?.Invoke(card,value,index);
    }

    public virtual void AddPassCards(List<Card> cards)
    {
        foreach (var item in cards)
        {
            AddCard(item);
        }
    }

    public override void IncrementScore(int score)
    {
        DidLead = true;
        base.IncrementScore(score);
    }

    public override void Reset()
    {
        DidLead = false;
        base.Reset();
    }

}

public enum PlayerState
{
    Waiting,
    ChoosePass,
    YourTurn
}
