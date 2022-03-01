using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSelector : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.Instance.IsMultiGame)
        {
            GetComponent<MultiGameScript>().enabled = true;
        }
        else
        {
            GetComponent<GameScript>().enabled = true;
        }
    }
}
