using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class BalootCardsUIManager : CardsUIManager
{
    public Image BalootCard;

    [SerializeField]
    ProjectCards[] projectCards;
    public override void ShowPlayerCards(PlayerBase mainPlayer, bool setInteractable, int count)
    {
        playerCardsUI = new List<CardUI>();
        playableCards = new List<CardUI>();

        for (int i = 0; i < count; i++)
        {
            GameObject newCard = Instantiate(cardElementsHolder.playerCard, cardElementsHolder.CardsHolder[0].transform);
            playerCardsUI.Add(newCard.GetComponent<CardUI>());

            Card card = mainPlayer.OwnedCards[i];

            playerCardsUI.Last().Set(cardElementsHolder.cardShapeSprites[(int)card.Shape].Sprites[(int)card.Rank], card, (card) =>
            {
                mainPlayer.ChooseCard(card);
                //AddToPassCards(newCard.GetComponent<CardUI>());
            });

            playerCardsUI.Last().SetInteractable(setInteractable);
        }

        AddCards(count);
        OrganizeCards();
    }

    public void RevealCards(int playerIndex,Dictionary<List<Card>, Projects> AllProjects)
    {
        for (int i = 0; i < AllProjects.Count; i++)
        {
            foreach (var item in AllProjects.ElementAt(i).Key)
            {
                GameObject newCard = Instantiate(cardBack, projectCards[playerIndex].CardsHolder[i].transform);
                newCard.GetComponent<RectTransform>().sizeDelta = new Vector2(55,80);

                newCard.transform.DOScaleX(0, 0.25f).SetDelay(1).OnComplete(() =>
                {
                    newCard.GetComponent<Image>().sprite = 
                    cardElementsHolder.cardShapeSprites[(int)item.Shape].Sprites[(int)item.Rank];

                    newCard.transform.DOScaleX(1, 0.25f);
                });
            }

            projectCards[playerIndex].NameText[i].text = AllProjects.ElementAt(i).Value.ToString();
        }

        StartCoroutine(RemoveProjectCards(playerIndex));
    }

    IEnumerator RemoveProjectCards(int index)
    {
        yield return new WaitForSeconds(3);
        projectCards[index].RemoveAll();
    }

    protected override bool CheckIfPlayable(Card card, RoundInfo trickInfo, PlayerBase player)
    {
        BalootRoundInfo info = (BalootRoundInfo)trickInfo;
        bool firstHand = trickInfo.CardsOntable.Count == 0;

        if (trickInfo.TrickShape != card.Shape && player.HasShape(trickInfo.TrickShape) && !firstHand)
            return false;

        return true;
    }

    internal void AddRemaingCards(PlayerBase mainPlayer)
    {
        AddCards(3);

        for (int i = 5; i < 8; i++)
        {
            GameObject newCard = Instantiate(cardElementsHolder.playerCard, cardElementsHolder.CardsHolder[0].transform);
            playerCardsUI.Add(newCard.GetComponent<CardUI>());

            Card card = mainPlayer.OwnedCards[i];

            playerCardsUI.Last().Set(cardElementsHolder.cardShapeSprites[(int)card.Shape].Sprites[(int)card.Rank], card, (card) =>
            {
                mainPlayer.ChooseCard(card);
                //AddToPassCards(newCard.GetComponent<CardUI>());
            });

            playerCardsUI.Last().SetInteractable(false);
        }

        //fix
        //StartCoroutine(UpdateCards(mainPlayer));
        SetPlayerCards(mainPlayer);
        OrganizeCards();

        //for (int i = 1; i < cardElementsHolder.CardsHolder.Length; i++)
        //{
        //    cardElementsHolder.CardsHolder[i].SetLocations();
        //}

        BalootCard.gameObject.SetActive(false);
    }

    public void AddBalootCard(Card card)
    {
        BalootCard.gameObject.SetActive(true);
        BalootCard.sprite = cardElementsHolder.cardShapeSprites[(int)card.Shape].Sprites[(int)card.Rank];
    }

    internal void RemoveAllCards()
    {

        foreach (var item in cardElementsHolder.CardsHolder)
        {
            item.RemoveCards();
        }
        BalootCard.gameObject.SetActive(false);
        playerCardsUI.Clear();
    }
}
