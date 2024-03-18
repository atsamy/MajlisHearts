using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class AIPlayerBaloot : PlayerBaloot, IMergePlayer
{
    public bool FakePlayer { private get; set; }
    bool cancelDouble;

    BalootGameType balootGameType;
    Dictionary<CardRank, int> rankAdjust => (balootGameType == BalootGameType.Hokum) ? CardHelper.HokumRank : CardHelper.SunRank;


    float riskTaking;
    public AIPlayerBaloot(int index) : base(index)
    {
        isPlayer = false;
        riskTaking = Random.value;
    }

    public override void SetTurn(RoundInfo info)
    {
        base.SetTurn(info);
        playCard(info);
    }

    public override async void CheckDouble(int value)
    {
        await Task.Delay(2000);
        //somehow here leads to a case of double deal continue
        if (!cancelDouble)
            SelectDouble(false, value);
    }

    public void Merge(PlayerBase player)
    {
        PlayerBaloot oldPlayer = (PlayerBaloot)player;

        OwnedCards = oldPlayer.OwnedCards;
        Score = oldPlayer.Score;
        TotalScore = oldPlayer.TotalScore;
        shapeCount = oldPlayer.ShapeCount;
        Name = oldPlayer.Name;

        //OnDoubleCard = oldPlayer.OnDoubleCard;
        OnCardReady = oldPlayer.OnCardReady;
        //OnPassCardsReady = oldPlayer.OnPassCardsReady;
    }

    public override void ChooseProjects(BalootGameType type)
    {
        if (type != BalootGameType.Hokum)
        {
            CheckFourhundredProject();
        }

        CheckOneHundredProject(type);

        CheckSequenceProject(Projects.OneHundred);
        CheckSequenceProject(Projects.Fifty);
        CheckSequenceProject(Projects.Fifty);
        CheckSequenceProject(Projects.Sira);
        CheckSequenceProject(Projects.Sira);
    }

    public override async void CheckGameType(RoundScriptBaloot roundScript)
    {
        await Task.Delay(1000);
        //SelectType(BalootGameType.Pass);
        //return;
        List<Card> allcards = new List<Card>(OwnedCards)
        {
            roundScript.BalootCard
        };

        int sunScore = SunsScore(allcards);

        if (roundScript.BiddingRound == 0)
        {
            if (roundScript.HokumIndex == -1)
            {
                int hokumScore = HokumScore(allcards, roundScript.BalootCard.Shape);

                if (sunScore >= 30)
                {
                    SelectType(BalootGameType.Sun);
                }
                else if (hokumScore >= 30)
                {
                    SelectType(BalootGameType.Hokum);
                }
                else
                {
                    SelectType(BalootGameType.Pass);
                }
            }
            else if (roundScript.HokumIndex != index)
            {
                if (sunScore >= 25 && Random.value > riskTaking)
                {
                    SelectType(BalootGameType.Sun);
                }
                else
                {
                    SelectType(BalootGameType.Pass);
                }
            }
            else
            {
                SelectType(BalootGameType.Hokum);
            }
        }
        else
        {
            if (roundScript.HokumIndex == -1)
            {
                CardShape cardShape;
                int hokumScore = OtherHokumScore(allcards, out cardShape);
                
                ChangedHokumShape(cardShape);

                if (sunScore >= 30)
                {
                    SelectType(BalootGameType.Sun);
                }
                else if (hokumScore >= 30)
                {
                    roundScript.balootRoundInfo.HokumShape = cardShape;
                    Debug.Log(cardShape);
                    SelectType(BalootGameType.Hokum);
                }
                else
                {
                    SelectType(BalootGameType.Pass);
                }
            }
            else if (roundScript.HokumIndex != index)
            {
                SelectType(BalootGameType.Pass);
            }
            else
            {
                SelectType(BalootGameType.Hokum);
            }
        }
    }

    int SunsScore(List<Card> allCards)
    {
        int score = 0;

        for (int i = 0; i < 4; i++)
        {
            List<Card> shapeCard = allCards.Where(a => a.Shape == (CardShape)i).OrderByDescending(
                a => CardHelper.SunRank[a.Rank]).ToList();

            foreach (var item in shapeCard)
            {
                int value = CardValue(item, new List<Card>());

                score += value == 0 ? 10 : 0;
                score += (value == 1 && shapeCard.Count > 1) ? 5 : 0;
                score += (value == 2 && shapeCard.Count > 2) ? 5 : 0;
            }
        }

        //maximum score 90
        //average win 30

        return score;
    }

    public int HokumScore(List<Card> allCards, CardShape shape)
    {
        int score = 0;

        for (int i = 0; i < 4; i++)
        {
            List<Card> shapeCard = allCards.Where(a => a.Shape == (CardShape)i).OrderByDescending(
                a => CardHelper.HokumRank[a.Rank]).ToList();

            if ((CardShape)i == shape)
            {
                // add score depends on how many cards i have
                score += shapeCard.Count > 2 ? (shapeCard.Count) * 5 : 0;

                foreach (var item in shapeCard)
                {
                    int value = CardValue(item, new List<Card>());
                    score += value == 0 ? 10 : 0;
                    score += (value == 1 && shapeCard.Count > 1) ? 5 : 0;
                    score += (value == 2 && shapeCard.Count > 2) ? 5 : 0;
                }
            }
            else
            {
                foreach (var item in shapeCard)
                {
                    int value = CardValue(item, new List<Card>());
                    score += value == 0 ? 5 : 0;
                }
            }
        }
        //maximum score 90
        //average win 30
        return score;
    }

    public int OtherHokumScore(List<Card> allCards,CardShape balootCard, out CardShape shape)
    {
        int score = 0;
        int bestScore = 0;

        shape = CardShape.Spade;

        for (int i = 0; i < 4; i++)
        {
            if (balootCard == (CardShape)i)
                continue;

            List<Card> shapeCard = allCards.Where(a => a.Shape == (CardShape)i).OrderByDescending(
                a => CardHelper.HokumRank[a.Rank]).ToList();


            foreach (var item in shapeCard)
            {
                int value = CardValue(item, new List<Card>());
                score += value == 0 ? 10 : value;
            }

            if (score > bestScore)
            {
                bestScore = score;
                shape = (CardShape)i;
            }
        }

        return bestScore;
    }

    internal override void CancelDouble()
    {
        cancelDouble = true;
    }

    async void playCard(RoundInfo info)
    {
        if (!FakePlayer)
            await Task.Delay(1000);

        int hand = info.CardsOntable.Count;
        BalootRoundInfo roundInfo = info as BalootRoundInfo;

        shapeCount = shapeCount.OrderByDescending(a => a.Value).ToDictionary(x => x.Key, x => x.Value);

        if (FakePlayer)
            await Task.Delay(Mathf.Max(800, OwnedCards.Count * Random.Range(250, 300)));

        if (hand == 0)
        {
            ChooseCard(ChooseWinCard(roundInfo, OwnedCards));
        }
        else
        {
            List<Card> specificShape = OwnedCards.Where(a => a.Shape == info.TrickShape).OrderBy(a => a.Rank).ToList();

            switch (hand)
            {
                case 1:
                    ChooseCard(ChooseWinCard(roundInfo, specificShape));
                    break;
                case 2:
                    //check if my team player card is winning or not
                    //then decide if i should try to win
                    if (isTeamPlayerWinning(roundInfo))
                    {
                        ChooseCard(ChooseLoseCard(roundInfo, specificShape));
                    }
                    else
                    {
                        ChooseCard(ChooseWinCard(roundInfo, specificShape));
                    }
                    break;
                case 3:
                    //check if my team player card is winning or not
                    //then find the least winnig card if available
                    if (isTeamPlayerWinning(roundInfo))
                    {
                        //Debug.Log("Team player winning choose lose card");
                        ChooseCard(ChooseLoseCard(roundInfo, specificShape));
                    }
                    else
                    {
                        //Debug.Log("Team player not winning choose win card");
                        ChooseCard(ChooseLeastWinCard(roundInfo, specificShape));
                    }
                    break;
            }
        }
    }

    //revise please
    public Card ChooseWinCard(BalootRoundInfo info, List<Card> shapeCards)
    {
        Dictionary<Card, int> AllCards = new Dictionary<Card, int>();
        //Card choosenCard;

        if (balootGameType == BalootGameType.Hokum)
        {
            if (shapeCards.Count > 0)
            {
                foreach (var item in shapeCards)
                {
                    int value = CardValue(item, info.CardsDrawn);

                    if (item.Shape == info.HokumShape)
                    {
                        if (value != 0)
                        {
                            value = (100 - value);
                        }
                    }
                    else
                    {
                        int shapeCardsOutside = 8 - (info.CardsDrawn.Count(a => a.Shape == item.Shape) + ShapeCount[item.Shape]);
                        int hokumCardsOutside = 8 - (info.CardsDrawn.Count(a => a.Shape == info.HokumShape) + ShapeCount[info.HokumShape]);

                        if (value == 0)
                        {
                            value = Mathf.Max(0, value + hokumCardsOutside - shapeCardsOutside);
                        }
                        else
                        {
                            value = (50 - value);
                        }
                    }

                    AllCards.Add(item, value);
                }

                AllCards = AllCards.OrderBy(a => a.Value).ToDictionary(x => x.Key, x => x.Value);
                return AllCards.First().Key;
            }
            else
            {
                if (info.TrickShape != info.HokumShape)
                {
                    if (shapeCount[info.HokumShape] > 0)
                    {
                        List<Card> hokumCards = OwnedCards.Where(a => a.Shape == info.HokumShape).OrderBy(a => CardHelper.HokumRank[a.Rank]).ToList();

                        if (info.CardsOntable.Any(a => a.Shape == info.HokumShape))
                        {
                            Card hokumCard = info.CardsOntable.Find(a => a.Shape == info.HokumShape);

                            foreach (var item in hokumCards)
                            {
                                if (CardHelper.HokumRank[item.Rank] > CardHelper.HokumRank[hokumCard.Rank])
                                {
                                    return item;
                                }
                            }

                            return ChooseLoseCard(info, shapeCards);
                        }

                        return hokumCards.First();
                    }
                    return ChooseLoseCard(info, shapeCards);
                }
                return ChooseLoseCard(info, shapeCards);
            }
            //if not win lose
        }
        else
        {
            if (shapeCards.Count > 0)
            {
                foreach (var item in shapeCards)
                {
                    // check bug here
                    AllCards.Add(item, CardValue(item, info.CardsDrawn));
                }

                AllCards = AllCards.OrderBy(a => a.Value).ToDictionary(x => x.Key, x => x.Value);
                //index error
                if (AllCards.ElementAt(0).Value != 0)
                {
                    return AllCards.Last().Key;
                }
                else
                {
                    return AllCards.First().Key;
                }
            }
            else
            {
                return ChooseLoseCard(info, shapeCards);
            }
        }
    }

    public Card ChooseLoseCard(BalootRoundInfo info, List<Card> shapeCards)
    {
        //bug here owned cards 
        if (shapeCards.Count > 0)
        {
            return shapeCards.OrderBy(a => rankAdjust[a.Rank]).ToList().First();
        }
        else
        {
            Dictionary<Card, int> allCardsValues = new Dictionary<Card, int>();
            Debug.Log("owned cards" + OwnedCards.Count);
            foreach (var item in OwnedCards)
            {
                if (balootGameType == BalootGameType.Hokum)
                {
                    if (item.Shape == info.HokumShape)
                    {
                        allCardsValues.Add(item, rankAdjust[item.Rank] + 100);
                    }
                    else
                    {
                        allCardsValues.Add(item, rankAdjust[item.Rank]);
                    }
                }
                else
                {
                    allCardsValues.Add(item, rankAdjust[item.Rank]);
                }
            }
            return allCardsValues.OrderBy(a => a.Value).First().Key;
        }
    }

    public Card ChooseLeastWinCard(BalootRoundInfo info, List<Card> shapeCards)
    {
        if (shapeCards.Count > 0)
        {
            List<Card> cards = shapeCards.OrderBy(a => rankAdjust[a.Rank]).ToList();

            Card winningCard = info.CardsOntable[0];

            for (int i = 1; i < info.CardsOntable.Count; i++)
            {
                if (balootGameType == BalootGameType.Hokum && info.CardsOntable[i].Shape == info.HokumShape)
                {
                    if (winningCard.Shape == info.HokumShape)
                    {
                        if (info.CardsOntable[i].Rank > winningCard.Rank)
                        {
                            winningCard = info.CardsOntable[i];
                        }
                    }
                    else
                    {
                        winningCard = info.CardsOntable[i];
                    }
                }
                else if (info.CardsOntable[i].Shape == info.TrickShape && rankAdjust[info.CardsOntable[i].Rank] > rankAdjust[winningCard.Rank])
                {
                    winningCard = info.CardsOntable[i];
                }
            }

            foreach (var item in cards)
            {
                if (rankAdjust[item.Rank] > rankAdjust[winningCard.Rank])
                {
                    return item;
                }
            }

            return cards.First();
        }
        else
        {
            if (balootGameType == BalootGameType.Hokum && ShapeCount[info.HokumShape] > 0)
            {
                return OwnedCards.Where(a => a.Shape == info.HokumShape).OrderBy(a => rankAdjust[a.Rank]).First();
            }
            else
            {
                // instead of using the weakest card the player can also get rid of cards
                // from shape with least cards if the player
                return OwnedCards.OrderBy(a => rankAdjust[a.Rank]).First();
            }
        }
    }

    /// <summary>
    /// Return an int number that determine the probability of the card winning 0 means that it will sure win
    /// </summary>
    /// <param name="card"></param>
    /// <param name="balootInfo"></param>
    /// <returns></returns>
    public int CardValue(Card card, List<Card> CardsDrawn)
    {
        int value = 0;

        for (int i = rankAdjust[card.Rank] + 1; i <= rankAdjust[CardRank.Ace]; i++)
        {
            Card higherCard = new Card(card.Shape, CardHelper.SunRank.FirstOrDefault(x => x.Value == i).Key);

            if (OwnedCards.Contains(higherCard) || CardsDrawn.Contains(higherCard))
            {
                continue;
            }
            else
            {
                value++;
            }
        }

        return value;
    }

    bool isTeamPlayerWinning(BalootRoundInfo info)
    {
        Card teamPlayerCard = info.CardsOntable[info.CardsOntable.Count - 2];

        if (balootGameType == BalootGameType.Hokum)
        {
            if (teamPlayerCard.Shape != info.TrickShape && teamPlayerCard.Shape != info.HokumShape)
            {
                return false;
            }
            else if (teamPlayerCard.Shape == info.HokumShape)
            {
                foreach (var item in info.CardsOntable)
                {
                    if (CardHelper.HokumRank[item.Rank] > CardHelper.HokumRank[teamPlayerCard.Rank] && item.Shape == info.HokumShape)
                        return false;
                }
            }
            else
            {
                foreach (var item in info.CardsOntable)
                {
                    if (CardHelper.HokumRank[item.Rank] > CardHelper.HokumRank[teamPlayerCard.Rank] && item.Shape == info.TrickShape)
                        return false;
                    else if (item.Shape == info.HokumShape)
                        return false;
                }
            }
            return true;
        }
        else
        {
            if (teamPlayerCard.Shape != info.TrickShape)
                return false;
            else
            {
                foreach (var item in info.CardsOntable)
                {
                    if (CardHelper.SunRank[item.Rank] > CardHelper.SunRank[teamPlayerCard.Rank] && item.Shape == info.TrickShape)
                        return false;
                }
            }

            return true;
        }
    }

    public override void Reset()
    {
        cancelDouble = false;
        base.Reset();
    }

    internal void SetGameType(BalootGameType roundType)
    {
        balootGameType = roundType;
    }
}

public interface IMergePlayer
{
    public void Merge(PlayerBase player)
    {
        
    }
}
