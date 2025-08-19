using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; private set; }

    [System.Serializable]
    public class Item
    {
        public string itemName;
        public Sprite itemIcon;
        public int price;
        [HideInInspector] public bool purchased;
    }

    public List<Item> allItems = new List<Item>();
    private List<Item> playerInventory = new List<Item>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadInventory();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PurchaseItem(int itemIndex)
    {
        if (itemIndex < 0 || itemIndex >= allItems.Count || allItems[itemIndex].purchased) return;

        // Проверка денег (добавьте свою логику)
        allItems[itemIndex].purchased = true;
        playerInventory.Add(allItems[itemIndex]);
        SaveInventory();
    }

    public List<Item> GetPlayerInventory() => playerInventory;
    public bool HasItem(string itemName) => playerInventory.Exists(item => item.itemName == itemName);

    private void SaveInventory()
    {
        foreach (var item in allItems)
        {
            PlayerPrefs.SetInt(item.itemName + "_purchased", item.purchased ? 1 : 0);
        }
    }

    private void LoadInventory()
    {
        playerInventory.Clear();
        foreach (var item in allItems)
        {
            item.purchased = PlayerPrefs.GetInt(item.itemName + "_purchased", 0) == 1;
            if (item.purchased) playerInventory.Add(item);
        }
    }
}