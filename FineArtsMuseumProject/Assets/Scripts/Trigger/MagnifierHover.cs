using System;
using DesignPatterns;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class MagnifierHover : MonoBehaviour,IPointerDownHandler, IPointerUpHandler, IDragHandler
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
    
    [Header("Hand Tracking Rays")]
    public XRRayInteractor leftHandRayInteractor;
    public XRRayInteractor rightHandRayInteractor;


    public Transform ignoredRoot;
    
    public static GameObject magnifierCanvas;

    void Start()
    {
        magnifierCanvas = ignoredRoot.gameObject;
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
    
    public void OnPointerDown(PointerEventData eventData)
    {
        // Debug.Log("PointerDown");
        // if (PlatformManager.Instance.IsCloud)
        // {
        //     HandlePointerEvent(eventData.position);
        // }
        if (PlatformManager.Instance.IsCloud || PlatformManager.Instance.IsWebGL)
        {
            var parentCanvas = magnifierImage.GetComponentInParent<Canvas>();
            MoveMagnifierToPointer(parentCanvas, eventData.position);
            HandlePointerEvent(eventData.position);
        }
        
    }
    
    // Sự kiện khi pointer kéo (drag)
    public void OnDrag(PointerEventData eventData)
    {
        if (PlatformManager.Instance.IsCloud || PlatformManager.Instance.IsWebGL)
        {
            HandlePointerEvent(eventData.position);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Debug.Log("PointerUp");
        if (PlatformManager.Instance.IsCloud)
        {
            HideMagnifier();
        }
    }
    
    private void MoveMagnifierToPointer(Canvas parentCanvas, Vector2 pointerPos)
    {
        if (!magnifierImage) return;

        // Bật kính lúp
        magnifierImage.gameObject.SetActive(true);
        if (magnifierFrame != null)
            magnifierFrame.gameObject.SetActive(true);

        // Tuỳ thuộc vào renderMode của Canvas
        if (parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            // Screen Space Overlay: gán thẳng
            magnifierImage.rectTransform.position = pointerPos;
            if (magnifierFrame != null)
                magnifierFrame.position = pointerPos;
        }
        else
        {
            // Screen Space Camera hoặc World Space: phải đổi toạ độ
            RectTransform canvasRect = parentCanvas.transform as RectTransform;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvasRect, 
                    pointerPos, 
                    parentCanvas.worldCamera, 
                    out Vector2 localPoint))
            {
                magnifierImage.rectTransform.localPosition = localPoint;
                if (magnifierFrame != null)
                    magnifierFrame.localPosition = localPoint;
            }
        }
    }
    
    /// <summary>
    /// Xử lý các event pointer (pointer down và drag) cho chế độ Cloud
    /// </summary>
    /// <param name="pointerPos">Vị trí của pointer (màn hình)</param>
    void HandlePointerEvent(Vector3 pointerPos)
    {
        Ray ray = mainCamera.ScreenPointToRay(pointerPos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f, targetLayer))
        {
            //magnifierImage.gameObject.SetActive(false);
            // Nếu trúng đối tượng có tag "Player" thì không xử lý
            if (hit.transform.tag == "Player") return;

            // Hiển thị kính lúp
            if (magnifierFrame != null)
                magnifierFrame.gameObject.SetActive(true);

            // Cập nhật vị trí của UI kính lúp theo vị trí pointer
            magnifierImage.rectTransform.position = pointerPos;
            if (magnifierFrame != null)
                magnifierFrame.position = pointerPos;

            Vector3 hitPos = hit.point;
            Vector3 normal = hit.normal;

            if (!mainCamera.orthographic)
            {
                // Với Perspective
                zoomCamera.transform.position = hitPos + normal * cameraOffset;
                zoomCamera.transform.rotation = Quaternion.LookRotation(-normal);
                zoomCamera.fieldOfView = mainCamera.fieldOfView / zoomFactor;
            }
            else
            {
                // Với Orthographic
                zoomCamera.transform.position = hitPos + normal * cameraOffset;
                zoomCamera.transform.rotation = Quaternion.LookRotation(-normal);
                zoomCamera.orthographicSize = mainCamera.orthographicSize / zoomFactor;
            }
            
            magnifierImage.gameObject.SetActive(true);
            
        }
        else
        {
            HideMagnifier();
        }
    }

    

    void Update()
    {
        if (PlatformManager.Instance.IsVR)
        {
            if (!TryHandleRayInteractor(leftHandRayInteractor))
            {
                if (!TryHandleRayInteractor(rightHandRayInteractor))
                {
                    // Nếu không có hand ray trúng thì thử controller ray
                    if (!TryHandleRayInteractor(leftRayInteractor))
                    {
                        TryHandleRayInteractor(rightRayInteractor);
                    }
                }
            }
        }
        else if (!PlatformManager.Instance.IsCloud && !PlatformManager.Instance.IsWebGL)
        {
            HandleMouseHover();
        }
    }

    void HandleMouseHover()
    {
        Vector3 mousePos = Input.mousePosition;
        Ray ray = mainCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, targetLayer))
        {
            if (hit.transform.tag == "Player") return;

            // Show kính lúp
            magnifierImage.gameObject.SetActive(true);
            if (magnifierFrame != null)
                magnifierFrame.gameObject.SetActive(true);

            magnifierImage.rectTransform.position = mousePos;
            if (magnifierFrame != null)
                magnifierFrame.position = mousePos;

            Vector3 hitPos = hit.point;
            Vector3 normal = hit.normal;

            if (!mainCamera.orthographic)
            {
                // Với Perspective
                zoomCamera.transform.position = hitPos + normal * cameraOffset;
                zoomCamera.transform.rotation = Quaternion.LookRotation(-normal);
                zoomCamera.fieldOfView = mainCamera.fieldOfView / zoomFactor;
            }
            else
            {
                // Với Orthographic
                zoomCamera.transform.position = hitPos + normal * cameraOffset;
                zoomCamera.transform.rotation = Quaternion.LookRotation(-normal);
                zoomCamera.orthographicSize = mainCamera.orthographicSize / zoomFactor;
            }
        }
        else
        {
            HideMagnifier();
        }
    }
    
    
    void HideMagnifier()
    {
        magnifierImage.gameObject.SetActive(false);
        if (magnifierFrame != null)
            magnifierFrame.gameObject.SetActive(false);
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

                // Vector3 hitPos = hit.point;
                // Vector3 camOffsetDir = (mainCamera.transform.position - hitPos).normalized;
                // Vector3 uiWorldPos = hitPos + camOffsetDir * cameraOffset;
                Vector3 hitPos  = hit.point;
                Vector3 normal  = hit.normal;                     // ◀︎ lấy pháp tuyến
                Vector3 uiWorldPos = hitPos + normal * cameraOffset;
                magnifierImage.gameObject.SetActive(true);
                if (magnifierFrame != null)
                    magnifierFrame.gameObject.SetActive(true);

                Vector3 screenPosition = mainCamera.WorldToScreenPoint(uiWorldPos);
                ConvertScreenToWorldCanvasPosition(screenPosition, magnifierImage.rectTransform);
                if (magnifierFrame != null)
                {
                    ConvertScreenToWorldCanvasPosition(screenPosition, magnifierFrame);
                }

                // zoomCamera.transform.position = uiWorldPos;
                // zoomCamera.transform.rotation = Quaternion.LookRotation(hitPos - mainCamera.transform.position);
                // zoomCamera.fieldOfView = mainCamera.fieldOfView / zoomFactor;
                
                zoomCamera.transform.position = uiWorldPos;
                zoomCamera.transform.rotation = Quaternion.LookRotation(-normal); // ◀︎ vuông góc
                zoomCamera.fieldOfView = mainCamera.fieldOfView / zoomFactor;
                
                return true;
            }
            
        }else
        {
            HideMagnifier();
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


    public static bool IsActive()
    {
        if (magnifierCanvas != null)
        {
            if (magnifierCanvas.activeSelf)
            {
                return true;
            }
           
            
        }
        return false;

        
    }
}
