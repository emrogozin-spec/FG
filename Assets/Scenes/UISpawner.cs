using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class UISpawner : MonoBehaviour, IPointerClickHandler
{
    [System.Serializable]
    public class UIItem
    {
        public GameObject prefab; // UI префаб (должен иметь RectTransform)
        public string itemName;
    }

    [Header("UI Spawn Settings")]
    public List<UIItem> itemsToSpawn = new List<UIItem>();
    public RectTransform spawnParent; // Контейнер для UI элементов
    public Vector2 firstSpawnPos = new Vector2(0, -50);
    public Vector2 spawnOffset = new Vector2(0, -60);
    public int maxSpawned = 5;

    [Header("Appearance")]
    public float fadeStep = 0.1f;

    private List<GameObject> spawnedUIElements = new List<GameObject>();
    private Vector2 nextSpawnPos;
    private Dictionary<GameObject, UIItem> itemMap = new Dictionary<GameObject, UIItem>();
    private GameObject currentlyDraggedUI;
    private Vector2 dragOffset;

    private void Start()
    {
        nextSpawnPos = firstSpawnPos;
        SetupUIItemHandlers();
    }

    private void SetupUIItemHandlers()
    {
        foreach (UIItem item in itemsToSpawn)
        {
            if (item.prefab != null)
            {
                var clickHandler = item.prefab.AddComponent<UIItemClickHandler>();
                clickHandler.Setup(this, item);
                itemMap.Add(item.prefab, item);
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Обработка кликов по самому spawner'у
    }

    private void Update()
    {
        HandleUIDrag();
    }

    private void HandleUIDrag()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryStartUIDrag();
        }

        if (currentlyDraggedUI != null)
        {
            if (Input.GetMouseButton(0))
            {
                ContinueUIDrag();
            }
            else
            {
                EndUIDrag();
            }
        }
    }

    private void TryStartUIDrag()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (RaycastResult result in results)
        {
            if (spawnedUIElements.Contains(result.gameObject))
            {
                currentlyDraggedUI = result.gameObject;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    spawnParent,
                    Input.mousePosition,
                    null,
                    out Vector2 localPoint);
                dragOffset = (Vector2)currentlyDraggedUI.GetComponent<RectTransform>().localPosition - localPoint;
                break;
            }
        }
    }

    private void ContinueUIDrag()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            spawnParent,
            Input.mousePosition,
            null,
            out Vector2 localPoint);
        currentlyDraggedUI.GetComponent<RectTransform>().localPosition = localPoint + dragOffset;
    }

    private void EndUIDrag()
    {
        // Можно добавить логику фиксации в определенной зоне
        currentlyDraggedUI = null;
    }

    public void SpawnUIItem(UIItem item)
    {
        if (spawnedUIElements.Count >= maxSpawned)
        {
            RemoveFirstUIItem();
        }

        GameObject newUIElement = Instantiate(item.prefab, spawnParent);
        newUIElement.GetComponent<RectTransform>().localPosition = nextSpawnPos;

        // Настройка прозрачности через CanvasGroup
        CanvasGroup cg = newUIElement.GetComponent<CanvasGroup>() ?? newUIElement.AddComponent<CanvasGroup>();
        float alpha = 1f - (spawnedUIElements.Count * fadeStep);
        cg.alpha = Mathf.Clamp(alpha, 0.3f, 1f);

        spawnedUIElements.Add(newUIElement);
        newUIElement.name = $"{item.itemName}_{spawnedUIElements.Count}";

        nextSpawnPos += spawnOffset;
    }

    private void RemoveFirstUIItem()
    {
        if (spawnedUIElements.Count == 0) return;

        GameObject oldest = spawnedUIElements[0];
        spawnedUIElements.RemoveAt(0);
        Destroy(oldest);

        ShiftUIItemsUp();
    }

    private void ShiftUIItemsUp()
    {
        nextSpawnPos -= spawnOffset;

        for (int i = 0; i < spawnedUIElements.Count; i++)
        {
            RectTransform rt = spawnedUIElements[i].GetComponent<RectTransform>();
            rt.localPosition = firstSpawnPos + spawnOffset * i;

            CanvasGroup cg = spawnedUIElements[i].GetComponent<CanvasGroup>();
            float alpha = 1f - i * fadeStep;
            cg.alpha = Mathf.Clamp(alpha, 0.3f, 1f);

            UIItem item = itemMap[spawnedUIElements[i]];
            spawnedUIElements[i].name = $"{item.itemName}_{i + 1}";
        }
    }

    public void ClearAllUI()
    {
        foreach (GameObject uiElement in spawnedUIElements)
        {
            if (uiElement != null) Destroy(uiElement);
        }
        spawnedUIElements.Clear();
        nextSpawnPos = firstSpawnPos;
    }
}

public class UIItemClickHandler : MonoBehaviour, IPointerClickHandler
{
    private UISpawner uiSpawner;
    private UISpawner.UIItem myItem;

    public void Setup(UISpawner spawner, UISpawner.UIItem item)
    {
        this.uiSpawner = spawner;
        this.myItem = item;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (uiSpawner != null)
        {
            uiSpawner.SpawnUIItem(myItem);
        }
    }
}