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
    public bool IsMultiGame;
    [HideInInspector]
    public int Currency;
    [HideInInspector]
    public List<InventoryItem> Inventory;
    [HideInInspector]
    public PlayerInfo MyPlayer;

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

        //Currency = 1500;
        //MyPlayer.Level = 2;

        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }
    // Start is called before the first frame update
    void Start()
    {
        //Inventory = new List<InventoryItem>();
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
        return Inventory.Contains(new InventoryItem(category, id));
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

        PlayfabManager.instance.SetPlayerData(new Dictionary<string, string>()
        {
            { "Customization", JsonUtility.ToJson(Customization) }
        });
    }
}

[System.Serializable]
public class PlayerInfo
{
    public string Name;
    public int Level;
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