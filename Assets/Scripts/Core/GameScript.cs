//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScript : MonoBehaviour
{
    public Player[] Players;
    // Start is called before the first frame update
    void Start()
    {
        Players = new Player[4];

        for (int i = 0; i < 4; i++)
        {
            Players[i] = new Player();
        }

        Deal();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Deal()
    {
        List<Card> AllCards = GetAllCards();

        
        for (int i = 0; i < Players.Length; i++)
        {
            for (int j = 0; j < 13; j++)
            {
                int getRandom = Random.Range(0, AllCards.Count);

                Players[i].AddCard(AllCards[getRandom]);
                AllCards.RemoveAt(getRandom);
            }

            print(Players[i].OwnedCards);
        }
    }

    private List<Card> GetAllCards()
    {
        List<Card> cards = new List<Card>();

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 13; j++)
            {
                cards.Add(new Card((CardShape)i, (CardRank)j));
            }
        }

        return cards;
    }
}
