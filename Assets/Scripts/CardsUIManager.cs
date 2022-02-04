using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardsUIManager : MonoBehaviour
{
    public GameObject playerCard;
    public Transform playerCardsParent;

    List<CardUI> playerCardsUI;

    public void ShowPlayerCards(MainPlayer mainPlayer)
    {
        playerCardsUI = new List<CardUI>();

        Sprite[] cardSprites = Resources.LoadAll<Sprite>("Cards/classic-playing-cards");

        for (int i = 0; i < 13; i++)
        {
            GameObject newCard = Instantiate(playerCard, playerCardsParent);
            newCard.transform.localPosition = new Vector3((i - 6) * 100, 0);

            playerCardsUI.Add(newCard.GetComponent<CardUI>());

            Card card = mainPlayer.OwnedCards[i];

            int rankModifid = ((int)card.Rank) + 1;
            rankModifid %= 13;

            //print("classic - playing - cards_" + (((int)card.Shape * 13) + rankModifid));

            Sprite sprite = cardSprites.Single(s => s.name == "classic-playing-cards_" + (((int)card.Shape * 13) + rankModifid));

            playerCardsUI.Last().Set(sprite, card, (card) =>
             {
                 mainPlayer.ChooseCard(card);
             });

            playerCardsUI.Last().SetInteractable(false);
        }
    }

    public void SetPlayableCards(TrickInfo info, Player player,bool firstHand)
    {
        foreach (var item in playerCardsUI)
        {
            item.SetInteractable(checkIfPlayable(item.CardInfo, info,player, firstHand));
        }
    }

    public void DisableAllCards()
    {
        foreach (var item in playerCardsUI)
        {
            item.SetInteractable(false);
        }
    }

    bool checkIfPlayable(Card card, TrickInfo trickInfo, Player player,bool firstHand)
    {
        if (trickInfo.roundNumber == 0 && firstHand && !(card.Shape == CardShape.Club && card.Rank == CardRank.Two))
            return false;
        if (trickInfo.TrickShape != card.Shape && player.HasShape(trickInfo.TrickShape) && !firstHand)
            return false;
        if (trickInfo.roundNumber == 0)
        {
            if (card.Shape == CardShape.Heart)
                return false;
            if (card.Shape == CardShape.Spade && card.Rank == CardRank.Queen)
                return false;
        }
        if (!trickInfo.heartBroken && card.Shape == CardShape.Heart && firstHand)
            return false;

        return true;
    }
}

public class TrickInfo
{
    public CardShape TrickShape;
    public bool heartBroken;
    public int roundNumber;

    public TrickInfo()
    {
        TrickShape = CardShape.Club;
        heartBroken = false;
        roundNumber = 0;
    }
}