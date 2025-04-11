using UnityEngine;

public class FaceCameraYAxisOnly : MonoBehaviour
{
    [Tooltip("Camera mà canvas sẽ luôn nhìn về")]
    public Transform cameraTransform;

    [Tooltip("Tốc độ xoay mượt về hướng camera")]
    public float rotationSmoothSpeed = 5f;

    [Tooltip("Khoảng cách cố định từ camera tới canvas")]
    public float fixedDistanceFromCamera = 0.7f;

    public bool _isObject;
    public bool _isSphereControl;

    private void LateUpdate()
    {
        if (cameraTransform == null) return;

        if (_isSphereControl)
        {
            // ✅ Giữ nguyên vị trí
            // ✅ Chỉ xoay theo trục X và Z (không đổi Y rotation)

            Vector3 directionFromCamera = transform.position - cameraTransform.position;
            directionFromCamera.y = 0; // ❌ Loại trục Y khỏi hướng xoay
            directionFromCamera.Normalize();

            if (directionFromCamera.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionFromCamera);

                // Giữ lại giá trị Y hiện tại (không đổi)
                Vector3 currentEuler = transform.eulerAngles;
                Vector3 newEuler = targetRotation.eulerAngles;
                newEuler.y = currentEuler.y;

                transform.rotation = Quaternion.Euler(newEuler);
            }

            return;
        }

        if (!_isObject)
        {
            Vector3 targetPosition = cameraTransform.position + cameraTransform.forward * fixedDistanceFromCamera;
            targetPosition.y = cameraTransform.position.y;
            transform.position = targetPosition;

            Vector3 directionFromCamera = transform.position - cameraTransform.position;
            directionFromCamera.y = 0;

            if (directionFromCamera.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionFromCamera);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothSpeed * Time.deltaTime);
            }
        }
        else
        {
            // Chỉ cập nhật Y nếu là object
            Vector3 currentPosition = transform.position;
            transform.position = new Vector3(
                currentPosition.x,
                cameraTransform.position.y,
                currentPosition.z
            );
        }
    

    }
}