using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;
    
    [Header("Spawn Settings")]
    public string spawnPointTag = "SpawnPoint";
    public float verticalSpacing = 0.5f; // Отступ между объектами
    public Vector3 spawnOffset = Vector3.zero;
    public bool spawnAsList = true; // Режим списка (сверху вниз)
    
    [Header("Object Management")]
    public bool keepObjectsBetweenSceneLoads = true;
    public bool autoAddDraggableComponent = true;
    public float defaultDragSpeed = 15f;
    
    [Header("Fallback Settings")]
    public Vector3 fallbackSpawnPosition = new Vector3(0, 3f, 0); // Выше начальная позиция
    
    private Queue<GameObject> spawnQueue = new Queue<GameObject>();
    private Vector3 baseSpawnPosition; // Базовая позиция (точка спавна)
    private List<GameObject> spawnedObjects = new List<GameObject>();
    private bool isInitialized = false;
    private string currentSceneName;
    private Camera mainCamera;
    private float currentYOffset = 0f; // Текущее смещение по Y
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
            isInitialized = true;
            currentSceneName = SceneManager.GetActiveScene().name;
            mainCamera = Camera.main;
            
            // Инициализируем базовую позицию
            baseSpawnPosition = GetSpawnPosition();
            currentYOffset = 0f;
            
            Debug.Log("SpawnManager инициализирован. Базовая позиция: " + baseSpawnPosition);
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
    
    void Update()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
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
        
        // Получаем размер объекта
        float objectHeight = GetObjectHeight(prefab);
        
        // Вычисляем позицию для нового объекта
        Vector3 spawnPosition;
        
        if (spawnAsList)
        {
            // Режим списка: каждый объект ниже предыдущего
            spawnPosition = baseSpawnPosition + new Vector3(0, -currentYOffset, 0) + spawnOffset;
            
            // Увеличиваем смещение для следующего объекта
            currentYOffset += objectHeight + verticalSpacing;
        }
        else
        {
            // Старый режим (в одной точке)
            spawnPosition = baseSpawnPosition + spawnOffset;
        }
        
        GameObject newObject = Instantiate(prefab, spawnPosition, Quaternion.identity);
        
        if (autoAddDraggableComponent && newObject.GetComponent<DraggableObject>() == null)
        {
            if (newObject.GetComponent<RectTransform>() == null)
            {
                DraggableObject draggable = newObject.AddComponent<DraggableObject>();
                draggable.dragSpeed = defaultDragSpeed;
                draggable.mainCamera = mainCamera;
            }
        }
        
        if (keepObjectsBetweenSceneLoads)
        {
            DontDestroyOnLoad(newObject);
        }
        
        spawnedObjects.Add(newObject);
        
        Debug.Log($"Спавнен объект: {prefab.name} на позиции: {spawnPosition}, высота: {objectHeight}, смещение: {currentYOffset}");
    }
    
    // Получаем высоту объекта через коллайдер или рендерер
    private float GetObjectHeight(GameObject prefab)
    {
        // Пытаемся получить коллайдер
        Collider collider = prefab.GetComponent<Collider>();
        if (collider != null)
        {
            return collider.bounds.size.y;
        }
        
        // Если нет коллайдера, пытаемся получить рендерер
        Renderer renderer = prefab.GetComponent<Renderer>();
        if (renderer != null)
        {
            return renderer.bounds.size.y;
        }
        
        // Если это UI элемент
        RectTransform rectTransform = prefab.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            return rectTransform.rect.height;
        }
        
        // Если ничего не найдено, возвращаем стандартное значение
        return 1f;
    }
    
    // Сбрасываем позицию спавна
    public void ResetSpawnPosition()
    {
        baseSpawnPosition = GetSpawnPosition();
        currentYOffset = 0f;
        Debug.Log("Позиция спавна сброшена: " + baseSpawnPosition);
    }
    
    private Vector3 GetSpawnPosition()
    {
        if (string.IsNullOrEmpty(spawnPointTag))
        {
            return fallbackSpawnPosition;
        }
        
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag(spawnPointTag);
        if (spawnPoints.Length == 0)
        {
            Debug.LogWarning($"Не найден объект с тегом '{spawnPointTag}', используем позицию по умолчанию");
            return fallbackSpawnPosition;
        }
        
        // Берем первый найденный spawn point
        return spawnPoints[0].transform.position;
    }
    
    public bool IsOnActiveScene()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        return gameObject.scene == activeScene;
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
        ResetSpawnPosition();
        
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
        
        mainCamera = Camera.main;
        spawnedObjects.RemoveAll(obj => obj == null);
        UpdateDraggableObjectsCamera();
        
        // Сбрасываем позицию при загрузке новой сцены
        ResetSpawnPosition();
        
        if (keepObjectsBetweenSceneLoads)
        {
            foreach (GameObject obj in spawnedObjects)
            {
                if (obj != null && !IsObjectPersistent(obj))
                {
                    DontDestroyOnLoad(obj);
                }
            }
        }
        
        if (spawnQueue.Count > 0)
        {
            Debug.Log($"В очереди {spawnQueue.Count} объектов, запускаем спавн...");
            Invoke(nameof(SpawnAllQueuedObjects), 0.1f);
        }
        
        Debug.Log($"Всего объектов: {spawnedObjects.Count}");
    }
    
    private void UpdateDraggableObjectsCamera()
    {
        foreach (GameObject obj in spawnedObjects)
        {
            if (obj != null)
            {
                DraggableObject draggable = obj.GetComponent<DraggableObject>();
                if (draggable != null)
                {
                    draggable.mainCamera = mainCamera;
                }
            }
        }
    }
    
    private bool IsObjectPersistent(GameObject obj)
    {
        return obj != null && !obj.scene.isLoaded;
    }
    
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
    
    // Метод для ручного изменения отступа
    public void SetVerticalSpacing(float spacing)
    {
        verticalSpacing = spacing;
        Debug.Log($"Установлен отступ: {spacing}");
    }
}