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
        //Deal.StartNewGame();
    }

    public void AddPlayer(int index, Player player)
    {
        Deal.Players[index] = player;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            List<Card> cards = new List<Card>();

            for (int i = 0; i < 4; i++)
            {
                foreach (var item in Deal.Players[i].OwnedCards)
                {
                    if (!cards.Contains(item))
                        cards.Add(item);
                    else
                    {
                        Debug.Log("dublicate card: " + item.ToString());
                    }
                }
            }
        }
    }

    public void StartGame()
    {
        Deal.StartDeal();
    }

    GameState GameState;
}
