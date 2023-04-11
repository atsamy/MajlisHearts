using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Threading.Tasks;

public class BalootCardsUIManager : CardsUIManager
{
    public Transform deckParent;
    GameObject[] deck;

    [SerializeField]
    ProjectCards[] projectCards;
    public async Task ShowPlayerCardsBaloot(PlayerBase mainPlayer, Card balootCard, int startIndex)
    {
        playerCardsUI = new List<CardUI>();
        playableCards = new List<CardUI>();

        deck = new GameObject[2];

        for (int i = 0; i < 2; i++)
        {
            deck[i] = Instantiate(cardBack, deckParent);
            deck[i].transform.localPosition = Vector3.zero;
        }

        for (int i = 0; i < 2; i++)
        {
            await ShuffleCards(mainPlayer, startIndex, 3 - i, i * 3, () =>
            {
                OrganizeCards();
            }, null);
        }

        AddBalootCard(balootCard);
    }

    private async Task ShuffleCards(PlayerBase mainPlayer, int startIndex, int count, int round, Action organizeCards, Action LastCards)
    {
        for (int j = startIndex; j < 4 + startIndex; j++)
        {
            int index = j % 4;

            List<GameObject> newCards = InitCards(count, cardElementsHolder.CardsHolder[index], deckParent.position, index == 0 ? cardElementsHolder.playerCard : cardBack);

            if (index == 0)
            {
                for (int k = 0; k < newCards.Count; k++)
                {
                    CardUI cardUI = newCards[k].GetComponent<CardUI>();
                    playerCardsUI.Add(cardUI);
                    Card card = mainPlayer.OwnedCards[k + round];
                    cardUI.Set(cardElementsHolder.cardShapeSprites[(int)card.Shape].Sprites[(int)card.Rank], card, (card) =>
                    {
                        mainPlayer.ChooseCard(card);
                        MainPlayerCard(cardUI);
                    });
                    cardUI.SetInteractable(false);
                }
                organizeCards?.Invoke();
            }

            cardElementsHolder.CardsHolder[index].SetLocations(0.3f);

            if (j == 3 + startIndex)
            {
                LastCards?.Invoke();
            }

            await Task.Delay(500);
        }
    }

    private List<GameObject> InitCards(int count, PlayerCardsLayout cardsLayout, Vector3 position, GameObject Card)
    {
        List<GameObject> cards = new List<GameObject>();
        for (int j = 0; j < count; j++)
        {
            GameObject newCard = Instantiate(Card, cardsLayout.transform);
            newCard.transform.position = position;

            cards.Add(newCard);
        }

        return cards;
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

        //BalootRoundInfo info = (BalootRoundInfo)trickInfo;
        bool firstHand = trickInfo.CardsOntable.Count == 0;
        print(string.Format("shape {0} first hand {1} has shape {2}", 
            trickInfo.TrickShape, firstHand, player.HasShape(trickInfo.TrickShape)));

        if (trickInfo.TrickShape != card.Shape && player.HasShape(trickInfo.TrickShape) && !firstHand)
            return false;

        return true;
    }

    internal async Task AddRemaingCards(PlayerBase mainPlayer, BalootGameType balootGameType, int startIndex)
    {
        deck[1].gameObject.SetActive(false);

        await ShuffleCards(mainPlayer, startIndex, 3, 5, () =>
        {
            if (balootGameType == BalootGameType.Hokum)
                OrganizeCards(CardHelper.HokumRank);
            else
                OrganizeCards(CardHelper.SunRank);
        }, () =>
        {
            deck[0].gameObject.SetActive(false);
        });

        //foreach (var item in playerCardsUI)
        //{
        //    item.SetOnPressed((card) =>
        //    {
        //        mainPlayer.ChooseCard(card);
        //        MainPlayerCard(item);
        //    });
        //}

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
