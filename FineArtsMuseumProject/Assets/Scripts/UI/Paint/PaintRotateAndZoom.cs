using System;
using System.Collections;
using System.Collections.Generic;
using Camera;
using InputController;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PaintRotateAndZoom : MonoBehaviour, IPointerDownHandler, IDragHandler, IScrollHandler, IPointerUpHandler, IBeginDragHandler, IEndDragHandler
{
    [field: SerializeField] private bool isPaint = true;
    
    public float rotationSpeed = 100f;
    public float zoomSpeed = 0.01f;

    public float rotationSpeedMobile = 80f;
    public float zoomSpeedMobile = 0.015f;

    public float minScale = 0.2f;
    public float maxScale = 3f;
    public float resetDuration = 0.5f;

    public Scrollbar zoomScrollbar;
    public bool CanRotateUpDown = false;

    public Vector3 originalScale;
    private Quaternion originalRotation;
    private Coroutine resetCoroutine;

    private Vector3 averageScale;
    private bool updatingFromScroll = false;

    private Dictionary<int, Vector2> activeTouches = new Dictionary<int, Vector2>();
    private float initialDistance;
    private Vector3 initialScale;
    private Vector2 lastPointerPosition;
    private BoxCollider _boxCollider;

    public bool canRotate = true;

    private float CurrentRotationSpeed
    {
        get
        {
            if (PlatformManager.Instance.IsCloud || PlatformManager.Instance.IsMobile || PlatformManager.Instance.IsTomko)
                return rotationSpeedMobile;
            return rotationSpeed;
        }
    }

    private float CurrentZoomSpeed
    {
        get
        {
            if (PlatformManager.Instance.IsCloud || PlatformManager.Instance.IsMobile || PlatformManager.Instance.IsTomko)
                return zoomSpeedMobile;
            return zoomSpeed;
        }
    }

    private void OnEnable()
    {
        var paintCollider = GetComponent<BoxCollider>();
        if (paintCollider != null)
        {
            _boxCollider = paintCollider;
        }
        if (zoomScrollbar != null)
            zoomScrollbar.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        if (zoomScrollbar != null)
            zoomScrollbar.gameObject.SetActive(false);
    }

    void Start()
    {
        originalRotation = transform.rotation;
        averageScale = Vector3.one * ((minScale + maxScale) / 2f);

        if (zoomScrollbar != null)
        {
            zoomScrollbar.onValueChanged.AddListener(OnScrollbarChanged);
            zoomScrollbar.value = GetZoomScrollbarValue(transform.localScale.x);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        activeTouches[eventData.pointerId] = eventData.position;

        if (activeTouches.Count == 2)
        {
            var positions = new List<Vector2>(activeTouches.Values);
            initialDistance = Vector2.Distance(positions[0], positions[1]);
            initialScale = transform.localScale;
        }

        lastPointerPosition = eventData.position;
        MouseInput.Instance.SetIsDragImage(true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!activeTouches.ContainsKey(eventData.pointerId))
            return;

        activeTouches[eventData.pointerId] = eventData.position;

        if (activeTouches.Count == 2)
        {
            if(PlatformManager.Instance.IsTomko) return;
            var positions = new List<Vector2>(activeTouches.Values);
            float currentDistance = Vector2.Distance(positions[0], positions[1]);
            float scaleFactor = currentDistance / initialDistance;

            Vector3 newScale = initialScale * scaleFactor;
            newScale = ClampScale(newScale);
            transform.localScale = newScale;

            if (zoomScrollbar != null)
            {
                updatingFromScroll = true;
                zoomScrollbar.value = GetZoomScrollbarValue(newScale.x);
                updatingFromScroll = false;
            }
        }
        else if (activeTouches.Count == 1)
        {
            if (!canRotate) return;
            
            var delta = eventData.position - lastPointerPosition;
            lastPointerPosition = eventData.position;
            var rotateAmountX = delta.x * CurrentRotationSpeed * Time.deltaTime * 0.5f;
            
            if (isPaint)
            {
                transform.Rotate(Vector3.up, -rotateAmountX, Space.World);

                if (!CanRotateUpDown) return;
                var rotateAmountY = delta.y * CurrentRotationSpeed * Time.deltaTime * 0.5f;
                transform.Rotate(Vector3.right, rotateAmountY, Space.World);
            }
            else
            {
                /*var cameraUp = CameraManager.Instance.mainCamera.transform.up;
                var cameraRight = CameraManager.Instance.mainCamera.transform.right;
                
                var horizontalRotation = delta.x * CurrentRotationSpeed * Time.deltaTime;
                var verticalRotation = delta.y * CurrentRotationSpeed * Time.deltaTime;
                
                transform.Rotate(cameraUp, -horizontalRotation, Space.World);
                transform.Rotate(cameraRight, verticalRotation, Space.World);*/
                
                if (lastArcballVector.HasValue)
                {
                    var currentVector = GetArcballVector(eventData.position);

                    // Tính vector xoay bằng cách lấy tích chéo giữa vector ban đầu và vector hiện tại
                    var rotationAxis = Vector3.Cross(lastArcballVector.Value, currentVector);

                    // Tính góc xoay dựa trên dot product
                    var dot = Vector3.Dot(lastArcballVector.Value, currentVector);
                    dot = Mathf.Clamp(dot, -1.0f, 1.0f);
                    var angle = Mathf.Acos(dot) * Mathf.Rad2Deg; // chuyển sang độ

                    // Nhân với -1 để đảo chiều xoay (giúp kéo lên -> xoay lên, kéo trái -> xoay trái)
                    angle *= -1;

                    if (rotationAxis.sqrMagnitude > 1e-6f) // Kiểm tra để tránh xoay khi vector quá nhỏ
                    {
                        Quaternion rotation = Quaternion.AngleAxis(angle, rotationAxis.normalized);
                        transform.rotation = rotation * transform.rotation;
                    }
            
                    // Cập nhật vector cho lần kéo tiếp theo
                    lastArcballVector = currentVector;
                }
            }
        }
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        if(isPaint) return;
        lastArcballVector = GetArcballVector(eventData.position);
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        if(isPaint) return;
        lastArcballVector = null;
    }
    
    private Vector3? lastArcballVector = null;
    
    private Vector3 GetArcballVector(Vector2 screenPoint)
    {
        float x = (screenPoint.x - Screen.width * 0.5f) / (Screen.width * 0.5f);
        float y = (screenPoint.y - Screen.height * 0.5f) / (Screen.height * 0.5f);
        Vector3 v = new Vector3(x, y, 0);

        // Tính thành phần z để điểm nằm trên mặt cầu ảo
        float mag = v.x * v.x + v.y * v.y;
        if (mag > 1.0f)
        {
            v.Normalize();
        }
        else
        {
            v.z = Mathf.Sqrt(1.0f - mag);
        }
        
        // Chuyển đổi vector từ không gian màn hình sang không gian của camera
        Vector3 vWorld = CameraManager.Instance.mainCamera.transform.right * v.x +
                         CameraManager.Instance.mainCamera.transform.up * v.y +
                         CameraManager.Instance.mainCamera.transform.forward * v.z;
        return vWorld;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (activeTouches.ContainsKey(eventData.pointerId))
            activeTouches.Remove(eventData.pointerId);
        if(activeTouches.Count > 0) return;
        MouseInput.Instance.SetIsDragImage(false);
    }

    public void OnScroll(PointerEventData eventData)
    {
        float scroll = eventData.scrollDelta.y;

        if (scroll != 0f)
        {
            Vector3 scale = transform.localScale;
            scale += Vector3.one * scroll * CurrentZoomSpeed * 10f;
            scale = ClampScale(scale);
            transform.localScale = scale;

            if (zoomScrollbar != null)
            {
                updatingFromScroll = true;
                zoomScrollbar.value = GetZoomScrollbarValue(scale.x);
                updatingFromScroll = false;
            }
        }
    }

    public void SetAverageScale()
    {
        if (resetCoroutine != null) StopCoroutine(resetCoroutine);
        resetCoroutine = StartCoroutine(DoSmoothSetAverageScale());
    }

    IEnumerator DoSmoothSetAverageScale()
    {
        Vector3 startScale = transform.localScale;
        float elapsed = 0f;
        Vector3 targetScale = Vector3.one * ((minScale + maxScale) / 2f);

        while (elapsed < resetDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / resetDuration);
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);

            if (zoomScrollbar != null)
            {
                float newVal = GetZoomScrollbarValue(transform.localScale.x);
                updatingFromScroll = true;
                zoomScrollbar.value = newVal;
                updatingFromScroll = false;
            }

            yield return null;
        }

        transform.localScale = targetScale;

        if (zoomScrollbar != null)
        {
            updatingFromScroll = true;
            zoomScrollbar.value = GetZoomScrollbarValue(targetScale.x);
            updatingFromScroll = false;
        }

        resetCoroutine = null;
    }

    public void SmoothOriginResetTransform()
    {
        if (resetCoroutine != null) StopCoroutine(resetCoroutine);
        resetCoroutine = StartCoroutine(DoSmoothOriginReset());
    }

    public void SmoothAverageResetTransform()
    {
        if (resetCoroutine != null) StopCoroutine(resetCoroutine);
        resetCoroutine = StartCoroutine(DoSmoothAverageReset());
    }

    IEnumerator DoSmoothOriginReset()
    {
        Vector3 startScale = transform.localScale;
        Quaternion startRotation = transform.rotation;
        float elapsed = 0f;

        while (elapsed < resetDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / resetDuration);
            transform.localScale = Vector3.Lerp(startScale, originalScale, t);
            transform.rotation = Quaternion.Slerp(startRotation, originalRotation, t);

            if (zoomScrollbar != null)
            {
                float newVal = GetZoomScrollbarValue(transform.localScale.x);
                updatingFromScroll = true;
                zoomScrollbar.value = newVal;
                updatingFromScroll = false;
            }

            yield return null;
        }

        transform.localScale = originalScale;
        transform.rotation = originalRotation;

        if (zoomScrollbar != null)
        {
            updatingFromScroll = true;
            zoomScrollbar.value = GetZoomScrollbarValue(originalScale.x);
            updatingFromScroll = false;
        }

        resetCoroutine = null;
    }

    IEnumerator DoSmoothAverageReset()
    {
        Vector3 startScale = transform.localScale;
        Quaternion startRotation = transform.rotation;
        float elapsed = 0f;

        while (elapsed < resetDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / resetDuration);
            transform.localScale = Vector3.Lerp(startScale, averageScale, t);
            transform.rotation = Quaternion.Slerp(startRotation, originalRotation, t);

            if (zoomScrollbar != null)
            {
                float newVal = GetZoomScrollbarValue(transform.localScale.x);
                updatingFromScroll = true;
                zoomScrollbar.value = newVal;
                updatingFromScroll = false;
            }

            yield return null;
        }

        transform.localScale = averageScale;
        transform.rotation = originalRotation;

        if (zoomScrollbar != null)
        {
            updatingFromScroll = true;
            zoomScrollbar.value = GetZoomScrollbarValue(averageScale.x);
            updatingFromScroll = false;
        }

        resetCoroutine = null;
    }

    void OnScrollbarChanged(float value)
    {
        if (updatingFromScroll) return;

        float targetScale = Mathf.Lerp(minScale, maxScale, value);
        transform.localScale = new Vector3(targetScale, targetScale, targetScale);
    }

    Vector3 ClampScale(Vector3 scale)
    {
        float clamped = Mathf.Clamp(scale.x, minScale, maxScale);
        return new Vector3(clamped, clamped, clamped);
    }

    public float GetZoomScrollbarValue(float currentScale)
    {
        return Mathf.InverseLerp(minScale, maxScale, currentScale);
    }

    public void SetCollider(bool isActive)
    {
        if(_boxCollider == null) return;
        _boxCollider.enabled = isActive;
    }
    
    public void ZoomByPercentage(float percentage)
    {
        //from  0 to 1
        var targetScale = Mathf.Lerp(minScale, maxScale, percentage);
        transform.localScale = new Vector3(targetScale, targetScale, targetScale);
        
        if (zoomScrollbar != null)
        {
            updatingFromScroll = true;
            zoomScrollbar.value = GetZoomScrollbarValue(targetScale);
            updatingFromScroll = false;
        }
    }
    
    public float GetOriginalScalePercent()
    {
        return (Vector3.one * ((minScale + maxScale) / 2f)).x/maxScale;
    }
}
