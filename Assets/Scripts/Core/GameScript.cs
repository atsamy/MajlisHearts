using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScript : MonoBehaviour
{
    public DealScript Deal;
    void Start()
    {
        deal = new DealScript();
        deal.OnDealFinished += Deal_OnDealFinished;
    }

    private void Deal_OnDealFinished()
    {
        
    }

    public void AddPlayer(int index,Player player)
    {
        deal.Players[index] = player;
    }

    public void StartGame()
    {
        deal.StartDeal();
    }

    GameState GameState;
}
