using UnityEngine;
using UnityEngine.UI; // Добавьте эту строку

public class InventoryUI : MonoBehaviour
{
    public Transform inventoryContent;
    public GameObject inventoryItemPrefab;

    void Start()
    {
        RefreshInventory();
    }

    public void RefreshInventory()
    {
        foreach (Transform child in inventoryContent)
        {
            Destroy(child.gameObject);
        }

        foreach (var item in InventorySystem.Instance.GetPlayerInventory())
        {
            GameObject newItem = Instantiate(inventoryItemPrefab, inventoryContent);
            newItem.GetComponent<Image>().sprite = item.itemIcon; // Теперь Image будет распознаваться
        }
    }
}