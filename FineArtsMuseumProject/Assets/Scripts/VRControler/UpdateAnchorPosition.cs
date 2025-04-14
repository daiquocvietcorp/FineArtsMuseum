using UnityEngine;

public class UpdateAnchorPosition : MonoBehaviour
{
    [Header("Camera Reference")]
    [Tooltip("Gán Main Camera vào đây")]
    public UnityEngine.Camera mainCamera;

    [Header("Offset Settings")]
    [Tooltip("Offset từ camera đến hông (local space của camera); ví dụ: (-0.3, -0.5, 0.2) là bên trái, thấp hơn và phía trước")]
    public Vector3 hipOffset = new Vector3(-0.3f, -0.5f, 0.2f);

    [Header("Rotation Settings")]
    [Tooltip("Nếu true, đối tượng sẽ update rotation theo camera (chỉ thay đổi X và Z, giữ nguyên Y)")]
    public bool updateRotation = true;

    void LateUpdate()
    {
        if (mainCamera == null)
            return;

        // Cập nhật vị trí Anchor: chuyển hipOffset từ local space của camera sang world space
        Vector3 targetPosition = mainCamera.transform.TransformPoint(hipOffset);
        transform.position = targetPosition;
        
        // Nếu yêu cầu cập nhật rotation, hãy lấy Euler của camera và chỉ dùng trục X và Z
        if (updateRotation)
        {
            // Lấy góc của camera
            Vector3 camEuler = mainCamera.transform.rotation.eulerAngles;
            // Giữ trục Y của đối tượng như hiện tại
            Vector3 currentEuler = transform.rotation.eulerAngles;
            // Tạo góc mới: dùng góc X và Z của camera, nhưng giữ Y không đổi
            Vector3 newEuler = new Vector3(camEuler.x, currentEuler.y, camEuler.z);
            transform.rotation = Quaternion.Euler(newEuler);
        }
    }
}