using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;
    
    [Header("Spawn Settings")]
    public string spawnPointTag = "SpawnPoint";
    public float verticalSpacing = 2f;
    public Vector3 spawnOffset = Vector3.zero;
    
    [Header("Fallback Settings")]
    public Vector3 fallbackSpawnPosition = new Vector3(0, 5, 0);
    public bool useFallbackIfNoTag = true;
    
    private Queue<GameObject> spawnQueue = new Queue<GameObject>();
    private Vector3 lastSpawnPosition;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    public void AddToSpawnQueue(GameObject prefab)
    {
        if (prefab != null)
        {
            spawnQueue.Enqueue(prefab);
            Debug.Log($"Добавлен в очередь: {prefab.name}, всего в очереди: {spawnQueue.Count}");
        }
    }
    
    public void SpawnAllQueuedObjects()
    {
        if (spawnQueue.Count == 0) return;
        
        // Получаем позицию спавна с обработкой ошибок
        Vector3 spawnPosition = GetSpawnPosition();
        lastSpawnPosition = spawnPosition;
        
        while (spawnQueue.Count > 0)
        {
            GameObject prefab = spawnQueue.Dequeue();
            SpawnSingleObject(prefab);
        }
    }
    
    private Vector3 GetSpawnPosition()
    {
        // Проверяем, установлен ли тег
        if (string.IsNullOrEmpty(spawnPointTag))
        {
            Debug.LogWarning("Тег спавн-поинта не установлен! Используем запасную позицию.");
            return fallbackSpawnPosition;
        }
        
        // Ищем спавн-поинт
        GameObject spawnPoint = GameObject.FindGameObjectWithTag(spawnPointTag);
        if (spawnPoint == null)
        {
            Debug.LogWarning($"Не найден объект с тегом '{spawnPointTag}'! Используем запасную позицию.");
            return fallbackSpawnPosition;
        }
        
        return spawnPoint.transform.position;
    }
    
    private void SpawnSingleObject(GameObject prefab)
    {
        if (prefab == null) return;
        
        Vector3 spawnPosition = lastSpawnPosition + spawnOffset;
        Instantiate(prefab, spawnPosition, Quaternion.identity);
        
        lastSpawnPosition.y -= verticalSpacing;
        Debug.Log($"Спавнен объект: {prefab.name} на позиции: {spawnPosition}");
    }
    
    public void ClearSpawnQueue()
    {
        spawnQueue.Clear();
        Debug.Log("Очередь спавна очищена");
    }
    
    public int GetQueueCount()
    {
        return spawnQueue.Count;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (spawnQueue.Count > 0)
        {
            Invoke(nameof(SpawnAllQueuedObjects), 0.1f);
        }
    }
}