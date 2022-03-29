using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject UsernamePanel;

    // Start is called before the first frame update
    void Start()
    {
        if (!PlayerPrefs.HasKey("userName"))
        {
            UsernamePanel.SetActive(true);
        }

    }

    public void StartSingleGame()
    {
        GameManager.Instance.IsMultiGame = false;
        SceneManager.LoadScene(1);
    }

    //public void StartMulti
}
