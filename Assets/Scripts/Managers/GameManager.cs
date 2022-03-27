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
    public bool IsMultiGame;
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

        Currency = 1500;
        MyPlayer.Level = 2;

        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }
    // Start is called before the first frame update
    void Start()
    {
        Inventory = new List<InventoryItem>();
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


    public bool HasInInventory(string category, int index)
    {
        return Inventory.Contains(new InventoryItem(category, index));
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
    public InventoryItem(string category, int index)
    {
        Category = category;
        Index = index;
    }

    public override bool Equals(object obj)
    {
        return (Category == ((InventoryItem)obj).Category) && (Index == ((InventoryItem)obj).Index);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public string Category;
    public int Index;
}
