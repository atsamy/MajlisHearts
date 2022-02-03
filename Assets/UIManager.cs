using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    MainPlayer mainPlayer;
    CardsUIManager cardsUIManager;
    GameScript game;
    // Start is called before the first frame update
    void Start()
    {
        mainPlayer.OnPlayerTurn += PlayerTurn;
    }


    public void PlayerTurn(bool firstHand)
    {
        cardsUIManager.SetPlayableCards(game.Deal.TrickInfo, firstHand);
    }

    public void OnDisable()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
