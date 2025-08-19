using UnityEngine;
using UnityEngine.UI;

public class InventoryItemUI : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    [SerializeField] private Text itemNameText;

    public void Setup(InventorySystem.Item item)
    {
        itemImage.sprite = item.itemIcon;
        itemNameText.text = item.itemName;
    }
}