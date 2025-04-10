using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class VRObjectRotator : MonoBehaviour
{
    [Header("Tốc độ xoay")]
    public float rotationSpeedFromPosition = 200f;
    public float rotationSpeedFromRotation = 1f;

    [Header("XR Ray Interactors - Controller")]
    public XRRayInteractor leftControllerRay;
    public XRRayInteractor rightControllerRay;

    [Header("XR Ray Interactors - Hand Tracking")]
    public XRRayInteractor leftHandRay;
    public XRRayInteractor rightHandRay;

    // Để lưu Interactor nào đang xoay
    private XRRayInteractor activeInteractor = null;
    private bool isRotating = false;

    // Lưu trạng thái ban đầu
    private Quaternion initialObjectRotation;
    private Quaternion initialHandRotation;
    private Vector3 initialHandPosition;

    [Header("Chỉ xoay quanh trục Y (tranh ảnh)")]
    public bool isPicture = false;

    void Update()
    {
        // Lần lượt kiểm tra leftController, leftHand, rightController, rightHand
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

        // Lấy ActionBasedController
        var abc = interactor.GetComponentInParent<ActionBasedController>();
        if (abc == null)
            return false; // nếu không có ABC => không xử lý

        // Đọc giá trị trigger
        float triggerValue = abc.activateActionValue.action.ReadValue<float>();
        bool isTriggerPressed = (triggerValue > 0.1f);

        // 1) Nếu đang xoay bởi interactor này
        if (activeInteractor == interactor)
        {
            if (!isTriggerPressed)
            {
                StopRotate();
            }
            return true;
        }

        // 2) Nếu chưa xoay, thử raycast
        if (!isRotating && interactor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            if (hit.transform == transform)
            {
                if (isTriggerPressed)
                {
                    StartRotate(interactor);
                }
                return true;
            }
        }
        else
        {
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
            Quaternion currentHandRotation = activeInteractor.transform.rotation;
            Quaternion deltaRot = currentHandRotation * Quaternion.Inverse(initialHandRotation);

            Vector3 currentHandPosition = activeInteractor.transform.position;
            float deltaX = Vector3.Dot(currentHandPosition - initialHandPosition, activeInteractor.transform.right);
            float rotationFromPosition = deltaX * rotationSpeedFromPosition;

            if (isPicture)
            {
                // Chỉ xoay quanh Y nếu isPicture = true
                float angleFromHand = Quaternion.Angle(initialHandRotation, currentHandRotation);
                float finalYaw = rotationFromPosition + (angleFromHand * rotationSpeedFromRotation);
                transform.rotation = initialObjectRotation * Quaternion.Euler(0f, finalYaw, 0f);
            }
            else
            {
                // Xoay tự do
                transform.rotation = initialObjectRotation * deltaRot;
            }
        }
    }
}
