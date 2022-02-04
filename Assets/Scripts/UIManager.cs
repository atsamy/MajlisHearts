using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    MainPlayer mainPlayer;
    CardsUIManager cardsUIManager;
    GameScript game;
    // Start is called before the first frame update
    void Awake()
    {
        
        game = GameScript.Instance;

        game.Deal.OnCardsDealt += Deal_OnCardsDealt;

        cardsUIManager = GetComponentInChildren<CardsUIManager>();
    }

    private void Deal_OnCardsDealt()
    {
        mainPlayer = (MainPlayer)game.Deal.Players[0];
        mainPlayer.OnPlayerTurn += PlayerTurn;

        DealCards();
    }

    public void PlayerTurn(bool firstHand)
    {
        cardsUIManager.SetPlayableCards(game.Deal.TrickInfo,mainPlayer, firstHand);
    }

    public void DealCards()
    {
        cardsUIManager.ShowPlayerCards(mainPlayer);
    }

    public void OnDisable()
    {
        
    }
}
