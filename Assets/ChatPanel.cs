using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatPanel : MonoBehaviour
{
    public InputField inputField;
    public GameObject TextPrefab;
    public Transform Content;
    // Start is called before the first frame update
    void Start()
    {
        ChatManager.OnGotMessage += ChatManager_OnGotMessage;
    }

    private void ChatManager_OnGotMessage(string sender, string message)
    {
        Text text = Instantiate(TextPrefab, Content).GetComponent<Text>();
        text.text = "<color=green>" + sender + "</color>" + " " + message;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SendMessage()
    {
        ChatManager.Instance.SendPublicMessage(inputField.text);
        inputField.text = "";
    }
}
