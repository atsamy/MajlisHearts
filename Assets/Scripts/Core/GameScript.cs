using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScript : MonoBehaviour
{
    public DealScript Deal;
    void Start()
    {
        Deal = new DealScript();
        Deal.OnDealFinished += Deal_OnDealFinished;
    }

    private void Deal_OnDealFinished()
    {
        
    }

    public void AddPlayer(int index,Player player)
    {
        Deal.Players[index] = player;
    }

    public void StartGame()
    {
        Deal.StartDeal();
    }

    GameState GameState;
}
