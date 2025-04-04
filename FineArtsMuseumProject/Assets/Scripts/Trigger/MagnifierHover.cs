using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class MagnifierHover : MonoBehaviour
{
    [Header("Camera")]
    public UnityEngine.Camera mainCamera;
    public UnityEngine.Camera zoomCamera;

    [Header("UI")]
    public RawImage magnifierImage; // UI RawImage
    public RectTransform magnifierFrame;

    [Header("Settings")]
    public LayerMask targetLayer;
    public float zoomFactor = 3f;
    public float cameraOffset = 0.3f;

    [Header("XR Settings")]
    public XRRayInteractor leftRayInteractor;
    public XRRayInteractor rightRayInteractor;

    public Transform ignoredRoot;


    void Start()
    {
        magnifierImage.gameObject.SetActive(false);
        if (magnifierFrame != null)
            magnifierFrame.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        magnifierImage.gameObject.SetActive(false);
        if (magnifierFrame != null)
            magnifierFrame.gameObject.SetActive(false);
    }

    void Update()
    {
        if (PlatformManager.Instance.IsVR)
        {
            if (!TryHandleRayInteractor(leftRayInteractor))
            {
                TryHandleRayInteractor(rightRayInteractor);
            }
        }
        else
        {
            HandleMouseHover();
        }
    }

    void HandleMouseHover()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, targetLayer))
        {
            if (hit.transform.CompareTag("Player") || (ignoredRoot && hit.transform.IsChildOf(ignoredRoot))) return;

            Vector3 hitPos = hit.point;
            Vector3 uiWorldPos = hitPos + hit.normal * cameraOffset;

            magnifierImage.gameObject.SetActive(true);
            if (magnifierFrame != null)
                magnifierFrame.gameObject.SetActive(true);

            Vector3 screenPos = mainCamera.WorldToScreenPoint(uiWorldPos);
            magnifierImage.rectTransform.position = screenPos;
            if (magnifierFrame != null)
            {
                ConvertScreenToWorldCanvasPosition(screenPos, magnifierFrame);
            }

            zoomCamera.transform.position = uiWorldPos;
            zoomCamera.transform.rotation = Quaternion.LookRotation(-hit.normal);
            zoomCamera.fieldOfView = mainCamera.fieldOfView / zoomFactor;
        }
        else
        {
            magnifierImage.gameObject.SetActive(false);
            if (magnifierFrame != null)
                magnifierFrame.gameObject.SetActive(false);
        }
    }

    bool TryHandleRayInteractor(XRRayInteractor interactor)
    {
        if (interactor == null || !interactor.enabled)
            return false;

        if (interactor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            if (((1 << hit.collider.gameObject.layer) & targetLayer.value) != 0)
            {
                if (hit.transform.CompareTag("Player") || (ignoredRoot && hit.transform.IsChildOf(ignoredRoot))) return false;

                Vector3 hitPos = hit.point;
                Vector3 camOffsetDir = (mainCamera.transform.position - hitPos).normalized;
                Vector3 uiWorldPos = hitPos + camOffsetDir * cameraOffset;

                magnifierImage.gameObject.SetActive(true);
                if (magnifierFrame != null)
                    magnifierFrame.gameObject.SetActive(true);

                Vector3 screenPosition = mainCamera.WorldToScreenPoint(uiWorldPos);
                ConvertScreenToWorldCanvasPosition(screenPosition, magnifierImage.rectTransform);
                if (magnifierFrame != null)
                {
                    ConvertScreenToWorldCanvasPosition(screenPosition, magnifierFrame);
                }

                zoomCamera.transform.position = uiWorldPos;
                zoomCamera.transform.rotation = Quaternion.LookRotation(hitPos - mainCamera.transform.position);
                zoomCamera.fieldOfView = mainCamera.fieldOfView / zoomFactor;

                return true;
            }
            
        }else
        {
            magnifierImage.gameObject.SetActive(false);
            if (magnifierFrame != null)
                magnifierFrame.gameObject.SetActive(false);
        }

        return false;
    }

    void ConvertScreenToWorldCanvasPosition(Vector3 screenPosition, RectTransform targetRect)
    {
        if (targetRect == null || mainCamera == null) return;

        Canvas canvas = targetRect.GetComponentInParent<Canvas>();
        if (canvas == null || canvas.renderMode != RenderMode.WorldSpace) return;

        Ray ray = mainCamera.ScreenPointToRay(screenPosition);
        Plane canvasPlane = new Plane(canvas.transform.forward, canvas.transform.position);

        if (canvasPlane.Raycast(ray, out float distance))
        {
            Vector3 worldPoint = ray.GetPoint(distance);
            targetRect.position = worldPoint;
            targetRect.rotation = Quaternion.LookRotation(canvas.transform.forward);
        }
    }
}
