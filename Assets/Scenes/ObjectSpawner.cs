using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ObjectSpawner : MonoBehaviour, IPointerClickHandler
{
    public GameObject objectPrefab;
    
    private void Start()
    {
        Image image = GetComponent<Image>();
        if (image != null) image.raycastTarget = true;
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        AddToSpawnQueue();
    }
    
    public void AddToSpawnQueue()
    {
        if (objectPrefab == null)
        {
            Debug.LogWarning("Не назначен префаб!");
            return;
        }
        
        if (SpawnManager.Instance == null)
        {
            Debug.LogError("SpawnManager не найден!");
            return;
        }
        
        SpawnManager.Instance.AddToSpawnQueue(objectPrefab);
        Debug.Log($"✅ Добавлен в очередь: {objectPrefab.name}");
        
        // Автоматически спавним объект сразу после добавления в очередь
        SpawnManager.Instance.SpawnAllQueuedObjects();
    }
}