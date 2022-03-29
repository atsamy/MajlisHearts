using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugCards : MonoBehaviour
{
    public GameObject Text;
    Dictionary<Card, Text> AllCards;

    public void AddCards(List<Card> cards)
    {
        AllCards = new Dictionary<Card, Text>();

        foreach (var item in cards)
        {
            Text text = Instantiate(Text, transform).GetComponent<Text>();
            text.text = item.ToString();
            AllCards.Add(item, text);
        }
    }

    public void UpdateCards(List<Card> cards)
    {
        //AllCards = new Dictionary<Card, Text>();

        for (int i = 0; i < cards.Count; i++)
        {
            transform.GetChild(i).GetComponent<Text>().text = cards[i].ToString();
        }
    }

    public void ShowWeight(Card card, int Weight)
    {
        AllCards[card].text = card.ToString() + " " + Weight;
    }
}
