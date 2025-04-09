using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class TwoHandScaler : MonoBehaviour
{
    [Header("XR Ray Interactors")]
    public XRRayInteractor leftControllerRay;
    public XRRayInteractor rightControllerRay;
    
    [Header("XR Ray Interactors - Hand Tracking")]
    public XRRayInteractor leftHandRay;
    public XRRayInteractor rightHandRay;
    
    [Header("Target to scale")]
    public Transform targetObject;

    [Header("Smooth Settings")]
    public float smoothFactor = 8f;

    private float leftGripValue = 0f;
    private float rightGripValue = 0f;

    private float initialDistance = 0f;
    private Vector3 initialScale;

    void Update()
    {
        UpdateScale();
    }

    private void UpdateScale()
    {
        
        UpdateGripValues();

        XRRayInteractor leftRay = GetActiveRay(leftControllerRay, leftHandRay, leftGripValue);
        XRRayInteractor rightRay = GetActiveRay(rightControllerRay, rightHandRay, rightGripValue);

        if (leftRay != null && rightRay != null)
        {
            Vector3 leftPos = leftRay.transform.position;
            Vector3 rightPos = rightRay.transform.position;

            float currentDistance = Vector3.Distance(leftPos, rightPos);

            if (initialDistance == 0f)
            {
                initialDistance = currentDistance;
                initialScale = targetObject.localScale;
            }

            float scaleRatio = currentDistance / initialDistance;
            targetObject.localScale = Vector3.Lerp(targetObject.localScale, initialScale * scaleRatio, Time.deltaTime * 8f);
        }
        else
        {
            initialDistance = 0f;
        }
    }
    XRRayInteractor GetActiveRay(XRRayInteractor controllerRay, XRRayInteractor handRay, float gripValue)
    {
        if (controllerRay != null && controllerRay.TryGetCurrent3DRaycastHit(out _) && gripValue > 0.5f)
        {
            return controllerRay;
        }

        if (handRay != null && handRay.TryGetCurrent3DRaycastHit(out _) && gripValue > 0.5f)
        {
            return handRay;
        }

        return null;
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