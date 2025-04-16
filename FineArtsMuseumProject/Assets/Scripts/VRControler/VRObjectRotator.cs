using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class VRObjectRotator : MonoBehaviour
{
    [Header("Rotation Speeds")]
    [Tooltip("Tốc độ xoay khi di chuyển tay theo trục x của tay (deltaX)")]
    public float rotationSpeedFromPosition = 200f;

    [Tooltip("Tốc độ xoay khi so sánh góc quay tay (angleFromHand)")]
    public float rotationSpeedFromRotation = 1f;

    [Header("XR Ray Interactors (Auto Assign)")]
    [Tooltip("Ray tương ứng controller tay trái (được quét tự động)")]
    public XRRayInteractor leftControllerRay;
    [Tooltip("Ray tương ứng controller tay phải (được quét tự động)")]
    public XRRayInteractor rightControllerRay;
    [Tooltip("Ray tương ứng handtracking tay trái (được quét tự động)")]
    public XRRayInteractor leftHandRay;
    [Tooltip("Ray tương ứng handtracking tay phải (được quét tự động)")]
    public XRRayInteractor rightHandRay;

    [Header("Rotation Settings")]
    [Tooltip("Nếu true, chỉ xoay quanh trục Y (thường dùng cho tranh ảnh đứng)")]
    public bool isPicture = false;

    // Lưu Interactor nào đang xoay object
    private XRRayInteractor activeInteractor = null;
    private bool isRotating = false;
    
    // Lưu rotation/position ban đầu để tính delta
    private Quaternion initialObjectRotation;
    private Quaternion initialHandRotation;
    private Vector3 initialHandPosition;

    void Awake()
    {
        // Gọi một lần khi scene khởi tạo để tìm & cache XRRayInteractor
        XRRayInteractor[] interactors = FindObjectsOfType<XRRayInteractor>();
        foreach (var interactor in interactors)
        {
            string lowerName = interactor.gameObject.name.ToLower();
            if (leftControllerRay == null && lowerName.Contains("left") && lowerName.Contains("controller"))
                leftControllerRay = interactor;
            if (rightControllerRay == null && lowerName.Contains("right") && lowerName.Contains("controller"))
                rightControllerRay = interactor;
            if (leftHandRay == null && lowerName.Contains("left") && lowerName.Contains("hand"))
                leftHandRay = interactor;
            if (rightHandRay == null && lowerName.Contains("right") && lowerName.Contains("hand"))
                rightHandRay = interactor;
        }
        
        Debug.Log("Auto-Assigned XRRayInteractors:" +
                  "\nLeftControllerRay: " + leftControllerRay +
                  "\nRightControllerRay: " + rightControllerRay +
                  "\nLeftHandRay: " + leftHandRay +
                  "\nRightHandRay: " + rightHandRay);
    }

    void Update()
    {
        // Kiểm tra lần lượt: leftControllerRay, leftHandRay, rightControllerRay, rightHandRay
        if (!TryHandleInteractor(leftControllerRay))
        {
            if (!TryHandleInteractor(leftHandRay))
            {
                if (!TryHandleInteractor(rightControllerRay))
                {
                    TryHandleInteractor(rightHandRay);
                }
            }
        }
    }

    bool TryHandleInteractor(XRRayInteractor interactor)
    {
        if (interactor == null || !interactor.enabled)
            return false;

        // Lấy ActionBasedController để đọc trigger
        var abc = interactor.GetComponentInParent<ActionBasedController>();
        if (abc == null)
            return false;
        
        // Đọc giá trị trigger
        float triggerValue = abc.activateActionValue.action.ReadValue<float>();
        bool isTriggerPressed = (triggerValue > 0.1f);

        // 1) Nếu đang xoay bởi interactor này
        if (activeInteractor == interactor)
        {
            if (!isTriggerPressed)
            {
                // Buông trigger => dừng xoay
                StopRotate();
            }
            return true;
        }

        // 2) Nếu chưa xoay và interactor raycast vào object này
        if (!isRotating && interactor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            if (hit.transform == transform)
            {
                if (isTriggerPressed)
                {
                    // Bắt đầu xoay
                    StartRotate(interactor);
                }
                return true;
            }
        }
        else
        {
            // Nếu interactor này đang xoay mà triggerValue = 0 => dừng
            if (activeInteractor == interactor)
            {
                StopRotate();
            }
        }

        return false;
    }

    void StartRotate(XRRayInteractor interactor)
    {
        isRotating = true;
        activeInteractor = interactor;
        initialObjectRotation = transform.rotation;
        initialHandRotation = interactor.transform.rotation;
        initialHandPosition = interactor.transform.position;
    }

    void StopRotate()
    {
        isRotating = false;
        activeInteractor = null;
    }

    void LateUpdate()
    {
        if (isRotating && activeInteractor != null)
        {
            // Tính delta rotation
            Quaternion currentHandRotation = activeInteractor.transform.rotation;
            Quaternion deltaRot = currentHandRotation * Quaternion.Inverse(initialHandRotation);

            // Tính độ lệch vị trí tay (deltaX) theo trục right để xoay
            Vector3 currentHandPosition = activeInteractor.transform.position;
            float deltaX = Vector3.Dot(currentHandPosition - initialHandPosition, activeInteractor.transform.right);
            float rotationFromPosition = deltaX * rotationSpeedFromPosition;

            if (isPicture)
            {
                // Chỉ xoay quanh Y
                float angleFromHand = Quaternion.Angle(initialHandRotation, currentHandRotation);
                float finalYaw = rotationFromPosition + (angleFromHand * rotationSpeedFromRotation);
                transform.rotation = initialObjectRotation * Quaternion.Euler(0f, finalYaw, 0f);
            }
            else
            {
                // Xoay tự do: object xoay theo delta rotation
                transform.rotation = initialObjectRotation * deltaRot;
            }
        }
    }
}
