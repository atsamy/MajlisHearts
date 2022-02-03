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
            newCard.transform.localPosition = new Vector3((i - 6) * 100,0);

            PlayerCardsUI.Add(newCard.GetComponent<CardUI>());
            PlayerCardsUI.Last().Set(mainPlayer.OwnedCards[i], (card) =>
             {
                 mainPlayer.ChooseCard(card);
             });
        }
    }

    public void SetPlayableCards(TrickInfo info)
    {

    }
}

public class TrickInfo
{
    public CardShape TrickShape;
    public bool heartBroken;
    public int roundNumber;
}