using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Unity.Notifications.Android;
using NiobiumStudios;

[DefaultExecutionOrder(-2)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public event Action<int> OnCurrencyChanged;

    //[HideInInspector]
    //public List<InventoryItem> Customization;
    [HideInInspector]
    public GameType GameType;
    [HideInInspector]
    public int Bet;
    [HideInInspector]
    public bool IsTeam;
    [HideInInspector]
    public int Currency;
    [HideInInspector]
    public List<InventoryItem> Inventory;

    [HideInInspector]
    public PlayerInfo MyPlayer;
    [HideInInspector]
    public Dictionary<string, List<CatalogueItem>> Catalog;
    [HideInInspector]
    public Dictionary<string, string> EquippedItem;

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

    public void DeductCurrency(int value)
    {
        Currency -= value;
        OnCurrencyChanged?.Invoke(Currency);
        PlayfabManager.instance.DeductCurrency(value,(result)=>
        {

        });
    }

    public void AddCurrency(int value)
    {
        Currency += value;
        OnCurrencyChanged?.Invoke(Currency);
    }

    public bool HasInInventory(string category, string id)
    {
        if (Catalog[category].First().ID == id)
            return true;

        return Inventory.Contains(new InventoryItem(category, id));
    }

    public float AddPoints(int value)
    {
        //int Level = MyPlayer.Level;
        //MyPlayer.Points += value;
        //int newLevel = MyPlayer.Level;

        PlayfabManager.instance.SetPlayerData(new Dictionary<string, string>() {
            { "Points",MyPlayer.Points.ToString()}
        });

        //progress = ;

        return (float)value / LevelFactor;
    }

    public int GetRewardAndSave(int rank)
    {
        int reward = GetReward(rank);
        AddCurrency(reward);
        PlayfabManager.instance.AddCurrency(reward, null);

        return reward;
    }

    int GetReward(int rank) => rank switch
    {
        0 => GameManager.Instance.Bet * 2,
        1 => GameManager.Instance.Bet * (GameManager.Instance.IsTeam ? 2 : 1),
        _ => 0
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

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.D))
    //    {
    //        TimeSpan timeSpan = DailyRewards.instance.GetTimeDifference();

    //        print(timeSpan.TotalSeconds);
    //    }
    //}

    public void OnApplicationFocus(bool focus)
    {
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

            //AndroidNotification testnotification = new AndroidNotification
            //{
            //    Title = "Testing!",
            //    Text = "Testing test testing the test!",
            //    FireTime = DateTime.Now.AddSeconds(10)
            //};

            //AndroidNotificationCenter.SendNotification(testnotification, "channel_id");
        }
        else
        {
            AndroidNotificationCenter.CancelAllNotifications();
        }
    }
}

[System.Serializable]
public class PlayerInfo
{
    public string Name;
    public string Avatar;
    public int Points;

    public float CurrentPogress { get { return (float)(Points % GameManager.LevelFactor) / GameManager.LevelFactor; } }

    public int LevelPoints { get { return (int)GameManager.LevelFactor * (Level + 1); } }

    public int Level
    {
        get { return Mathf.FloorToInt(Points / GameManager.LevelFactor) + 1; }
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

public enum GameType
{
    Single,
    Fake,
    Online,
    Friends
}
