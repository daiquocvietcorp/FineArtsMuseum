using UnityEngine;

public class FaceCameraYAxisOnly : MonoBehaviour
{
    [Tooltip("Camera mà canvas sẽ luôn nhìn về")]
    public Transform cameraTransform;

    [Tooltip("Tốc độ xoay mượt về hướng camera")]
    public float rotationSmoothSpeed = 5f;

    [Tooltip("Khoảng cách cố định từ camera tới canvas")]
    public float fixedDistanceFromCamera = 0.7f;

    private void LateUpdate()
    {
        if (cameraTransform == null) return;

        // Vị trí mới: luôn trước mặt camera ở khoảng cách cố định và ngang tầm mắt
        Vector3 targetPosition = cameraTransform.position + cameraTransform.forward * fixedDistanceFromCamera;
        targetPosition.y = cameraTransform.position.y; // Đảm bảo ngang tầm mắt
        transform.position = targetPosition;

        // Xoay đối mặt camera theo trục Y
        Vector3 directionFromCamera = transform.position - cameraTransform.position;
        directionFromCamera.y = 0;

        if (directionFromCamera.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionFromCamera);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothSpeed * Time.deltaTime);
        }
    }
}