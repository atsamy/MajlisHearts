using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardsUIManager : MonoBehaviour
{
    public GameObject playerCard;
    public Transform playerCardsParent;

    List<CardUI> PlayerCardsUI;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowPlayerCards(MainPlayer mainPlayer)
    {
        for (int i = 0; i < 13; i++)
        {
            GameObject newCard = Instantiate(playerCard, playerCardsParent);
            newCard.transform.localPosition = new Vector3((i - 6) * 100, 0);

            PlayerCardsUI.Add(newCard.GetComponent<CardUI>());
            PlayerCardsUI.Last().Set(mainPlayer.OwnedCards[i], (card) =>
             {
                 mainPlayer.ChooseCard(card);
             });
        }
    }

    public void SetPlayableCards(TrickInfo info, Player player,bool firstHand)
    {
        foreach (var item in PlayerCardsUI)
        {
            item.SetInteractable(checkIfPlayable(item.CardInfo, info,player, firstHand));
        }
    }

    bool checkIfPlayable(Card card, TrickInfo trickInfo, Player player,bool firstHand)
    {
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