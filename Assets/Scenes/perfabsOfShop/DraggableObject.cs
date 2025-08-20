using UnityEngine;

public class DraggableObject : MonoBehaviour
{
    [Header("Drag Settings")]
    public float dragSpeed = 10f;
    public bool isDraggable = true;
    
    [HideInInspector]
    public Camera mainCamera; // Теперь публичное поле для установки извне
    
    private bool isDragging = false;
    private Vector3 offset;
    
    void Start()
    {
        // Если камера не установлена, пытаемся найти
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }
    
    void Update()
    {
        // Постоянно проверяем наличие камеры
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }
    
    void OnMouseDown()
    {
        if (isDraggable && mainCamera != null)
        {
            isDragging = true;
            offset = transform.position - GetMouseWorldPos();
        }
    }
    
    void OnMouseDrag()
    {
        if (isDragging && mainCamera != null)
        {
            Vector3 targetPosition = GetMouseWorldPos() + offset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, dragSpeed * Time.deltaTime);
        }
    }
    
    void OnMouseUp()
    {
        isDragging = false;
    }
    
    private Vector3 GetMouseWorldPos()
    {
        if (mainCamera == null) return Vector3.zero;
        
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = Mathf.Abs(mainCamera.transform.position.z);
        return mainCamera.ScreenToWorldPoint(mousePoint);
    }
    
    // Для UI элементов (если нужно)
    public void SetDraggable(bool draggable)
    {
        isDraggable = draggable;
    }
    
    // Метод для установки камеры извне
    public void SetCamera(Camera camera)
    {
        mainCamera = camera;
    }
}