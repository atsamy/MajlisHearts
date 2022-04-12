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

    public delegate void PlayerStatusUpdate(string user, int status);
    public static event PlayerStatusUpdate OnPlayerStatusUpdate;

    private void Awake()
    {
        Instance = this;
    }

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

        chatClient.AddFriends(IDs);
    }

    public void SendPrivateMessage(string player,string message)
    {
        chatClient.SendPrivateMessage(player, message);
    }

    public void OnChatStateChange(ChatState state)
    {

    }

    public void OnConnected()
    {
        chatClient.SetOnlineStatus(ChatUserStatus.Online);
    }

    public void OnDisconnected()
    {

    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {

    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {

    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        OnPlayerStatusUpdate?.Invoke(user,status);
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {

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
