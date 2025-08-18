using UnityEngine;
using UnityEngine.EventSystems;

public class TeleportSpriteToCenter : MonoBehaviour, IPointerClickHandler
{
    [Tooltip("Ссылка на спрайт, который нужно телепортировать")]
    public Transform spriteToTeleport;

    [Tooltip("Смещение от центра экрана")]
    public Vector2 offset = Vector2.zero;

    // Вызывается при нажатии на UI объект
    public void OnPointerClick(PointerEventData eventData)
    {
        if (spriteToTeleport != null)
        {
            // Получаем центр экрана в мировых координатах
            Vector3 screenCenter = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
            
            // Устанавливаем позицию спрайта с учетом смещения
            spriteToTeleport.position = new Vector3(screenCenter.x + offset.x, screenCenter.y + offset.y, spriteToTeleport.position.z);
            
            Debug.Log("Спрайт телепортирован в центр экрана");
        }
        else
        {
            Debug.LogWarning("Не назначен спрайт для телепортации!");
        }
    }
}