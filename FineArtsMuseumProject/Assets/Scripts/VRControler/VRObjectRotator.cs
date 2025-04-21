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

    [Header("XR Ray Interactors (kéo tay vào Inspector)")]
    public XRRayInteractor leftControllerRay;
    public XRRayInteractor rightControllerRay;
    public XRRayInteractor leftHandRay;
    public XRRayInteractor rightHandRay;

    [Header("Press Threshold (0-1)")]
    public float pressThreshold = 0.25f;

    // Runtime
    private XRRayInteractor activeInteractor = null;
    private bool isRotating = false;
    private Quaternion lastHandRot;
    private Vector3 lastHandPos;


    public static bool AllowRotate = false;
    void Update()
    {
        if (MagnifierHover.IsActive())  return;
        if (!HandleInteractor(leftControllerRay))
            if (!HandleInteractor(leftHandRay))
                if (!HandleInteractor(rightControllerRay))
                    HandleInteractor(rightHandRay);
    }

    bool HandleInteractor(XRRayInteractor interactor)
    {
        if (interactor == null || !interactor.enabled) return false;

        var abc = interactor.GetComponentInParent<ActionBasedController>();
        if (abc == null) return false;

        float selectValue = 0f;
        if (abc.selectActionValue.action != null)
            selectValue = abc.selectActionValue.action.ReadValue<float>();

        float activateValue = 0f;
        if (abc.activateActionValue.action != null)
            activateValue = abc.activateActionValue.action.ReadValue<float>();

        bool isPressed = (selectValue > pressThreshold) || (activateValue > pressThreshold);

        if (activeInteractor == interactor)
        {
            if (!isPressed || AllowRotate == false)
                StopRotate();
            return true;
        }

        if (!isRotating && isPressed &&
            interactor.TryGetCurrent3DRaycastHit(out RaycastHit hit) &&
            hit.transform == transform)
        {
            if (PaintingDetailManager.Instance.IsChangeArcSlider()) return false;

            StartRotate(interactor);
            return true;
        }

        return false;
    }
    
    void StartRotate(XRRayInteractor interactor)
    {
        activeInteractor = interactor;
        isRotating = true;
        lastHandRot = interactor.transform.rotation;
        lastHandPos = interactor.transform.position;
    }

    void StopRotate()
    {
        isRotating = false;
        activeInteractor = null;
    }

    void LateUpdate()
    {
        if (!isRotating || activeInteractor == null) return;

        Quaternion currentHandRot = activeInteractor.transform.rotation;
        Quaternion deltaRot = currentHandRot * Quaternion.Inverse(lastHandRot);

        Vector3 currentHandPos = activeInteractor.transform.position;
        Vector3 moveDir = currentHandPos - lastHandPos;
        Vector3 handRight = activeInteractor.transform.right;
        float deltaX = Vector3.Dot(moveDir, handRight);
        float rotationFromPosition = deltaX * rotationSpeedFromPosition;

        if (isPicture)
        {
            // Tính yaw bằng hướng tay hiện tại so với trước
            Vector3 lastForward    = lastHandRot    * Vector3.forward;
            Vector3 currentForward = currentHandRot * Vector3.forward;

            float signedYaw = Vector3.SignedAngle(lastForward, currentForward, Vector3.up);
            float finalYaw = rotationFromPosition - (signedYaw * rotationSpeedFromRotation);

            transform.rotation = transform.rotation * Quaternion.Euler(0f, finalYaw, 0f);
        }

        else
        {
            transform.rotation = transform.rotation * Quaternion.Inverse(deltaRot);
        }

        lastHandRot = currentHandRot;
        lastHandPos = currentHandPos;
    }
}
