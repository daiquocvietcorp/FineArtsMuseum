using System;
using System.Collections;
using System.Collections.Generic;
using Camera;
using InputController;
using Trigger;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PaintRotateAndZoom : MonoBehaviour, IPointerDownHandler, IDragHandler, IScrollHandler, IPointerUpHandler, IBeginDragHandler, IEndDragHandler
{
    [field: SerializeField] private bool isPaint = true;
    [field: SerializeField] private bool isObject = false;
    
    public float rotationSpeed = 100f;
    public float zoomSpeed = 0.01f;

    public float rotationSpeedMobile = 80f;
    public float zoomSpeedMobile = 0.015f;

    public float minScale = 0.2f;
    public float maxScale = 3f;
    public float resetDuration = 0.5f;

    public Scrollbar zoomScrollbar;
    public bool CanRotateUpDown = false;
    [SerializeField] private bool isRotateOneDirect;

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
    
    private float _dragTime;
    private bool _isDragObject;    
    
    public bool canRotate = true;

    public bool IsUseOnPointerDown = false;
    public MagnifierHover mobileMagnifierHover;
    public MagnifierHover otherMagnifierHover;
    
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
        
        //new
        
        if(isObject && PlatformManager.Instance.IsTomko)
        {
            MouseInput.Instance.SetIsDragImage(true);
            SmoothAverageResetTransform();
            _isDragObject = true;
        }
    }

    private void OnDisable()
    {
        if (zoomScrollbar != null)
            zoomScrollbar.gameObject.SetActive(false);
        
        if(isObject && PlatformManager.Instance.IsTomko)
            MouseInput.Instance.SetIsDragImage(false);
    }

    private float _holdTimer;
    private enum Direction
    {
        Horizontal,
        Vertical,
        None
    }
    private Direction _holdDirection = Direction.None;
    private void Update()
    {
        if(Time.time - _dragTime > 10f && !_isDragObject)
        {
            SmoothAverageResetTransform();
            _isDragObject = true;
            
            if(PlatformManager.Instance.IsTomko)
                AntiqueManager.Instance.ResetSlider();
        }
        
        if(!isObject || !PlatformManager.Instance.IsTomko) return;

        if (!canRotate) return;
        
        if (Input.touchCount <= 0) return;
        if (!isObject) return;
        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
        {
            lastArcballVector = GetArcballVector(touch.position);
            _currentDragMode = DragMode.None;
        }
        
        if (touch.phase == TouchPhase.Moved)
        {
            if(AntiqueManager.Instance.IsChangeArcSlider()) return;
            if (!lastArcballVector.HasValue) return;
            var currentVector = GetArcballVector(touch.position);

            // Tính vector xoay bằng cách lấy tích chéo giữa vector ban đầu và vector hiện tại
            var rotationAxis = Vector3.Cross(lastArcballVector.Value, currentVector);

            switch (_currentDragMode)
            {
                case DragMode.None:
                {
                    if (rotationAxis != Vector3.zero)
                    {
                        if(Mathf.Approximately(Mathf.Abs(rotationAxis.x), Mathf.Abs(rotationAxis.y))) return;
                        _currentDragMode = Mathf.Abs(rotationAxis.x) > Mathf.Abs(rotationAxis.y)
                            ? DragMode.Horizontal
                            : DragMode.Vertical;
                        switch (_currentDragMode)
                        {
                            case DragMode.Horizontal:
                                rotationAxis.y = 0;
                                break;
                            case DragMode.Vertical:
                                rotationAxis.y *= 0.66f;
                                rotationAxis.x = 0;
                                break;
                        }
                    }
                    break;
                }
                case DragMode.Horizontal:
                    rotationAxis.y = 0;
                    break;
                case DragMode.Vertical:
                    rotationAxis.x = 0;
                    break;
            }
    
            rotationAxis.z = 0;
            // Tính góc xoay dựa trên dot product
            var dot = Vector3.Dot(lastArcballVector.Value, currentVector);
            dot = Mathf.Clamp(dot, -1.0f, 1.0f);
            var angle = Mathf.Acos(dot) * Mathf.Rad2Deg; // chuyển sang độ
            // Nhân với -1 để đảo chiều xoay
            angle *= -1;
            // Điều chỉnh độ nhạy xoay bằng rotationSpeed (hoặc phiên bản mobile) và Time.deltaTime
            angle *= Time.deltaTime * CurrentRotationSpeed;
            /*if (rotationAxis.sqrMagnitude > 1e-6f) // Kiểm tra để tránh xoay khi vector quá nhỏ
            {
                Quaternion rotation = Quaternion.AngleAxis(angle, rotationAxis.normalized);
                transform.rotation = rotation * transform.rotation;
            }*/
            
            Quaternion rotation = Quaternion.AngleAxis(angle, rotationAxis.normalized);
            transform.rotation = rotation * transform.rotation;
            // Cập nhật vector cho lần kéo tiếp theo
            lastArcballVector = currentVector;
            _isDragObject = true;
            return;
        }

        if (touch.phase == TouchPhase.Ended)
        {
            _currentDragMode = DragMode.None;
            _dragTime = Time.time;
            _isDragObject = false;
            lastArcballVector = null;
        }
    }

    void Start()
    {
        originalRotation = transform.rotation;
        averageScale = Vector3.one * (minScale);

        if (zoomScrollbar != null)
        {
            zoomScrollbar.onValueChanged.AddListener(OnScrollbarChanged);
            zoomScrollbar.value = GetZoomScrollbarValue(transform.localScale.x);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (IsUseOnPointerDown)
        {
            if (PlatformManager.Instance.IsCloud && _canZoom)
            {
                if (mobileMagnifierHover != null)
                {
                    mobileMagnifierHover.OnPointerDown(eventData);
                }
            }
            
            if (PlatformManager.Instance.IsWebGL && _canZoom)
            {
                if (otherMagnifierHover != null)
                {
                    otherMagnifierHover.OnPointerDown(eventData);
                }
            }
        }
        
        if(isObject && PlatformManager.Instance.IsTomko) return;
        
        
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
        if (IsUseOnPointerDown)
        {
            if (PlatformManager.Instance.IsCloud && _canZoom)
            {
                if (mobileMagnifierHover != null)
                {
                    mobileMagnifierHover.OnDrag(eventData);
                }
            }
            
            if (PlatformManager.Instance.IsWebGL && _canZoom)
            {
                if (otherMagnifierHover != null)
                {
                    otherMagnifierHover.OnDrag(eventData);
                }
            }
        }
        
        if(isObject && PlatformManager.Instance.IsTomko) return;
        
        
        if (!activeTouches.ContainsKey(eventData.pointerId))
            return;

        activeTouches[eventData.pointerId] = eventData.position;

        if (activeTouches.Count == 2)
        {
            if(PlatformManager.Instance.IsTomko && isPaint) return;
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

            
            var rotateAmountX = delta.x * CurrentRotationSpeed * Time.deltaTime;
            var rotateAmountY = delta.y * CurrentRotationSpeed * Time.deltaTime;

            // if (isObject)
            // {
            //     if (!lastArcballVector.HasValue) return;
            //     var currentVector = GetArcballVector(eventData.position);
            //
            //     // Tính vector xoay bằng cách lấy tích chéo giữa vector ban đầu và vector hiện tại
            //     var rotationAxis = Vector3.Cross(lastArcballVector.Value, currentVector);
            //
            //     switch (_currentDragMode)
            //     {
            //         case DragMode.None:
            //         {
            //             if (rotationAxis != Vector3.zero)
            //             {
            //                 _currentDragMode = Mathf.Abs(rotationAxis.x) > Mathf.Abs(rotationAxis.y)
            //                                    ? DragMode.Horizontal
            //                                    : DragMode.Vertical;
            //                 switch (_currentDragMode)
            //                 {
            //                     case DragMode.Horizontal:
            //                         rotationAxis.y = 0;
            //                         break;
            //                     case DragMode.Vertical:
            //                         rotationAxis.x = 0;
            //                         break;
            //                 }
            //             }
            //             break;
            //         }
            //         case DragMode.Horizontal:
            //             rotationAxis.y = 0;
            //             break;
            //         case DragMode.Vertical:
            //             rotationAxis.x = 0;
            //             break;
            //     }
            //         
            //     rotationAxis.z = 0;
            //
            //     // Tính góc xoay dựa trên dot product
            //     var dot = Vector3.Dot(lastArcballVector.Value, currentVector);
            //     dot = Mathf.Clamp(dot, -1.0f, 1.0f);
            //     var angle = Mathf.Acos(dot) * Mathf.Rad2Deg; // chuyển sang độ
            //
            //     // Nhân với -1 để đảo chiều xoay
            //     angle *= -1;
            //
            //     // Điều chỉnh độ nhạy xoay bằng rotationSpeed (hoặc phiên bản mobile) và Time.deltaTime
            //     angle *= Time.deltaTime * CurrentRotationSpeed;
            //
            //     if (rotationAxis.sqrMagnitude > 1e-6f) // Kiểm tra để tránh xoay khi vector quá nhỏ
            //     {
            //         Quaternion rotation = Quaternion.AngleAxis(angle, rotationAxis.normalized);
            //         transform.rotation = rotation * transform.rotation;
            //     }
            //
            //     // Cập nhật vector cho lần kéo tiếp theo
            //     lastArcballVector = currentVector;
            //     return;
            // }
            if (isObject)
            {
                if (!lastArcballVector.HasValue) return;
                var currentVector = GetArcballVector(eventData.position);

                // Tính vector xoay bằng cách lấy tích chéo giữa vector ban đầu và vector hiện tại
                var rotationAxis = Vector3.Cross(lastArcballVector.Value, currentVector);

                switch (_currentDragMode)
                {
                    case DragMode.None:
                    {
                        if (rotationAxis != Vector3.zero)
                        {
                            _currentDragMode = Mathf.Abs(rotationAxis.x) > Mathf.Abs(rotationAxis.y)
                                ? DragMode.Horizontal
                                : DragMode.Vertical;
                            switch (_currentDragMode)
                            {
                                case DragMode.Horizontal:
                                    rotationAxis.y = 0;
                                    break;
                                case DragMode.Vertical:
                                    rotationAxis.x = 0;
                                    break;
                            }
                        }
                        break;
                    }
                    case DragMode.Horizontal:
                        rotationAxis.y = 0;
                        break;
                    case DragMode.Vertical:
                        rotationAxis.x = 0;
                        break;
                }
        
                rotationAxis.z = 0;

                // Tính góc xoay dựa trên dot product
                var dot = Vector3.Dot(lastArcballVector.Value, currentVector);
                dot = Mathf.Clamp(dot, -1.0f, 1.0f);
                var angle = Mathf.Acos(dot) * Mathf.Rad2Deg; // chuyển sang độ

                // Nhân với -1 để đảo chiều xoay
                angle *= -1;

                // Điều chỉnh độ nhạy xoay bằng rotationSpeed (hoặc phiên bản mobile) và Time.deltaTime
                angle *= Time.deltaTime * CurrentRotationSpeed;

                /*if (rotationAxis.sqrMagnitude > 1e-6f) // Kiểm tra để tránh xoay khi vector quá nhỏ
                {
                    Quaternion rotation = Quaternion.AngleAxis(angle, rotationAxis.normalized);
                    transform.rotation = rotation * transform.rotation;
                }*/
                
                Quaternion rotation = Quaternion.AngleAxis(angle, rotationAxis.normalized);
                transform.rotation = rotation * transform.rotation;

                // Cập nhật vector cho lần kéo tiếp theo
                lastArcballVector = currentVector;
                _isDragObject = true;
                return;
            }
            
            if (isRotateOneDirect)
            {
                transform.Rotate(Vector3.up, -rotateAmountX, Space.Self);   
            }
            else
            { 
                // Lấy giá trị Euler angles hiện tại
                Vector3 currentEuler = transform.eulerAngles;

                // Tính góc xoay theo kéo ngang (dành cho trục Y)
                float rotateHorizontal = delta.x * CurrentRotationSpeed * Time.deltaTime * 0.5f;
                currentEuler.y += rotateHorizontal;

                // Nếu cho phép xoay lên/xuống thì xử lý kéo dọc (thay đổi trục Z)
                if (CanRotateUpDown)
                {
                    float rotateVertical = delta.y * CurrentRotationSpeed * Time.deltaTime * 0.5f;
                    currentEuler.z += rotateVertical;
                }

                lastPointerPosition = eventData.position;
                // Gán lại giá trị Euler angles mới vào đối tượng
                transform.eulerAngles = currentEuler;
            }

        }
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        if(isObject && PlatformManager.Instance.IsTomko) return;
        
        if(isPaint) return;
        lastArcballVector = GetArcballVector(eventData.position);
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        if(isObject && PlatformManager.Instance.IsTomko) return;
        
        if(isPaint) return;
        lastArcballVector = null;
        _currentDragMode = DragMode.None;
    }
    
    private Vector3? lastArcballVector = null;
    private enum DragMode { None, Horizontal, Vertical }
    private DragMode _currentDragMode = DragMode.None;
    
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
        if (IsUseOnPointerDown)
        {
            if (PlatformManager.Instance.IsCloud)
            {
                if (mobileMagnifierHover != null)
                {
                    mobileMagnifierHover.OnPointerUp(eventData);
                }
            }
        }
        
        if(isObject && PlatformManager.Instance.IsTomko) return;
        
        if (activeTouches.ContainsKey(eventData.pointerId))
            activeTouches.Remove(eventData.pointerId);
        if(activeTouches.Count > 0) return;
        MouseInput.Instance.SetIsDragImage(false);
        _currentDragMode = DragMode.None;
        _dragTime = Time.time;
        _isDragObject = false;
    }

    public void OnScroll(PointerEventData eventData)
    {
        if(isObject && PlatformManager.Instance.IsTomko) return;
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
        Vector3 targetScale = Vector3.one * (minScale);

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
        //return (Vector3.one * ((minScale + maxScale) / 2f)).x/maxScale;
        return 0;
    }
    
    public float GetAvarageScalePercent()
    {
        return (Vector3.one * ((minScale + maxScale) / 2f)).x/maxScale;
        //return 0;
    }

    private bool _canZoom = false;
    
    public void SetEnableZoom(bool isActive)
    {
        _canZoom = isActive;
    }
}
