using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HeartsCardsUIManager : CardsUIManager
{
    public override void ShowPlayerCards(PlayerBase mainPlayer, bool setInteractable, int count)
    {
        playerCardsUI = new List<CardUI>();
        playableCards = new List<CardUI>();

        for (int i = 0; i < count; i++)
        {
            GameObject newCard = Instantiate(playerCard, CardsHolder[0].transform);
            playerCardsUI.Add(newCard.GetComponent<CardUI>());

            Card card = mainPlayer.OwnedCards[i];

            playerCardsUI.Last().Set(cardShapeSprites[(int)card.Shape].Sprites[(int)card.Rank], card, (card) =>
            {
                AddToPassCards(newCard.GetComponent<CardUI>());
            });

            playerCardsUI.Last().SetInteractable(setInteractable);
        }

        AddCards(count);
        OrganizeCards();
    }

    internal void AddDoubledCard(Card card, int index)
    {
        if (card.IsTenOfDiamonds)
            playersDetails[index].ShowDouble(0);
        else if (card.IsQueenOfSpades)
            playersDetails[index].ShowDouble(1);
    }

    public void AddToPassCards(CardUI cardUI)
    {
        if (HeartsUIManager.Instance.AddCard(cardUI))
        {
            playerCardsUI.Remove(cardUI);
            cardUI.SetOnPressed((card) =>
            {
                ReturnToStack(cardUI);
            });

            cardUI.transform.rotation = Quaternion.identity;
            CardsHolder[0].SetLocations();
        }
        else
        {
            UIManager.Instance.SetCardLocations();
        }
    }

    public void ReturnToStack(CardUI cardUI)
    {
        playerCardsUI.Add(cardUI);
        cardUI.transform.SetParent(CardsHolder[0].transform);
        HeartsUIManager.Instance.RemoveCard(cardUI);
        cardUI.PassCard = false;
        cardUI.SetOnPressed((card) =>
        {
            AddToPassCards(cardUI);
        });

        OrganizeCards();
    }

    private void RemoveDoubleIcon(Card card, int index)
    {
        if (card.IsTenOfDiamonds)
            playersDetails[index].HideDouble(0);
        else if (card.IsQueenOfSpades)
            playersDetails[index].HideDouble(1);
    }

    public override void CardsPlayed(int playerIndex, Card card)
    {
        base.CardsPlayed(playerIndex, card);
        RemoveDoubleIcon(card, playerIndex);
    }

    public override void MainPlayerCard(CardUI cardUI)
    {
        base.MainPlayerCard(cardUI);
        RemoveDoubleIcon(cardUI.CardInfo, 0);
    }

    protected override bool CheckIfPlayable(Card card, RoundInfo trickInfo, PlayerBase player)
    {
        bool firstHand = trickInfo.CardsOntable.Count == 0;

        if (trickInfo.TrickNumber == 0 && firstHand && !(card.Shape == CardShape.Club && card.Rank == CardRank.Two))
            return false;
        if (trickInfo.TrickShape != card.Shape && player.HasShape(trickInfo.TrickShape) && !firstHand)
            return false;
        //if (trickInfo.roundNumber == 0)
        //{
        //    if (card.Shape == CardShape.Heart)
        //        return false;
        //    if (card.IsQueenOfSpades || card.IsTenOfDiamonds)
        //        return false;
        //}
        //if (!trickInfo.heartBroken && card.Shape == CardShape.Heart && firstHand && !player.HasOnlyHearts())
        //    return false;

        return true;
    }
}
