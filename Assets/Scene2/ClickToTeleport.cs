using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class ClickToTeleport : MonoBehaviour
{
    [Header("Настройки телепортации")]
    [SerializeField] private Transform targetToTeleport; // Спрайт для телепортации
    [SerializeField] private float teleportDelay = 0.3f; // Задержка перед телепортацией
    [SerializeField] private AudioClip teleportSound; // Звук телепортации
    [SerializeField] private ParticleSystem teleportEffect; // Эффект телепортации

    [Header("Настройки центра")]
    [SerializeField] private Transform customCenter; // Опциональный кастомный центр
    [SerializeField] private float yOffset = 0f; // Смещение по Y

    private Vector3 GetCenterPosition()
    {
        if (customCenter != null)
        {
            return customCenter.position + Vector3.up * yOffset;
        }
        else
        {
            // Центр экрана в мировых координатах
            Vector3 center = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
            center.z = targetToTeleport.position.z; // Сохраняем Z-координату цели
            center.y += yOffset;
            return center;
        }
    }

    private void OnMouseDown()
    {
        if (targetToTeleport == null)
        {
            Debug.LogError("Target to teleport is not assigned!", this);
            return;
        }

        StartCoroutine(TeleportCoroutine());
    }

    private IEnumerator TeleportCoroutine()
    {
        // Воспроизводим звук
        if (teleportSound != null)
        {
            AudioSource.PlayClipAtPoint(teleportSound, Camera.main.transform.position);
        }

        // Запускаем эффект (если есть)
        if (teleportEffect != null)
        {
            teleportEffect.Play();
            yield return new WaitForSeconds(teleportEffect.main.duration * 0.5f);
        }
        else
        {
            yield return new WaitForSeconds(teleportDelay);
        }

        // Телепортируем объект
        targetToTeleport.position = GetCenterPosition();

        // Завершаем эффект
        if (teleportEffect != null)
        {
            yield return new WaitForSeconds(teleportEffect.main.duration * 0.5f);
            teleportEffect.Stop();
        }
    }

    private void OnValidate()
    {
        // Автоматически добавляем коллайдер если его нет
        if (GetComponent<Collider2D>() == null)
        {
            gameObject.AddComponent<BoxCollider2D>();
        }
        
        // Убедимся что коллайдер является триггером
        GetComponent<Collider2D>().isTrigger = true;
    }

    // Для визуализации центра в редакторе
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(GetCenterPosition(), 0.5f);
    }
}