using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class GameSelector : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        if (GameManager.Instance.GameType == GameType.Friends || GameManager.Instance.GameType == GameType.Online)
        {
            gameObject.AddComponent<MultiGameScript>().enabled = true;
        }
        else
        {
            gameObject.AddComponent<GameScript>().enabled = true;
        }
    }
}
