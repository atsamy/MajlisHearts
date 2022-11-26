using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PausePanel : MonoBehaviour
{
    [SerializeField]
    Popup confirmExitPopup;

    public void Show()
    {
        gameObject.SetActive(true);

        if(GameManager.Instance.GameType == GameType.Single)
            Time.timeScale = 0;
    }
    public void Continue()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1;
        GameSFXManager.Instance.PlayClip("Click");
    }

    public void BackToMajlis()
    {
        if (GameManager.Instance.GameType == GameType.Single)
        {
            Exit();
        }
        else
        {
            SFXManager.Instance.PlayClip("Popup");
            confirmExitPopup.ShowWithCode("confirmexitmessage", () =>
            {
                UIManager.Instance.LeaveRoom();
                Exit();
            }, () =>
            {
                Continue();
            });
        }
    }

    private void Exit()
    {
        FadeScreen.Instance.FadeIn(2, () =>
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(1);
        });

        GameSFXManager.Instance.PlayClip("Click");
    }
}
