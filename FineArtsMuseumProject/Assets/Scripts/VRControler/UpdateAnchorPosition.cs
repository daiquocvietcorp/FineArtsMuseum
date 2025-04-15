using UnityEngine;

public class UpdateAnchorPosition : MonoBehaviour
{
    [Header("Camera Reference")]
    [Tooltip("Gán Main Camera (hoặc XR Camera) vào đây")]
    public UnityEngine.Camera mainCamera;

    [Header("Horizontal Offset (Local Space)")]
    [Tooltip("Offset trên mặt phẳng ngang so với camera. Ví dụ: (-0.3, 0, 0.2) để đặt Anchor ở hông bên trái phía trước")]
    public Vector3 horizontalOffset = new Vector3(-0.3f, 0f, 0.2f);

    [Header("Height Settings")]
    [Tooltip("Chiều cao mặc định cho hông người chơi nếu chưa hiệu chỉnh (m), ví dụ 1.0m")]
    public float defaultHipHeight = 1.0f;
    [Tooltip("Ngưỡng chiều cao (m) khi người chơi đã đứng dậy đủ để hiệu chỉnh hông, ví dụ 1.2m")]
    public float calibrateThreshold = 1.2f;
    [Tooltip("Tốc độ lerp để cập nhật chiều cao (hipHeight) khi người chơi di chuyển")]
    public float heightLerpSpeed = 2f;

    // Biến nội bộ
    private float currentHipHeight;
    private bool calibrated = false;

    void Start()
    {
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera chưa được gán!");
            return;
        }
        // Khởi tạo chiều cao: chọn giá trị lớn hơn giữa default và chiều cao hiện tại của camera
        currentHipHeight = Mathf.Max(defaultHipHeight, mainCamera.transform.position.y);
        UpdateAnchorPos();
    }

    void LateUpdate()
    {
        if (mainCamera == null)
            return;

        // Xác định vị trí cơ bản: lấy vị trí của Main Camera và ghi đè Y bằng currentHipHeight (khóa chiều cao)
        Vector3 basePosition = mainCamera.transform.position;
        basePosition.y = currentHipHeight;

        // Lấy hướng tiến và phải theo mặt phẳng ngang từ Main Camera:
        Vector3 flatForward = mainCamera.transform.forward;
        flatForward.y = 0f;
        flatForward.Normalize();
        Vector3 flatRight = mainCamera.transform.right;
        flatRight.y = 0f;
        flatRight.Normalize();

        // Tính vị trí mới của Anchor: vị trí cơ bản cộng offset theo hướng
        Vector3 targetPosition = basePosition + flatRight * horizontalOffset.x + flatForward * horizontalOffset.z;
        targetPosition.y = currentHipHeight; // đảm bảo Y bằng hip height
        transform.position = targetPosition;

        // Nếu người chơi đã đứng đủ cao (calibrateThreshold), cập nhật currentHipHeight theo cách mượt.
        if (!calibrated)
        {
            if (mainCamera.transform.position.y >= calibrateThreshold)
            {
                // Khi đạt được ngưỡng, đánh dấu calibrated
                calibrated = true;
                currentHipHeight = mainCamera.transform.position.y;
            }
        }
        else
        {
            // Nếu đã calibrated, lerp currentHipHeight theo chiều cao của camera.
            currentHipHeight = Mathf.Lerp(currentHipHeight, mainCamera.transform.position.y, Time.deltaTime * heightLerpSpeed);
        }
    }

    void UpdateAnchorPos()
    {
        // Cập nhật vị trí Anchor ban đầu theo Main Camera, dựa trên horizontalOffset và default hip height
        Vector3 basePos = mainCamera.transform.position;
        basePos.y = currentHipHeight;
        Vector3 flatForward = mainCamera.transform.forward;
        flatForward.y = 0f;
        flatForward.Normalize();
        Vector3 flatRight = mainCamera.transform.right;
        flatRight.y = 0f;
        flatRight.Normalize();
        Vector3 pos = basePos + flatRight * horizontalOffset.x + flatForward * horizontalOffset.z;
        pos.y = currentHipHeight;
        transform.position = pos;
    }
}
