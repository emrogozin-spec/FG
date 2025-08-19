using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ObjectSpawner : MonoBehaviour, IPointerClickHandler
{
    [Header("Spawn Settings")]
    public GameObject objectPrefab; // Префаб для спавна
    public string targetSceneName; // Имя целевой сцены
    
    [Header("UI Settings")]
    public bool useButtonSound = true;
    
    private void Start()
    {
        // Гарантируем, что кнопка кликабельна
        Image image = GetComponent<Image>();
        if (image != null) image.raycastTarget = true;
        
        Button button = GetComponent<Button>();
        if (button != null) button.onClick.AddListener(OnButtonClick);
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        SpawnObject();
    }
    
    private void OnButtonClick()
    {
        SpawnObject();
    }
    
    public void SpawnObject()
    {
        if (objectPrefab == null || string.IsNullOrEmpty(targetSceneName))
        {
            Debug.LogWarning("Не назначен префаб или сцена!");
            return;
        }
        
        // Проверяем, существует ли экземпляр SpawnManager
        if (SpawnManager.Instance == null)
        {
            Debug.LogError("SpawnManager не найден! Убедитесь, что он есть на сцене.");
            return;
        }
        
        // Добавляем префаб в очередь для спавна
        SpawnManager.Instance.AddToSpawnQueue(objectPrefab);
        
        // Загружаем сцену (если еще не на целевой сцене)
        if (SceneManager.GetActiveScene().name != targetSceneName)
        {
            SceneManager.LoadScene(targetSceneName);
        }
        else
        {
            // Если уже на целевой сцене, сразу спавним
            SpawnManager.Instance.SpawnAllQueuedObjects();
        }
    }
}