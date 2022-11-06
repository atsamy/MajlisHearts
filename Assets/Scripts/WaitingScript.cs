using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaitingScript : MonoBehaviour
{
    [SerializeField]
    Image loadingBar;

    [SerializeField]
    Image shapeImage;

    [SerializeField]
    Sprite[] shapeSprites;

    int direction = -1;

    public void Show()
    {
        if (GameManager.Instance.GameType == GameType.Single)
            return;

        gameObject.SetActive(true);
        shapeImage.sprite = shapeSprites[Random.Range(0, shapeSprites.Length)];
    }

    public void Hide()
    {
        if (GameManager.Instance.GameType == GameType.Single)
            return;

        gameObject.SetActive(false);
    }

    void Update()
    {
        loadingBar.fillAmount += direction * Time.deltaTime * 0.5f;

        if ((loadingBar.fillAmount <= 0 && direction == -1) || (loadingBar.fillAmount >= 1 && direction == 1))
        {
            direction *= -1;
            loadingBar.fillClockwise = !loadingBar.fillClockwise;
        }
    }
}
