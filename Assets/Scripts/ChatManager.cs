using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Chat.Demo;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatManager : MonoBehaviour, IChatClientListener
{
    public static ChatManager Instance;
    ChatClient chatClient;

    string currentChannel;

    public delegate void PlayerStatusUpdate(string user, int status);
    public static event PlayerStatusUpdate OnPlayerStatusUpdate;


    string[] friendIDs;

    public delegate void GotMessage(string sender, string message);
    public static event GotMessage OnGotMessage;

    public delegate void GotPrivateMessage(string sender, string message);
    public static event GotPrivateMessage OnGotPrivateMessage;


    private void Awake()
    {
        Instance = this;
    }

    //public void Connect()
    //{
    //    chatClient.ConnectUsingSettings();
    //}

    void Update()
    {
        if (this.chatClient != null)
        {
            this.chatClient.Service();
        }
    }

    public void DebugReturn(DebugLevel level, string message)
    {

    }

    public void AddFriends(string[] IDs)
    {
        AuthenticationValues auth = new AuthenticationValues(GameManager.Instance.MyPlayer.Name);
        chatClient = new ChatClient(this);
        chatClient.AuthValues = auth;

        ChatAppSettings chatSettings = PhotonNetwork.PhotonServerSettings.AppSettings.GetChatSettings();
        chatClient.ConnectUsingSettings(chatSettings);

        friendIDs = IDs;
    }

    public void SendPrivateMessage(string player, string message)
    {
        chatClient.SendPrivateMessage(player, message);
    }

    //public void SendMessage()
    //{

    //}

    public void SubscribeToChannel(string channel)
    {
        chatClient.Subscribe(new string[] { channel });
    }

    public void SendPublicMessage(string message)
    {
        chatClient.PublishMessage(currentChannel, message);
    }

    public void OnChatStateChange(ChatState state)
    {

    }

    public void OnConnected()
    {
        Debug.Log("chat client connected");

        chatClient.SetOnlineStatus(ChatUserStatus.Online);
        chatClient.AddFriends(friendIDs);
    }

    public void OnDisconnected()
    {

    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        OnGotMessage?.Invoke(senders[0], messages[0].ToString());
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        OnGotPrivateMessage?.Invoke(sender, message.ToString());
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        OnPlayerStatusUpdate?.Invoke(user, status);
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        Debug.Log("Subscribed to " + channels[0]);
        currentChannel = channels[0];
    }

    public void OnUnsubscribed(string[] channels)
    {

    }

    public void OnUserSubscribed(string channel, string user)
    {

    }

    public void OnUserUnsubscribed(string channel, string user)
    {

    }
}
