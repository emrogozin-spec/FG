using UnityEngine;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
    [System.Serializable]
    public class CollectableItem
    {
        public GameObject prefab;
        public string itemName;
    }

    [Header("Spawn Settings")]
    public List<CollectableItem> itemsToSpawn = new List<CollectableItem>();
    public Transform spawnParent;
    public Vector3 firstSpawnPos = new Vector3(0, 4, -6); // Z = -6
    public Vector3 spawnOffset = new Vector3(0, -1.2f, 0);
    public int maxSpawned = 5;

    [Header("Appearance")]
    public float fadeStep = 0.1f;

    [Header("Background Settings")]
    public Transform background; // Движущийся фон
    public LayerMask backgroundLayer;

    private List<GameObject> spawnedObjects = new List<GameObject>();
    private List<GameObject> fixedObjects = new List<GameObject>();
    private Vector3 nextSpawnPos;
    private Dictionary<GameObject, CollectableItem> itemMap = new Dictionary<GameObject, CollectableItem>();
    private GameObject currentlyDraggedObject;
    private Vector3 offset;
    private Vector3 dragStartBackgroundPos;

    private void Start()
    {
        nextSpawnPos = firstSpawnPos;
        SetupItemClickHandlers();
    }

    private void SetupItemClickHandlers()
    {
        foreach (CollectableItem item in itemsToSpawn)
        {
            if (item.prefab != null)
            {
                var clicker = item.prefab.AddComponent<ItemClickDetector>();
                clicker.Setup(this, item);
                itemMap.Add(item.prefab, item);
            }
        }
    }

    private void Update()
    {
        HandleDrag();
    }

    private void HandleDrag()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryStartDrag();
        }

        if (currentlyDraggedObject != null)
        {
            if (Input.GetMouseButton(0))
            {
                ContinueDrag();
            }
            else
            {
                EndDrag();
            }
        }
    }

    private void TryStartDrag()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (hit.collider != null && (spawnedObjects.Contains(hit.collider.gameObject) || fixedObjects.Contains(hit.collider.gameObject)))
        {
            currentlyDraggedObject = hit.collider.gameObject;
            offset = currentlyDraggedObject.transform.position - 
                   Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
            dragStartBackgroundPos = background.position;
        }
    }

    private void ContinueDrag()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
        Vector3 newPosition = mousePos + offset;
        
        // Учитываем движение фона
        Vector3 backgroundMovement = background.position - dragStartBackgroundPos;
        currentlyDraggedObject.transform.position = new Vector3(newPosition.x, newPosition.y, -6) + backgroundMovement;
    }

    private void EndDrag()
    {
        RaycastHit2D hit = Physics2D.Raycast(
            Camera.main.ScreenToWorldPoint(Input.mousePosition),
            Vector2.zero,
            Mathf.Infinity,
            backgroundLayer);

        if (hit.collider != null && background != null)
        {
            FixObjectToBackground(currentlyDraggedObject);
        }
        else if (spawnedObjects.Contains(currentlyDraggedObject))
        {
            ReturnObjectToSpawnPosition(currentlyDraggedObject);
        }

        currentlyDraggedObject = null;
    }

    private void FixObjectToBackground(GameObject obj)
    {
        // Устанавливаем Z = -6
        obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, -6);
        
        // Переносим в список зафиксированных
        if (spawnedObjects.Contains(obj))
        {
            spawnedObjects.Remove(obj);
            fixedObjects.Add(obj);
            ShiftItemsUp();
        }
        
        MakeObjectStatic(obj);
    }

    private void ReturnObjectToSpawnPosition(GameObject obj)
    {
        int index = spawnedObjects.IndexOf(obj);
        if (index >= 0)
        {
            obj.transform.position = firstSpawnPos + spawnOffset * index;
            obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, -6);
        }
    }

    private void MakeObjectStatic(GameObject obj)
    {
        var dragger = obj.GetComponent<ItemClickDetector>();
        if (dragger != null) Destroy(dragger);

        var collider = obj.GetComponent<Collider2D>();
        if (collider != null) collider.enabled = false;

        obj.layer = LayerMask.NameToLayer("Ignore Raycast");
    }

    public void SpawnItem(CollectableItem item)
    {
        if (spawnedObjects.Count >= maxSpawned)
        {
            RemoveFirstItem();
        }

        GameObject newObj = Instantiate(item.prefab, spawnParent);
        newObj.transform.position = nextSpawnPos;
        newObj.transform.position = new Vector3(newObj.transform.position.x, newObj.transform.position.y, -6);

        float alpha = 1f - (spawnedObjects.Count * fadeStep);
        SetObjectAlpha(newObj, Mathf.Clamp(alpha, 0.3f, 1f));

        spawnedObjects.Add(newObj);
        newObj.name = $"{item.itemName}_{spawnedObjects.Count}";

        nextSpawnPos += spawnOffset;
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
            spawnedObjects[i].transform.position = firstSpawnPos + spawnOffset * i;
            spawnedObjects[i].transform.position = new Vector3(spawnedObjects[i].transform.position.x, spawnedObjects[i].transform.position.y, -6);
            float alpha = 1f - i * fadeStep;
            SetObjectAlpha(spawnedObjects[i], Mathf.Clamp(alpha, 0.3f, 1f));
            
            CollectableItem item = itemMap[spawnedObjects[i]];
            spawnedObjects[i].name = $"{item.itemName}_{i + 1}";
        }
    }

    private void SetObjectAlpha(GameObject obj, float alpha)
    {
        Renderer rend = obj.GetComponent<Renderer>();
        if (rend != null)
        {
            Color c = rend.material.color;
            c.a = alpha;
            rend.material.color = c;
        }
        
        CanvasGroup cg = obj.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.alpha = alpha;
        }
    }

    public void ClearAll()
    {
        foreach (GameObject obj in spawnedObjects)
        {
            if (obj != null) Destroy(obj);
        }
        foreach (GameObject obj in fixedObjects)
        {
            if (obj != null) Destroy(obj);
        }
        spawnedObjects.Clear();
        fixedObjects.Clear();
        nextSpawnPos = firstSpawnPos;
    }
}

public class ItemClickDetector : MonoBehaviour
{
    private Spawner itemSpawner;
    private Spawner.CollectableItem myItem;

    public void Setup(Spawner spawner, Spawner.CollectableItem item)
    {
        this.itemSpawner = spawner;
        this.myItem = item;
    }

    private void OnMouseDown()
    {
        if (itemSpawner != null)
        {
            itemSpawner.SpawnItem(myItem);
        }
    }
}