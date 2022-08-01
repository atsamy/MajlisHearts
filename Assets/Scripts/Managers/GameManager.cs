using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

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

    public bool AddPoints(int value, out float progress)
    {
        int Level = MyPlayer.Level;
        MyPlayer.Points += value;
        int newLevel = MyPlayer.Level;

        PlayfabManager.instance.SetPlayerData(new Dictionary<string, string>() {
            { "Points",MyPlayer.Points.ToString()}
        });

        progress = (float)value / LevelFactor;

        return newLevel > Level;
    }

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
}

[System.Serializable]
public class PlayerInfo
{
    public string Name;
    public string Avatar;
    public int Points;

    public float CurrentPogress { get { return (float)(Points % GameManager.LevelFactor) / GameManager.LevelFactor; } }

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
