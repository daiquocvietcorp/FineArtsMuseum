using UnityEngine;

public class FaceCameraYAxisOnly : MonoBehaviour
{
    [Tooltip("Camera mà canvas (hoặc object) sẽ luôn nhìn về")]
    public Transform cameraTransform;

    [Tooltip("Tốc độ xoay mượt về hướng camera")]
    public float rotationSmoothSpeed = 5f;

    [Tooltip("Khoảng cách cố định từ camera tới canvas (hoặc object)")]
    public float fixedDistanceFromCamera = 0.7f;

    [Tooltip("Nếu true, object sẽ liên tục update vị trí và xoay theo camera; nếu false, chỉ update 1 lần ban đầu (sau khi đối diện với camera)")]
    public bool isMoving;

    public bool _isObject;
    public bool _isSphereControl;

    // Sử dụng hasInitialized để xác định đã thực hiện update ban đầu hay chưa
    private bool hasInitialized = false;

    private void Start()
    {
        if (cameraTransform == null)
            return;

        // Thiết lập vị trí ban đầu khi khởi tạo
        if (!_isSphereControl)
        {
            if (!_isObject)
            {
                // Nếu không phải object, đặt vị trí phía trước camera với khoảng cách cố định
                Vector3 targetPosition = cameraTransform.position + cameraTransform.forward * fixedDistanceFromCamera;
                targetPosition.y = cameraTransform.position.y;
                transform.position = targetPosition;
            }
            else
            {
                // Nếu là object, chỉ cập nhật Y cho phù hợp (giữ X, Z)
                Vector3 currentPosition = transform.position;
                transform.position = new Vector3(
                    currentPosition.x,
                    cameraTransform.position.y,
                    currentPosition.z
                );
            }
        }
        // Đặt hướng ban đầu: đối diện với camera (trên mặt phẳng ngang)
        Vector3 directionFromCamera = transform.position - cameraTransform.position;
        directionFromCamera.y = 0f;
        if (directionFromCamera.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionFromCamera);
            transform.rotation = targetRotation;
        }
        hasInitialized = true;
    }

    private void LateUpdate()
    {
        if (cameraTransform == null) return;
        
        // Nếu không cập nhật (isMoving == false) thì chỉ update 1 lần ban đầu
        if (!isMoving) return;

        if (_isSphereControl)
        {
            // Khi là Sphere Control: giữ nguyên vị trí, chỉ xoay trên trục XZ
            Vector3 directionFromCamera = transform.position - cameraTransform.position;
            directionFromCamera.y = 0f;
            directionFromCamera.Normalize();
            
            if (directionFromCamera.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionFromCamera);
                // Giữ lại giá trị Y hiện tại không thay đổi
                Vector3 currentEuler = transform.eulerAngles;
                Vector3 newEuler = targetRotation.eulerAngles;
                newEuler.y = currentEuler.y;
                transform.rotation = Quaternion.Euler(newEuler);
            }
            return;
        }

        if (!_isObject)
        {
            // Nếu không là object, cập nhật vị trí luôn dựa theo camera
            Vector3 targetPosition = cameraTransform.position + cameraTransform.forward * fixedDistanceFromCamera;
            targetPosition.y = cameraTransform.position.y;
            transform.position = targetPosition;

            Vector3 directionFromCamera = transform.position - cameraTransform.position;
            directionFromCamera.y = 0f;
            if (directionFromCamera.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionFromCamera);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothSpeed * Time.deltaTime);
            }
        }
        else
        {
            // Nếu là object, chỉ cập nhật trục Y
            Vector3 currentPosition = transform.position;
            transform.position = new Vector3(
                currentPosition.x,
                cameraTransform.position.y,
                currentPosition.z
            );
        }
    }
}
