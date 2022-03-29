using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoublePanelScript : MonoBehaviour
{
    public GameObject QueenPanel;
    public GameObject TenPanel;
    // Start is called before the first frame update
    public void ShowPanel(Card card)
    {
        if (card.IsQueenOfSpades)
        {
            QueenPanel.SetActive(true);
        }
        else
        {
            TenPanel.SetActive(true);
        }
    }

    public void SetQueenDouble(bool value)
    {
        QueenPanel.SetActive(false);

        UIManager.Instance.SetDoubleCard(Card.QueenOfSpades, value);
    }

    public void SetTenPanel(bool value)
    {
        TenPanel.SetActive(false);

        UIManager.Instance.SetDoubleCard(Card.TenOfDiamonds, value);
    }
}
