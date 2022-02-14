using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AIPlayer : Player
{
    public AIPlayer(int index) : base(index)
    {

    }

    public override void SetTurn(TrickInfo info, int hand)
    {
        playCard(info, hand);
    }

    async void playCard(TrickInfo info, int hand)
    {
        await System.Threading.Tasks.Task.Delay(1000);

        if (hand == 0 && info.roundNumber == 0)
        {
            ChooseCard(new Card(CardShape.Club, CardRank.Two));
        }
        else
        {
            var sorted = shapeCount.OrderByDescending(a => a.Value);

            if (hand == 0)
            {

                CardShape selectedShape = sorted.ElementAt(0).Key;

                if (selectedShape == CardShape.Heart && !info.heartBroken)
                {
                    if(sorted.ElementAt(1).Value > 0)
                        selectedShape = sorted.ElementAt(1).Key;
                }
                //bug here
                Card selectedCard = OwnedCards.First(a => a.Shape == selectedShape);

                ChooseCard(selectedCard);
            }
            else
            {
                List<Card> specificShape = OwnedCards.Where(a => a.Shape == info.TrickShape).ToList();

                if (specificShape.Count > 0)
                {
                    ChooseCard(specificShape[0]);
                }
                else
                {
                    specificShape = OwnedCards.Where(a => a.Shape == CardShape.Heart).ToList();

                    if (OwnedCards.Contains(new Card(CardShape.Spade, CardRank.Queen)))
                    {
                        ChooseCard(new Card(CardShape.Spade, CardRank.Queen));
                    }
                    else if (specificShape.Count > 0)
                    {
                        ChooseCard(specificShape.Last());
                    }
                    else
                    {
                        CardShape selectedShape = sorted.ElementAt(0).Key;

                        ChooseCard(OwnedCards.Where(a => a.Shape == selectedShape).Last());
                    }
                }
            }
        }
    }

    public override void SelectPassCards()
    {
        Dictionary<Card, int> weightedCards = new Dictionary<Card, int>();


        foreach (var item in OwnedCards)
        {
            weightedCards.Add(item, (int)item.Rank);
        }

        weightedCards.OrderByDescending(a => a.Value);

        List<Card> passCards = new List<Card>();

        for (int i = 0; i < 3; i++)
        {
            passCards.Add(weightedCards.ElementAt(i).Key);
        }

        PassCards(passCards);
    }

    
}
