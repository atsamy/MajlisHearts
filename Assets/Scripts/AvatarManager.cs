using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarManager : MonoBehaviour
{
    [HideInInspector]
    public Sprite playerAvatar;

    public Sprite RobotAvatar;

    public static AvatarManager Instance;
    Dictionary<string, Sprite> savedPlayers;

    private void Awake()
    {
        Instance = this;
        savedPlayers = new Dictionary<string, Sprite>();
    }

    public void SetPlayerAvatar(string name)
    {
        playerAvatar = Resources.Load<Sprite>("Avatar/" + name);
    }

    public Sprite GetAvatarSprite(string name)
    {
        return Resources.Load<Sprite>("Avatar/" + name);
    }

    public void SetPlayerAvatar(string name,string avatar)
    {
        if (savedPlayers.ContainsKey(name))
            return;

        Sprite sprite = Resources.Load<Sprite>("Avatar/" + avatar);
        savedPlayers.Add(name, sprite);
    }

    public Sprite GetPlayerAvatar(string name)
    {
        if (name == GameManager.Instance.MyPlayer.Name)
            return playerAvatar;

        if (savedPlayers.ContainsKey(name))
            return savedPlayers[name];
        else if (name.Contains("AI"))
            return RobotAvatar;
        else
            return null;
    }
}
