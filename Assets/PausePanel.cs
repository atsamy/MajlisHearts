using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PausePanel : MonoBehaviour
{
    public void Show()
    {
        gameObject.SetActive(true);
        Time.timeScale = 0;
    }
    public void Continue()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1;
    }

    public void BackToMajlis()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(1);
    }
}
