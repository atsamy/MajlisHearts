using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[DefaultExecutionOrder(-2)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [HideInInspector]
    public bool IsMultiGame;

    public List<InventoryItem> Inventory;

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

        MyPlayer.Currency = 1500;
        MyPlayer.Level = 2;
    }
    // Start is called before the first frame update
    void Start()
    {
        Inventory = new List<InventoryItem>();
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
    public int Currency;
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

    public string Category;
    public int Index;
}
