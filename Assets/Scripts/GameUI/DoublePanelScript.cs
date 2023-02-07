using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoublePanelScript : MonoBehaviour
{
    public GameObject QueenPanel;
    public GameObject TenPanel;

    public void ShowPanel(Card card)
    {
        //yield return new WaitForSeconds(2f);

        gameObject.SetActive(true);

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
            HeartsUIManager.Instance.SetDoubleCard(CardHelper.TenOfDiamonds, value);
            TenPanel.SetActive(false);
        }
        else if (QueenPanel.activeSelf)
        {
            HeartsUIManager.Instance.SetDoubleCard(CardHelper.QueenOfSpades, value);
            QueenPanel.SetActive(false);
        }

        if (!TenPanel.activeSelf && !QueenPanel.activeSelf)
            gameObject.SetActive(false);

        GameSFXManager.Instance.PlayClip(value ? "Double" : "No");
    }
}
