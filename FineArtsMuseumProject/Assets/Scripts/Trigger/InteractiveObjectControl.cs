using UnityEngine;
using UnityEngine.EventSystems;

public class InteractiveObjectControl : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public float rotationSpeed = 5f; // Tốc độ xoay
    private bool isRotating = false; // Kiểm tra có đang giữ chuột không
    private Vector3 lastMousePosition; // Lưu vị trí chuột trước đó
    
    void Update()
    {
        if (isRotating)
        {
            RotateObject();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown");
        isRotating = true;
        lastMousePosition = Input.mousePosition; // Lưu vị trí chuột khi bắt đầu nhấn
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("OnPointerUp");
        isRotating = false; // Ngừng xoay khi thả chuột
    }

    private void RotateObject()
    {
        Vector3 deltaMouse = Input.mousePosition - lastMousePosition; // Tính độ thay đổi vị trí chuột
        lastMousePosition = Input.mousePosition; // Cập nhật vị trí chuột mới

        float rotateX = -deltaMouse.y * rotationSpeed * Time.deltaTime; // Xoay theo trục X (dọc)
        float rotateY = deltaMouse.x * rotationSpeed * Time.deltaTime; // Xoay theo trục Y (ngang)

        transform.Rotate(rotateX, rotateY, 0, Space.World); // Xoay theo không gian thế giới
    }
}