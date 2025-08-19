using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;
    
    [Header("Spawn Settings")]
    public string spawnPointTag = "SpawnPoint";
    public float verticalSpacing = 0.5f; // Уменьшил отступ
    public Vector3 spawnOffset = Vector3.zero;
    
    [Header("Object Management")]
    public bool keepObjectsBetweenSceneLoads = true;
    public bool autoAddDraggableComponent = true;
    public float defaultDragSpeed = 15f;
    
    [Header("Fallback Settings")]
    public Vector3 fallbackSpawnPosition = new Vector3(0, 2, 0); // Понизил позицию
    
    private Queue<GameObject> spawnQueue = new Queue<GameObject>();
    private Vector3 lastSpawnPosition;
    private List<GameObject> spawnedObjects = new List<GameObject>();
    private bool isInitialized = false;
    private string currentSceneName;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
            isInitialized = true;
            currentSceneName = SceneManager.GetActiveScene().name;
            Debug.Log("SpawnManager инициализирован");
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void OnDestroy()
    {
        if (isInitialized)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Debug.Log("SpawnManager уничтожен");
        }
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
        if (spawnQueue.Count == 0)
        {
            Debug.Log("Очередь спавна пуста");
            return;
        }
        
        Debug.Log($"Начинаем спавн {spawnQueue.Count} объектов...");
        
        Vector3 spawnPosition = GetSpawnPosition();
        lastSpawnPosition = spawnPosition;
        
        int spawnedCount = 0;
        while (spawnQueue.Count > 0)
        {
            GameObject prefab = spawnQueue.Dequeue();
            if (prefab != null)
            {
                SpawnSingleObject(prefab);
                spawnedCount++;
            }
        }
        
        Debug.Log($"Успешно спавнено {spawnedCount} объектов");
    }
    
    private void SpawnSingleObject(GameObject prefab)
    {
        if (prefab == null) return;
        
        Vector3 spawnPosition = lastSpawnPosition + spawnOffset;
        GameObject newObject = Instantiate(prefab, spawnPosition, Quaternion.identity);
        
        // Автоматически добавляем DraggableObject если нужно
        if (autoAddDraggableComponent && newObject.GetComponent<DraggableObject>() == null)
        {
            if (newObject.GetComponent<RectTransform>() == null)
            {
                DraggableObject draggable = newObject.AddComponent<DraggableObject>();
                draggable.dragSpeed = defaultDragSpeed;
            }
        }
        
        // Делаем объект персистентным сразу
        if (keepObjectsBetweenSceneLoads)
        {
            DontDestroyOnLoad(newObject);
        }
        
        // Добавляем в список созданных объектов
        spawnedObjects.Add(newObject);
        
        // Обновляем позицию для следующего объекта
        lastSpawnPosition.y -= verticalSpacing;
        
        Debug.Log($"Спавнен объект: {prefab.name} на позиции: {spawnPosition}");
    }
    
    private Vector3 GetSpawnPosition()
    {
        if (string.IsNullOrEmpty(spawnPointTag))
        {
            return fallbackSpawnPosition;
        }
        
        GameObject spawnPoint = GameObject.FindGameObjectWithTag(spawnPointTag);
        if (spawnPoint == null)
        {
            return fallbackSpawnPosition;
        }
        
        return spawnPoint.transform.position;
    }
    
    public void ClearSpawnQueue()
    {
        spawnQueue.Clear();
        Debug.Log("Очередь спавна очищена");
    }
    
    public void ClearAllObjects()
    {
        foreach (GameObject obj in spawnedObjects)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }
        spawnedObjects.Clear();
        spawnQueue.Clear();
        
        Debug.Log("Все объекты и очередь очищены");
    }
    
    public int GetQueueCount()
    {
        return spawnQueue.Count;
    }
    
    public int GetSpawnedObjectsCount()
    {
        return spawnedObjects.Count;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Загружена сцена: {scene.name}");
        currentSceneName = scene.name;
        
        // Очищаем null объекты из списка
        spawnedObjects.RemoveAll(obj => obj == null);
        
        // Если объекты должны сохраняться между сценами
        if (keepObjectsBetweenSceneLoads)
        {
            // Убеждаемся что все объекты персистентные
            foreach (GameObject obj in spawnedObjects)
            {
                if (obj != null && !IsObjectPersistent(obj))
                {
                    DontDestroyOnLoad(obj);
                }
            }
        }
        
        // Спавним объекты из очереди
        if (spawnQueue.Count > 0)
        {
            Debug.Log($"В очереди {spawnQueue.Count} объектов, запускаем спавн...");
            Invoke(nameof(SpawnAllQueuedObjects), 0.1f);
        }
        
        Debug.Log($"Всего объектов: {spawnedObjects.Count}");
    }
    
    // Проверяем является ли объект персистентным
    private bool IsObjectPersistent(GameObject obj)
    {
        // Объект персистентный если его сцена не загружена
        return obj != null && !obj.scene.isLoaded;
    }
    
    // Метод для перемещения всех объектов в текущую сцену
    public void MoveAllObjectsToCurrentScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        int movedCount = 0;
        
        foreach (GameObject obj in spawnedObjects)
        {
            if (obj != null)
            {
                SceneManager.MoveGameObjectToScene(obj, currentScene);
                movedCount++;
            }
        }
        
        Debug.Log($"Перемещено объектов в текущую сцену: {movedCount}");
    }
}