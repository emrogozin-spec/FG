using UnityEngine;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    [SerializeField] private Text itemNameText;
    [SerializeField] private Text itemPriceText;
    [SerializeField] private Button purchaseButton;
    [SerializeField] private Text buttonText;

    public void Setup(InventorySystem.Item item, System.Action onPurchase)
    {
        itemImage.sprite = item.itemIcon;
        itemNameText.text = item.itemName;
        itemPriceText.text = "$" + item.price.ToString();

        purchaseButton.onClick.AddListener(() => onPurchase());
        UpdateButton(item.purchased);
    }

    public void UpdateButton(bool purchased)
    {
        if (purchased)
        {
            buttonText.text = "Куплено";
            purchaseButton.interactable = false;
        }
        else
        {
            buttonText.text = "Купить";
            purchaseButton.interactable = true;
        }
    }
}