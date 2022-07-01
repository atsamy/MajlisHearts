using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MessageScript : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI name;
    [SerializeField]
    Text message;
    [SerializeField]
    Image avatar;

    public void Set(string name,string message,string avatar)
    {
        this.name.text = name;
        this.message.text = message;
        //implement avatar manager
    }
}
