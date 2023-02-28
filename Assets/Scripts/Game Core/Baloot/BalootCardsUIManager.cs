using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Threading.Tasks;
using Mono.Cecil;

public class BalootCardsUIManager : CardsUIManager
{
    public Transform deckParent;
    GameObject[] deck;

    [SerializeField]
    ProjectCards[] projectCards;
    public async void ShowPlayerCardsBaloot(PlayerBase mainPlayer, Card balootCard)
    {
        playerCardsUI = new List<CardUI>();
        playableCards = new List<CardUI>();

        deck = new GameObject[2];

        //Vector3 originPos = new Vector3(Screen.width / 2, Screen.height / 2);

        for (int i = 0; i < 2; i++)
        {
            deck[i] = Instantiate(cardBack, deckParent);
            deck[i].transform.localPosition = Vector3.zero;
        }

        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < (3 - i); j++)
            {
                GameObject newCard = Instantiate(cardElementsHolder.playerCard, cardElementsHolder.CardsHolder[0].transform);
                playerCardsUI.Add(newCard.GetComponent<CardUI>());

                Card card = mainPlayer.OwnedCards[j + (i * 3)];

                playerCardsUI.Last().Set(cardElementsHolder.cardShapeSprites[(int)card.Shape].Sprites[(int)card.Rank], card, (card) =>
                {
                    mainPlayer.ChooseCard(card);
                    //AddToPassCards(newCard.GetComponent<CardUI>());
                });

                playerCardsUI.Last().SetInteractable(false);

                newCard.transform.position = deckParent.position;
                //newCard.transform.DOLocalMove(Vector3.zero, 0.25f);
            }

            cardElementsHolder.CardsHolder[0].SetLocations(0.3f);
            await Task.Delay(500);
            AddVerticalCardsInOrigin(cardElementsHolder.CardsHolder[1], cardBack, 90, (3 - i), deckParent.position);
            await Task.Delay(500);
            AddHorizontalCardsInOrigin(cardElementsHolder.CardsHolder[2], cardBack, (3 - i), deckParent.position);
            await Task.Delay(500);
            AddVerticalCardsInOrigin(cardElementsHolder.CardsHolder[3], cardBack, -90, (3 - i), deckParent.position);
            await Task.Delay(500);


        }
        //await Task.Delay(300);

        OrganizeCards();

        AddBalootCard(balootCard);
    }

    public void RevealCards(int playerIndex, Dictionary<List<Card>, Projects> AllProjects)
    {
        for (int i = 0; i < AllProjects.Count; i++)
        {
            foreach (var item in AllProjects.ElementAt(i).Key)
            {
                GameObject newCard = Instantiate(cardBack, projectCards[playerIndex].CardsHolder[i].transform);
                newCard.GetComponent<RectTransform>().sizeDelta = new Vector2(55, 80);

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

    internal void AddRemaingCards(PlayerBase mainPlayer, BalootGameType balootGameType)
    {
        deck[1].gameObject.SetActive(false);

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

        if (balootGameType == BalootGameType.Hokum)
            OrganizeCards(CardHelper.HokumRank);
        else
            OrganizeCards(CardHelper.SunRank);

        //for (int i = 1; i < cardElementsHolder.CardsHolder.Length; i++)
        //{
        //    cardElementsHolder.CardsHolder[i].SetLocations();
        //}
        deck[0].gameObject.SetActive(false);

    }

    public void OrganizeCards(Dictionary<CardRank, int> adjustRanks)
    {
        playerCardsUI = playerCardsUI.OrderBy(a => a.CardInfo.Shape).ThenByDescending(a => adjustRanks[a.CardInfo.Rank]).ToList();

        for (int i = 0; i < playerCardsUI.Count; i++)
        {
            playerCardsUI[i].transform.SetSiblingIndex(i);
        }

        cardElementsHolder.CardsHolder[0].SetLocations();
    }

    public void AddBalootCard(Card card)
    {
        deck[1].transform.DOScale(new Vector3(0, 1.2f, 1), 0.25f).OnComplete(() =>
        {
            deck[1].GetComponent<Image>().sprite = cardElementsHolder.cardShapeSprites[(int)card.Shape].Sprites[(int)card.Rank];
            deck[1].transform.DOScale(Vector3.one, 0.25f);
        });
        //BalootCard.gameObject.SetActive(true);
        //
    }

    internal void RemoveAllCards()
    {

        foreach (var item in cardElementsHolder.CardsHolder)
        {
            item.RemoveCards();
        }
        deck[0].gameObject.SetActive(false);
        deck[1].gameObject.SetActive(false);
        playerCardsUI.Clear();
    }
}
