using Trigger;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class VRObjectRotator : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float rotationSpeedFromPosition = 200f;
    public float rotationSpeedFromRotation = 1f;
    public bool isPicture = false;
    public bool useCameraRotation = false; // NEW: dùng camera xoay thay vì tay cầm

    [Header("XR Ray Interactors (kéo vào Inspector)")]
    public XRRayInteractor leftControllerRay;
    public XRRayInteractor rightControllerRay;
    public XRRayInteractor leftHandRay;
    public XRRayInteractor rightHandRay;

    [Header("Input threshold")]
    public float pressThreshold = 0.25f;

    [Header("Smoothing")]
    [Range(0.03f, .3f)]
    public float smoothTime = 0.08f;
    float velYaw, velPitch, velRoll;

    [Header("Camera góc nhìn player")]
    public UnityEngine.Camera playerCamera;

    /* ------------- trạng thái ---------------- */
    XRRayInteractor activeInteractor;
    bool isRotating;
    Quaternion lastHandRot;
    Vector3 lastHandPos;
    Vector3 lastCameraForward;

    [HideInInspector] public bool allowRotate = true;

    void Update()
    {
        Debug.Log(allowRotate);

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

        float sel = abc.selectActionValue.action != null ? abc.selectActionValue.action.ReadValue<float>() : 0f;
        float act = abc.activateActionValue.action != null ? abc.activateActionValue.action.ReadValue<float>() : 0f;
        bool pressed = (sel > pressThreshold) || (act > pressThreshold);

        if (activeInteractor == it)
        {
            if (!pressed || !allowRotate || (isPicture && !IsRayHittingSelf(it)))
                StopRotate();
            return true;
        }

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
        isRotating = true;
        lastHandRot = it.transform.rotation;
        lastHandPos = it.transform.position;
        lastCameraForward = playerCamera.transform.forward;
    }

    void StopRotate()
    {
        isRotating = false;
        activeInteractor = null;
    }

    void LateUpdate()
    {
        if (!allowRotate || !isRotating || activeInteractor == null) return;

        // Nếu là tranh và ray không còn chạm vật thể thì dừng
        if (isPicture && !IsRayHittingSelf(activeInteractor))
        {
            StopRotate();
            return;
        }

        if (useCameraRotation)
        {
            // Xoay dựa theo góc camera
            Vector3 currentForward = playerCamera.transform.forward;
            float yawAngle = Vector3.SignedAngle(lastCameraForward, currentForward, Vector3.up);
            transform.rotation = Quaternion.Euler(0f, yawAngle * rotationSpeedFromRotation, 0f) * transform.rotation;
            lastCameraForward = currentForward;
        }
        else
        {
            // Xoay dựa theo tay cầm
            Quaternion curHandRot = activeInteractor.transform.rotation;
            Quaternion deltaRot = curHandRot * Quaternion.Inverse(lastHandRot);

            Vector3 moveDir = activeInteractor.transform.position - lastHandPos;
            float rotPos = Vector3.Dot(moveDir, activeInteractor.transform.right) * rotationSpeedFromPosition;

            if (isPicture)
            {
                var lastF = lastHandRot * Vector3.forward;
                var curF = curHandRot * Vector3.forward;
                float yawSigned = Vector3.SignedAngle(lastF, curF, Vector3.up);
                float finalYaw = rotPos - yawSigned * rotationSpeedFromRotation;
                transform.rotation = transform.rotation * Quaternion.Euler(0f, finalYaw, 0f);
            }
            else
            {
                // Cho phép xoay nhanh hơn nếu muốn
                float angle;
                Vector3 axis;
                deltaRot.ToAngleAxis(out angle, out axis);
                angle *= 2f; // Tăng tốc độ xoay
                deltaRot = Quaternion.AngleAxis(angle, axis);

                transform.rotation = transform.rotation * Quaternion.Inverse(deltaRot);
            }

            lastHandRot = curHandRot;
            lastHandPos = activeInteractor.transform.position;
        }
    }
}
