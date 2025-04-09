using System;
using System.Collections;
using System.Collections.Generic;
using InputController;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PaintRotateAndZoom : MonoBehaviour, IPointerDownHandler, IDragHandler, IScrollHandler, IPointerUpHandler
{
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
        if(MouseInput.Instance.IsHold) return;
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
        if(MouseInput.Instance.IsHold) return;
        if (!activeTouches.ContainsKey(eventData.pointerId))
            return;

        activeTouches[eventData.pointerId] = eventData.position;

        if (activeTouches.Count == 2)
        {
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

            Vector2 delta = eventData.position - lastPointerPosition;
            lastPointerPosition = eventData.position;

            float rotateAmountX = delta.x * CurrentRotationSpeed * Time.deltaTime * 0.5f;
            transform.Rotate(Vector3.up, -rotateAmountX, Space.World);

            if (CanRotateUpDown)
            {
                float rotateAmountY = delta.y * CurrentRotationSpeed * Time.deltaTime * 0.5f;
                transform.Rotate(Vector3.right, rotateAmountY, Space.World);
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if(MouseInput.Instance.IsHold) return;
        if (activeTouches.ContainsKey(eventData.pointerId))
            activeTouches.Remove(eventData.pointerId);
        if(activeTouches.Count > 0) return;
        MouseInput.Instance.SetIsDragImage(false);
    }

    public void OnScroll(PointerEventData eventData)
    {
        if(MouseInput.Instance.IsHold) return;
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
}
