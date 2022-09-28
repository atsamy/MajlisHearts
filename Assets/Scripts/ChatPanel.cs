using ArabicSupport;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatPanel : MonoBehaviour
{
    [SerializeField]
    InputField inputField;
    [SerializeField]
    GameObject messagePrefab;
    [SerializeField]
    Transform Content;

    bool toggleMessageColor;
    // Start is called before the first frame update
    void Start()
    {
        ChatManager.OnGotMessage += ChatManager_OnGotMessage;
    }

    private void ChatManager_OnGotMessage(string sender, string message)
    {
        if (Content.childCount == 5)
        {
            Destroy(Content.GetChild(0));
        }
        MessageScript messageObj = Instantiate(messagePrefab, Content).GetComponent<MessageScript>();
        toggleMessageColor = !toggleMessageColor;
        messageObj.Set(sender , message, toggleMessageColor);
    }

    public void SendMessage()
    {
        ChatManager.Instance.SendPublicMessage(inputField.text);
        inputField.text = "";
    }
}
