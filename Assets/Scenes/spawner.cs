using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
    [System.Serializable]
    public class ShopItem
    {
        public GameObject uiPrefab; // UI префаб (должен иметь RectTransform)
        public string itemName;
        public int price;
    }

    [Header("Shop Settings")]
    public List<ShopItem> shopItems = new List<ShopItem>();
    public Transform spawnParent; // Родитель для спауна объектов
    public Vector2 firstSpawnPos = new Vector2(0, 0);
    public Vector2 spawnOffset = new Vector2(0, -120f);
    public int maxSpawned = 5;

    [Header("Appearance")]
    public float fadeStep = 0.1f;

    [Header("Scene Settings")]
    public string targetSceneName = "MainScene"; // Сцена куда копируем

    private List<GameObject> spawnedObjects = new List<GameObject>();
    private Vector2 nextSpawnPos;
    private Dictionary<GameObject, ShopItem> itemMap = new Dictionary<GameObject, ShopItem>();

    private void Start()
    {
        nextSpawnPos = firstSpawnPos;
        SetupItemClickHandlers();
    }

    private void SetupItemClickHandlers()
    {
        foreach (ShopItem item in shopItems)
        {
            if (item.uiPrefab != null)
            {
                var button = item.uiPrefab.GetComponent<Button>();
                if (button == null) button = item.uiPrefab.AddComponent<Button>();
                
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => OnItemClicked(item));
                
                itemMap.Add(item.uiPrefab, item);
            }
        }
    }

    private void OnItemClicked(ShopItem item)
    {
        if (CanAfford(item.price))
        {
            SpawnItem(item);
            ChargeForItem(item.price);
        }
        else
        {
            Debug.Log("Not enough money!");
            // Здесь можно показать UI сообщение о недостатке средств
        }
    }

    private bool CanAfford(int price)
    {
        // Реализуйте свою логику проверки денег
        return true; // Заглушка
    }

    private void ChargeForItem(int amount)
    {
        // Реализуйте списание денег
    }

    public void SpawnItem(ShopItem item)
    {
        if (spawnedObjects.Count >= maxSpawned)
        {
            RemoveFirstItem();
        }

        // Создаем на текущей сцене
        GameObject newObj = Instantiate(item.uiPrefab, spawnParent);
        RectTransform rt = newObj.GetComponent<RectTransform>();
        rt.anchoredPosition = nextSpawnPos;

        // Настраиваем прозрачность
        SetObjectAlpha(newObj, 1f - (spawnedObjects.Count * fadeStep));

        spawnedObjects.Add(newObj);
        newObj.name = $"{item.itemName}_{spawnedObjects.Count}";

        // Копируем на целевую сцену
        CopyToTargetScene(item);

        nextSpawnPos += spawnOffset;
    }

    private void CopyToTargetScene(ShopItem item)
    {
        // Находим или загружаем целевую сцену
        Scene targetScene = SceneManager.GetSceneByName(targetSceneName);
        if (!targetScene.IsValid())
        {
            Debug.LogError($"Target scene {targetSceneName} not found!");
            return;
        }

        // Находим корневой canvas на целевой сцене
        Canvas targetCanvas = null;
        GameObject[] rootObjects = targetScene.GetRootGameObjects();
        foreach (GameObject go in rootObjects)
        {
            targetCanvas = go.GetComponentInChildren<Canvas>(true);
            if (targetCanvas != null) break;
        }

        if (targetCanvas == null)
        {
            Debug.LogError("No Canvas found in target scene!");
            return;
        }

        // Создаем копию на целевой сцене
        GameObject sceneCopy = Instantiate(item.uiPrefab, targetCanvas.transform);
        sceneCopy.name = $"{item.itemName}_Copy";

        // Здесь можно добавить дополнительные настройки для копии
        // Например, изменить размер, позицию или добавить специальные компоненты
    }

    private void RemoveFirstItem()
    {
        if (spawnedObjects.Count == 0) return;

        GameObject oldest = spawnedObjects[0];
        spawnedObjects.RemoveAt(0);
        Destroy(oldest);

        ShiftItemsUp();
    }

    private void ShiftItemsUp()
    {
        nextSpawnPos -= spawnOffset;
        
        for (int i = 0; i < spawnedObjects.Count; i++)
        {
            RectTransform rt = spawnedObjects[i].GetComponent<RectTransform>();
            rt.anchoredPosition = firstSpawnPos + spawnOffset * i;
            
            SetObjectAlpha(spawnedObjects[i], 1f - i * fadeStep);
            
            ShopItem item = itemMap[spawnedObjects[i]];
            spawnedObjects[i].name = $"{item.itemName}_{i + 1}";
        }
    }

    private void SetObjectAlpha(GameObject obj, float alpha)
    {
        CanvasGroup cg = obj.GetComponent<CanvasGroup>();
        if (cg == null) cg = obj.AddComponent<CanvasGroup>();
        
        cg.alpha = Mathf.Clamp(alpha, 0.3f, 1f);
    }

    public void ClearAll()
    {
        foreach (GameObject obj in spawnedObjects)
        {
            if (obj != null) Destroy(obj);
        }
        spawnedObjects.Clear();
        nextSpawnPos = firstSpawnPos;
    }
}