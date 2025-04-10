using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class TwoHandScaler : MonoBehaviour
{
    [Header("XR Ray Interactors - Controller")]
    public XRRayInteractor leftControllerRay;
    public XRRayInteractor rightControllerRay;

    [Header("XR Ray Interactors - Hand Tracking")]
    public XRRayInteractor leftHandRay;
    public XRRayInteractor rightHandRay;

    [Header("Input Actions (Pinch Strength)")]
    public InputActionProperty leftControllerPinchAction;
    public InputActionProperty rightControllerPinchAction;
    public InputActionProperty leftHandPinchAction;
    public InputActionProperty rightHandPinchAction;

    [Header("Object to Scale")]
    public Transform targetObject;

    [Header("Settings")]
    public float pinchThreshold = 0.8f;
    public float smoothFactor = 8f;

    private float initialDistance = 0f;
    private Vector3 initialScale;

    void Update()
    {
        var activeInteractors = GetPinchingInteractors();

        if (activeInteractors.Count == 2)
        {
            Vector3 posA = activeInteractors[0].transform.position;
            Vector3 posB = activeInteractors[1].transform.position;

            float currentDistance = Vector3.Distance(posA, posB);

            if (initialDistance == 0f)
            {
                initialDistance = currentDistance;
                initialScale = targetObject.localScale;
            }

            float scaleRatio = currentDistance / initialDistance;
            Vector3 targetScale = initialScale * scaleRatio;
            targetObject.localScale = Vector3.Lerp(targetObject.localScale, targetScale, Time.deltaTime * smoothFactor);
        }
        else
        {
            initialDistance = 0f;
        }
    }

    List<XRRayInteractor> GetPinchingInteractors()
    {
        List<XRRayInteractor> list = new List<XRRayInteractor>();

        CheckInteractor(leftControllerRay, leftControllerPinchAction.action.ReadValue<float>(), list);
        CheckInteractor(rightControllerRay, rightControllerPinchAction.action.ReadValue<float>(), list);
        CheckInteractor(leftHandRay, leftHandPinchAction.action.ReadValue<float>(), list);
        CheckInteractor(rightHandRay, rightHandPinchAction.action.ReadValue<float>(), list);

        return list;
    }

    void CheckInteractor(XRRayInteractor interactor, float pinchValue, List<XRRayInteractor> list)
    {
        if (interactor == null || !interactor.enabled) return;

        if (pinchValue > pinchThreshold)
        {
            if (interactor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
            {
                if (hit.transform == transform)
                {
                    list.Add(interactor);
                }
            }
        }
    }
}
