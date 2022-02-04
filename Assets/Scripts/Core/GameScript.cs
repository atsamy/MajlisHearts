using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScript : MonoBehaviour
{
    public DealScript Deal;
    public static GameScript Instance;

    private void Awake()
    {
        Instance = this;
        Deal = new DealScript();
    }

    void Start()
    {
        Deal.OnDealFinished += Deal_OnDealFinished;

        StartGame();
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
