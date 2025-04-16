using UnityEngine;

public class UpdateAnchorPosition : MonoBehaviour
{
    [Header("Camera Reference")]
    [Tooltip("Gán Main Camera (hoặc XR Camera) vào đây")]
    public UnityEngine.Camera mainCamera;
    
    [Header("Offset Settings (Local Space)")]
    [Tooltip("Offset trên mặt phẳng ngang so với camera. Ví dụ: (-0.3, 0, 0.2) sẽ dịch sang trái 0.3m và về phía trước 0.2m so với camera.")]
    public Vector3 horizontalOffset = new Vector3(-0.3f, 0f, 0.2f);
    
    [Header("Height Settings")]
    [Tooltip("Khoảng cách từ mắt đến hông (để tính chiều cao hông); ví dụ: 0.5m")]
    public float headToHipOffset = 0.5f;
    
    [Header("Rotation Settings")]
    [Tooltip("Nếu true, Anchor sẽ update rotation theo camera (chỉ tính trên mặt phẳng ngang)")]
    public bool updateRotation = true;
    [Tooltip("Tốc độ xoay mượt")]
    public float rotationSmoothSpeed = 5f;

    void LateUpdate()
    {
        if (mainCamera == null)
            return;
        
        // Tính hipHeight dựa trên vị trí của camera: chiều cao hông = camera.y - headToHipOffset
        float hipHeight = mainCamera.transform.position.y - headToHipOffset;
        
        // Lấy vị trí của camera, sau đó ghi đè giá trị Y bằng hipHeight
        Vector3 basePosition = mainCamera.transform.position;
        basePosition.y = hipHeight;
        
        // Lấy hướng tiến và phải theo mặt phẳng ngang của camera
        Vector3 flatForward = mainCamera.transform.forward;
        flatForward.y = 0f;
        flatForward.Normalize();
        Vector3 flatRight = mainCamera.transform.right;
        flatRight.y = 0f;
        flatRight.Normalize();
        
        // Tính vị trí target cho Anchor dựa trên horizontalOffset
        Vector3 targetPosition = basePosition + flatRight * horizontalOffset.x + flatForward * horizontalOffset.z;
        targetPosition.y = hipHeight; // đảm bảo Y luôn bằng hipHeight
        transform.position = targetPosition;
        
        // Cập nhật rotation nếu updateRotation == true
        if (updateRotation)
        {
            // Lấy vector từ camera đến Anchor trên mặt phẳng ngang
            Vector3 directionFromCamera = transform.position - mainCamera.transform.position;
            directionFromCamera.y = 0f;
            if (directionFromCamera.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionFromCamera.normalized);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothSpeed * Time.deltaTime);
            }
        }
    }
}
