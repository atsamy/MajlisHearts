using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class GameSelector : MonoBehaviour
{
    public bool isSingleBalootGame;
    // Start is called before the first frame update
    void Awake()
    {
        if (isSingleBalootGame)
        {
            
        }
        if (GameManager.Instance.Game == Game.Baloot)
        {
            if (GameManager.Instance.GameType == GameType.Friends || GameManager.Instance.GameType == GameType.Online)
            {
                gameObject.AddComponent<MultiGameBalootScript>().enabled = true;
            }
            else
            {
                gameObject.AddComponent<GameScriptBaloot>().enabled = true;
            }
        }
        else
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
}
