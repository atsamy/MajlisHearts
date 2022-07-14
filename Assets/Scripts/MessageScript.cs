using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MessageScript : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI playerName;
    [SerializeField]
    Text message;
    [SerializeField]
    Image avatar;

    public void Set(string name,string message)
    {
        playerName.text = name;
        this.message.text = message;
        this.avatar.sprite = AvatarManager.Instance.GetPlayerAvatar(name);
    }
}
