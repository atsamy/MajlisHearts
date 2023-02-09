using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardElementsHolder : MonoBehaviour
{
    public int Spacing = 40;
    public GameObject playerCard;

    public PlayerCardsLayout[] CardsHolder;
    public CardShapeSprites[] cardShapeSprites;

    public Vector2 CardsStartResolution = new Vector2(209, 304);
    public Vector2 CardsOnTableResolution = new Vector2(143, 208);

    public Transform[] DeckCardsPosition;

    public PlayerDetails[] playersDetails;
}
