using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class VRObjectRotator : MonoBehaviour
{
    [Header("Cài đặt xoay")]
    [Tooltip("Tốc độ xoay (độ/giây)")]
    public float rotationSpeed = 45f;

    [Header("XR Ray Interactors - Controller")]
    public XRRayInteractor leftControllerRay;
    public XRRayInteractor rightControllerRay;

    [Header("XR Ray Interactors - Hand Tracking")]
    public XRRayInteractor leftHandRay;
    public XRRayInteractor rightHandRay;

    private XRRayInteractor activeInteractor = null;
    private bool isRotating = false;

    private Quaternion initialObjectRotation;
    private Quaternion initialHandRotation;

    private float leftGripValue = 0f;
    private float rightGripValue = 0f;

    void Update()
    {
        UpdateGripValues();

        if (!TryHandleInteractor(leftControllerRay, leftGripValue))
        {
            if (!TryHandleInteractor(leftHandRay, leftGripValue))
            {
                if (!TryHandleInteractor(rightControllerRay, rightGripValue))
                {
                    TryHandleInteractor(rightHandRay, rightGripValue);
                }
            }
        }
    }

    bool TryHandleInteractor(XRRayInteractor interactor, float gripValue)
    {
        if (interactor == null || !interactor.enabled) return false;

        if (interactor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            if (hit.transform == transform)
            {
                if (gripValue > 0.5f)
                {
                    if (!isRotating)
                    {
                        StartRotate(interactor);
                    }
                }
                else
                {
                    if (activeInteractor == interactor)
                    {
                        StopRotate();
                    }
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
            transform.rotation = initialObjectRotation * deltaRot;
        }
    }

    void UpdateGripValues()
    {
        leftGripValue = GetGripValue(InputDeviceRole.LeftHanded);
        rightGripValue = GetGripValue(InputDeviceRole.RightHanded);
    }

    float GetGripValue(InputDeviceRole role)
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(
            role == InputDeviceRole.LeftHanded ? XRNode.LeftHand : XRNode.RightHand);

        if (device.TryGetFeatureValue(CommonUsages.grip, out float value))
        {
            return value;
        }

        return 0f;
    }
}
