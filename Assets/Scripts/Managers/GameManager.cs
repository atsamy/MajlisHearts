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

    [HideInInspector]
    public List<InventoryItem> Customization;
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

    public void SetCustomization(string category, string id)
    {
        if (Customization.Any(a => a.Category == category))
        {
            Customization.Find(a => a.Category == category).ID = id;
        }
        else
        {
            Customization.Add(new InventoryItem(category, id));
        }

        Wrapper<InventoryItem> wrappedCustomization = new Wrapper<InventoryItem>();
        wrappedCustomization.array = Customization.ToArray();

        PlayfabManager.instance.SetPlayerData(new Dictionary<string, string>()
        {
            { "Customization", JsonUtility.ToJson( wrappedCustomization) }
        });
    }
}

[System.Serializable]
public class PlayerInfo
{
    public string Name;
    public string Avatar;
    public int Level;
    public int Score;
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
