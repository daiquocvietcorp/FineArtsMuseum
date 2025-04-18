using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class VRObjectRotator : MonoBehaviour
{
    [Header("Rotation Speeds")]
    public float rotationSpeedFromPosition = 200f;   // khi kéo tay
    public float rotationSpeedFromRotation = 1f;     // khi xoay cổ tay
    [Tooltip("Chỉ xoay trục Y nếu true (tranh, panel…)")]
    public bool  isPicture = false;

    [Header("XR Ray Interactors (kéo bằng tay)")]
    public XRRayInteractor leftControllerRay,  rightControllerRay;
    public XRRayInteractor leftHandRay,       rightHandRay;

    /* ───────── trạng thái runtime ───────── */
    XRRayInteractor activeInteractor;
    bool        isRotating;
    Quaternion  initialObjectRot, initialHandRot;
    Vector3     initialHandPos;

    /* ───────── main loop ───────── */
    void Update()
    {
        if (!TryHandle(leftControllerRay))
            if (!TryHandle(leftHandRay))
                if (!TryHandle(rightControllerRay))
                    TryHandle(rightHandRay);
    }

    /* ───────── xử lý 1 interactor ───────── */
    bool TryHandle(XRRayInteractor it)
    {
        if (it == null || !it.enabled) return false;

        /* Lấy ActionBasedController để đọc cả Select & Activate */
        var abc = it.GetComponentInParent<ActionBasedController>();
        if (abc == null) return false;

        float sel = 0f, act = 0f;
        if (abc.selectActionValue  .action != null) sel = abc.selectActionValue  .action.ReadValue<float>();
        if (abc.activateActionValue.action != null) act = abc.activateActionValue.action.ReadValue<float>();

        bool pressed = (sel > 0.5f) || (act > 0.5f);   // ← CHỈNH: chấp nhận Select _hoặc_ Activate

        /* Đang xoay bằng interactor này? */
        if (activeInteractor == it)
        {
            if (!pressed) StopRotate();
            return true;
        }

        /* Chưa xoay – kiểm tra raycast trúng object & đang bấm */
        if (!isRotating && pressed &&
            it.TryGetCurrent3DRaycastHit(out RaycastHit hit) &&
            hit.transform == transform)
        {
            StartRotate(it);
            return true;
        }
        return false;
    }

    /* ───────── bắt đầu / dừng ───────── */
    void StartRotate(XRRayInteractor it)
    {
        activeInteractor  = it;
        isRotating        = true;
        initialObjectRot  = transform.rotation;
        initialHandRot    = it.transform.rotation;
        initialHandPos    = it.transform.position;
    }
    void StopRotate()
    {
        isRotating       = false;
        activeInteractor = null;
    }

    /* ───────── áp xoay mỗi khung ───────── */
    void LateUpdate()
    {
        if (!isRotating || activeInteractor == null) return;

        Quaternion curHandRot = activeInteractor.transform.rotation;
        Quaternion deltaRot   = curHandRot * Quaternion.Inverse(initialHandRot);

        Vector3 curHandPos = activeInteractor.transform.position;
        float   deltaX     = Vector3.Dot(curHandPos - initialHandPos,
                                         activeInteractor.transform.right);
        float   rotPos     = deltaX * rotationSpeedFromPosition;

        if (isPicture)
        {
            float angHand = Quaternion.Angle(initialHandRot, curHandRot);
            float yaw     = rotPos + angHand * rotationSpeedFromRotation;
            transform.rotation = initialObjectRot * Quaternion.Euler(0f, yaw, 0f);
        }
        else
        {
            transform.rotation = initialObjectRot * deltaRot;
        }
    }
}
