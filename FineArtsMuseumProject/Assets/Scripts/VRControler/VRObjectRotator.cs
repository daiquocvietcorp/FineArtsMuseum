using Trigger;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class VRObjectRotator : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float rotationSpeedFromPosition = 200f;
    public float rotationSpeedFromRotation = 1f;
    public bool  isPicture = false;

    [Header("XR Ray Interactors (kéo vào Inspector)")]
    public XRRayInteractor leftControllerRay;
    public XRRayInteractor rightControllerRay;
    public XRRayInteractor leftHandRay;
    public XRRayInteractor rightHandRay;

    [Header("Input threshold")]
    public float pressThreshold = 0.25f;

    /* ------------- trạng thái ---------------- */
    XRRayInteractor activeInteractor;
    bool        isRotating;
    Quaternion  lastHandRot;
    Vector3     lastHandPos;

    /* ------------- NEW: cờ riêng cho từng object ------------- */
    [HideInInspector] public bool allowRotate = true;   // mặc định cho phép

    /* --------------------------------------------------------- */
    void Update()
    {
        if (!allowRotate || MagnifierHover.IsActive()) return;

        if (!Handle(leftControllerRay))
            if (!Handle(leftHandRay))
                if (!Handle(rightControllerRay))
                    Handle(rightHandRay);
    }

    bool Handle(XRRayInteractor it)
    {
        if (it == null || !it.enabled) return false;

        var abc = it.GetComponentInParent<ActionBasedController>();
        if (abc == null) return false;

        float sel = abc.selectActionValue.action != null   ? abc.selectActionValue.action.ReadValue<float>()   : 0f;
        float act = abc.activateActionValue.action != null ? abc.activateActionValue.action.ReadValue<float>() : 0f;
        bool pressed = (sel > pressThreshold) || (act > pressThreshold);

        /* --- đang xoay bởi interactor này --- */
        if (activeInteractor == it)
        {
            if (!pressed || !allowRotate || !IsRayHittingSelf(it))
                StopRotate();
            return true;
        }

        /* --- khởi động xoay --- */
        if (!isRotating && pressed && allowRotate && IsRayHittingSelf(it))
        {
            if (PaintingDetailManager.Instance.IsChangeArcSlider()) return false;
            StartRotate(it);
            return true;
        }

        return false;
    }

    bool IsRayHittingSelf(XRRayInteractor it) =>
        it.TryGetCurrent3DRaycastHit(out RaycastHit h) && h.transform == transform;

    void StartRotate(XRRayInteractor it)
    {
        activeInteractor = it;
        isRotating       = true;
        lastHandRot      = it.transform.rotation;
        lastHandPos      = it.transform.position;
    }

    void StopRotate()
    {
        isRotating       = false;
        activeInteractor = null;
    }

    void LateUpdate()
    {
        if (!allowRotate || !isRotating || activeInteractor == null) return;

        /* Nếu ray đã rời tranh → dừng */
        if (!IsRayHittingSelf(activeInteractor))
        {
            StopRotate();
            return;
        }

        Quaternion curHandRot = activeInteractor.transform.rotation;
        Quaternion deltaRot   = curHandRot * Quaternion.Inverse(lastHandRot);

        Vector3 moveDir = activeInteractor.transform.position - lastHandPos;
        float   rotPos  = Vector3.Dot(moveDir, activeInteractor.transform.right) * rotationSpeedFromPosition;

        if (isPicture)
        {
            var lastF = lastHandRot * Vector3.forward;
            var curF  = curHandRot  * Vector3.forward;
            float yawSigned = Vector3.SignedAngle(lastF, curF, Vector3.up);
            float finalYaw  = rotPos - yawSigned * rotationSpeedFromRotation;
            transform.rotation = transform.rotation * Quaternion.Euler(0f, finalYaw, 0f);
        }
        else
        {
            transform.rotation = transform.rotation * Quaternion.Inverse(deltaRot);
        }

        lastHandRot = curHandRot;
        lastHandPos = activeInteractor.transform.position;
    }
}
