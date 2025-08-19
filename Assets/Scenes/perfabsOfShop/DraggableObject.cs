using UnityEngine;

public class DraggableObject : MonoBehaviour
{
    [Header("Drag Settings")]
    public float dragSpeed = 10f;
    public bool isDraggable = true;
    
    private bool isDragging = false;
    private Vector3 offset;
    private Camera mainCamera;
    
    void Start()
    {
        mainCamera = Camera.main;
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
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = Mathf.Abs(mainCamera.transform.position.z);
        return mainCamera.ScreenToWorldPoint(mousePoint);
    }
    
    // Для UI элементов (если нужно)
    public void SetDraggable(bool draggable)
    {
        isDraggable = draggable;
    }
}