using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoublePanelScript : MonoBehaviour
{
    public GameObject QueenPanel;
    public GameObject TenPanel;

    //Card selectedCard;
    // Start is called before the first frame update
    public void ShowPanel(Card card)
    {
        gameObject.SetActive(true);
        //selectedCard = card;

        if (card.IsQueenOfSpades)
        {
            QueenPanel.SetActive(true);
        }
        else
        {
            TenPanel.SetActive(true);
        }
    }

    public void SetDouble(bool value)
    {
        if (TenPanel.activeSelf)
        {
            UIManager.Instance.SetDoubleCard(Card.TenOfDiamonds, value);
            TenPanel.SetActive(false);
        }
        else if (QueenPanel.activeSelf)
        {
            UIManager.Instance.SetDoubleCard(Card.QueenOfSpades, value);
            QueenPanel.SetActive(false);
        }

        if (!TenPanel.activeSelf && !QueenPanel.activeSelf)
            gameObject.SetActive(false);
    }
}
