using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoublePanelScript : MonoBehaviour
{
    public GameObject QueenPanel;
    public GameObject TenPanel;

    Card selectedCard;
    // Start is called before the first frame update
    public void ShowPanel(Card card)
    {
        gameObject.SetActive(true);
        selectedCard = card;

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
        gameObject.SetActive(false);
        UIManager.Instance.SetDoubleCard(selectedCard, value);
    }
}
