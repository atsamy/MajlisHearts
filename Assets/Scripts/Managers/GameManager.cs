using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.Playables;
#if UNITY_ANDROID
using Unity.Notifications.Android;
#elif UNITY_IOS
using Unity.Notifications.iOS;
#endif
using NiobiumStudios;

[DefaultExecutionOrder(-2)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public event Action<int> OnCoinsChanged;
    public event Action<int> OnGemsChanged;

    [HideInInspector]
    public GameType GameType;
    [HideInInspector]
    public Game Game;
    [HideInInspector]
    public int Bet;
    [HideInInspector]
    public bool IsTeam;
    [HideInInspector]
    public int Coins;
    [HideInInspector]
    public int Gems;
    [HideInInspector]
    public List<InventoryItem> Inventory;

    [HideInInspector]
    public GameData GameData;

    [HideInInspector]
    public PlayerInfo MyPlayer;
    [HideInInspector]
    public Dictionary<string, List<CatalogueItem>> Catalog;
    [HideInInspector]
    public Dictionary<string, string> EquippedItem;
    internal bool IsTutorialDone;
    public const float LevelFactor = 1000f;

    void Awake()
    {
        if (!Instance)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else
            Destroy(gameObject);

        MyPlayer = new PlayerInfo();

        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        EquippedItem = new Dictionary<string, string>();
    }

    public void DeductCoins(int value)
    {
        Coins -= value;
        OnCoinsChanged?.Invoke(Coins);
        PlayfabManager.instance.DeductCurrency("SC", value, (result) =>
        {

        });
    }

    public void AddCoins(int value)
    {
        Coins += value;
        OnCoinsChanged?.Invoke(Coins);

        PlayfabManager.instance.AddCurrency("SC", value, (result) =>
        {
            print("add coins: " + result);
        });
    }

    public void DeductGems(int value)
    {
        Gems -= value;
        OnGemsChanged?.Invoke(Gems);
        PlayfabManager.instance.DeductCurrency("HC", value, (result) =>
        {

        });
    }

    public void AddGems(int value)
    {
        Gems += value;
        OnGemsChanged?.Invoke(Gems);
        PlayfabManager.instance.AddCurrency("HC", value, (result) =>
        {
            print("add gems: " + result);
        });
    }

    public bool HasInInventory(string category, string id)
    {
        if (Catalog[category].First().ID == id)
            return true;

        return Inventory.Contains(new InventoryItem(category, id));
    }

    public float AddPoints(int value,int points,string game)
    {
        MyPlayer.Points += value;

        PlayfabManager.instance.SetPlayerData(new Dictionary<string, string>() {
            { "Points",MyPlayer.Points.ToString()}
        });

        if (MyPlayer.GamePoints.ContainsKey(game))
        {
            MyPlayer.GamePoints[game] += points;
        }
        else
        {
            MyPlayer.GamePoints.Add(game, points);
        }

        PlayfabManager.instance.UpdateStats(game, MyPlayer.GamePoints[game], (result) =>
        {

        });

        return value / LevelFactor;
    }

    public int GetRewardAndSave(int rank)
    {
        int reward = GetReward(rank);
        AddCoins(reward);

        return reward;
    }

    public int GetGemsAndSave(int rank)
    {
        int gems = GetGems(rank);
        AddGems(gems);

        return gems;
    }

    int GetReward(int rank) => rank switch
    {
        0 => GameManager.Instance.Bet * 2,
        1 => GameManager.Instance.Bet * (GameManager.Instance.IsTeam ? 2 : 1),
        _ => 0
    };

    int GetGems(int rank) => rank switch
    {
        0 => 12,
        1 => 10,
        2 => 8,
        _ => 6
    };

    public void EquipItem(string itemName, string itemID)
    {
        PlayfabManager.instance.SetPlayerData(new Dictionary<string, string>()
        {
            { itemName, itemID }
        });

        if (EquippedItem.ContainsKey(itemName))
        {
            EquippedItem[itemName] = itemID;
        }
        else
        {
            EquippedItem.Add(itemName, itemID);
        }
    }

    internal void SaveAvatar(string avatar)
    {
        PlayfabManager.instance.SetAvatar(avatar);
        MyPlayer.Avatar = avatar;
    }

    public void OnApplicationFocus(bool focus)
    {
#if UNITY_ANDROID
        if (!focus)
        {
            AndroidNotificationChannel notificationChannel = new AndroidNotificationChannel()
            {
                Id = "channel_id",
                Name = "Default Channel",
                Importance = Importance.High,
                Description = "Generic notifications",
            };
            AndroidNotificationCenter.RegisterNotificationChannel(notificationChannel);



            TimeSpan timeSpan = DailyRewards.instance.GetTimeDifference();

            if (timeSpan.TotalSeconds > 0)
            {
                AndroidNotification notification = new AndroidNotification
                {
                    Title = "Daily Rewards Ready!",
                    Text = "Don't forget to collect your daily reward and enjoy playing Majlismania!",
                    FireTime = DateTime.Now.Add(timeSpan)
                };

                AndroidNotificationCenter.SendNotification(notification, "channel_id");
            }
        }
        else
        {
            AndroidNotificationCenter.CancelAllNotifications();
        }
#endif
    }

    internal void SetMajlisName(string text)
    {
        MyPlayer.MajlisName = text;

        PlayfabManager.instance.SetPlayerData(new Dictionary<string, string>() { { "majlisName", text } });
    }
}

[System.Serializable]
public class PlayerInfo
{
    public string Name;
    public string MajlisName;
    public string Avatar;
    public int Points;

    public Dictionary<string, int> GamePoints;

    public float CurrentPogress { get { return (float)(Points % GameManager.LevelFactor) / GameManager.LevelFactor; } }

    public int LevelPoints { get { return (int)GameManager.LevelFactor * (Level + 1); } }

    public int Level
    {
        get { return Mathf.FloorToInt(Points / GameManager.LevelFactor) + 1; }
    }

    public PlayerInfo()
    {
        GamePoints = new Dictionary<string, int>();
    }
}

[System.Serializable]
public class InventoryItem
{
    public string Category;
    public string ID;

    public InventoryItem(string category, string id)
    {
        Category = category;
        ID = id;
    }

    public override bool Equals(object obj)
    {
        return (Category == ((InventoryItem)obj).Category) && (ID == ((InventoryItem)obj).ID);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}

[System.Serializable]
public class GameData
{
    public int TargetScore;
    public int[] EntryFee;
}


public enum GameType
{
    Single,
    Fake,
    Online,
    Friends
}

public enum Game
{
    Hearts = 0,
    Baloot = 1
}
