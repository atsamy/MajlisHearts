using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static int[] SerializeListOfCards(List<Card> cards)
    {
        int[] cardsSerialized = new int[cards.Count * 2];

        for (int i = 0; i < cards.Count; i++)
        {
            cardsSerialized[i * 2] = (int)cards[i].Rank;
            cardsSerialized[(i * 2) + 1] = (int)cards[i].Shape;
        }

        return cardsSerialized;
    }

    public static int[] SerializePlayersProjects(PlayerBase[] players)
    {
        List<int> projectsSerialized = new List<int>();

        foreach (PlayerBaloot player in players)
        {
            if (player.PlayerProjects.Count > 0)
            {
                projectsSerialized.Add(player.Index);
                projectsSerialized.Add(player.PlayerProjects.Count);

                foreach (var project in player.PlayerProjects)
                {
                    projectsSerialized.Add((int)project.Value);
                    projectsSerialized.Add(project.Key.Count);

                    foreach (var item in project.Key)
                    {
                        projectsSerialized.Add((int)item.Rank);
                        projectsSerialized.Add((int)item.Shape);
                    }
                }
            }
        }

        return projectsSerialized.ToArray();
    }

    public static Dictionary<int, Dictionary<List<Card>, Projects>> DeserializePlayersProjects(int[] data)
    {
        Dictionary<int, Dictionary<List<Card>, Projects>> allProjects = new();
        int index = 0;

        while (index < data.Length)
        {
            Dictionary<List<Card>, Projects> projects = new();

            int playerIndex = data[index];
            int noOfProjects = data[index + 1];
            index += 2;

            for (int i = 0; i < noOfProjects; i++)
            {
                Projects currentProject = (Projects)data[i + index];
                int cardsCount = data[i + 1 + index];
                index += 2;
                List<Card> cards = new List<Card>();

                for (int j = index; j < (cardsCount * 2) + index; j += 2)
                {
                    Card card = new Card((CardShape)data[j + 1], (CardRank)data[j]);
                    cards.Add(card);
                }

                index += (cardsCount * 2);
                projects.Add(cards, currentProject);
            }

            allProjects.Add(playerIndex, projects);
        }

        return allProjects;
    }

    public static int[] SerializeProjects(Dictionary<List<Card>, Projects> projects, int power, int score)
    {
        List<int> projectsSerialized = new List<int>
        {
            power,
            score
        };

        foreach (var project in projects)
        {
            projectsSerialized.Add((int)project.Value);
            projectsSerialized.Add(project.Key.Count);

            foreach (var item in project.Key)
            {
                projectsSerialized.Add((int)item.Rank);
                projectsSerialized.Add((int)item.Shape);
            }
        }

        return projectsSerialized.ToArray();
    }

    public static Dictionary<List<Card>, Projects> DeserializeProjects(int[] data, out int power, out int score)
    {
        power = data[0];
        score = data[1];

        Dictionary<List<Card>, Projects> projects = new();

        int index = 2;

        for (int i = index; i < data.Length; i++)
        {
            Projects project = (Projects)data[i];
            int number = data[i + 1];
            index += 2;

            List<Card> cards = new List<Card>();

            for (int j = index; j < (number * 2) + index; j += 2)
            {
                Card card = new Card((CardShape)data[j + 1], (CardRank)data[j]);
                cards.Add(card);
            }

            index += (number * 2);

            projects.Add(cards, project);
        }

        return projects;
    }

    public static List<Card> DeSerializeListOfCards(int[] data)
    {
        List<Card> cards = new List<Card>();

        for (int i = 0; i < data.Length; i += 2)
        {
            Card card = new Card((CardShape)data[i + 1], (CardRank)data[i]);
            cards.Add(card);
        }

        return cards;
    }

    public static int[] SerializeCard(Card card)
    {
        int[] cardSerialized = new int[2];

        cardSerialized[0] = (int)card.Rank;
        cardSerialized[1] = (int)card.Shape;

        return cardSerialized;
    }

    public static Card DeSerializeCard(int[] data)
    {
        return new Card((CardShape)data[1], (CardRank)data[0]);
    }

    public static int[] SerializeCardAndPlayer(Card card, int playerIndex)
    {
        int[] cardSerialized = new int[3];

        cardSerialized[0] = (int)card.Rank;
        cardSerialized[1] = (int)card.Shape;
        cardSerialized[2] = playerIndex;

        return cardSerialized;
    }

    public static KeyValuePair<int, Card> DeSerializeCardAndPlayer(int[] data)
    {
        return new KeyValuePair<int, Card>(data[2], new Card((CardShape)data[1], (CardRank)data[0]));
    }

    public static int[] SerializeCardValueAndIndex(Card card, bool value, int index)
    {
        int[] cardSerialized = new int[4];

        cardSerialized[0] = (int)card.Rank;
        cardSerialized[1] = (int)card.Shape;
        cardSerialized[2] = value ? 1 : 0;
        cardSerialized[3] = index;

        return cardSerialized;
    }

    public static KeyValuePair<bool, Card> DeSerializeCardvalueAndIndex(int[] data, out int index)
    {
        index = data[3];
        return new KeyValuePair<bool, Card>((data[2] == 1), new Card((CardShape)data[1], (CardRank)data[0]));
    }
}
