using UnityEngine;
using UnityEngine.UI; // Добавьте эту строку
using UnityEngine.EventSystems;

public class ShopManager : MonoBehaviour
{
    public Transform shopContent;
    public GameObject itemPrefab;

    void Start()
    {
        foreach (var item in InventorySystem.Instance.allItems)
        {
            GameObject newItem = Instantiate(itemPrefab, shopContent);
            newItem.GetComponent<Image>().sprite = item.itemIcon; // Теперь Image будет распознаваться

            EventTrigger trigger = newItem.GetComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerClick
            };
            entry.callback.AddListener((data) => OnItemClick(item));
            trigger.triggers.Add(entry);
        }
    }

    void OnItemClick(InventorySystem.Item item)
    {
        int index = InventorySystem.Instance.allItems.IndexOf(item);
        InventorySystem.Instance.PurchaseItem(index);
        shopContent.GetChild(index).GetComponent<Image>().color =
            item.purchased ? new Color(0.5f, 0.5f, 0.5f) : Color.white;
    }
}