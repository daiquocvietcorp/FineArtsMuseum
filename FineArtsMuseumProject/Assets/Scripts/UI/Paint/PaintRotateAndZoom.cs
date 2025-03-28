using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PaintRotateAndZoom : MonoBehaviour
{
    public float rotationSpeed = 100f;
    public float zoomSpeed = 0.01f;
    public float minScale = 0.2f;
    public float maxScale = 3f;
    public float resetDuration = 0.5f;

    public Scrollbar zoomScrollbar; // ← Gán Scrollbar vào đây (giá trị 0~1)

    private Vector3 originalScale;
    private Quaternion originalRotation;
    private Coroutine resetCoroutine;

    private bool updatingFromScroll = false;

    private void OnEnable()
    {
        if (zoomScrollbar != null)
        {
            zoomScrollbar.gameObject.SetActive(true);
        }
    }

    private void OnDisable()
    {
        if (zoomScrollbar != null)
        {
            zoomScrollbar.gameObject.SetActive(false);
        }
    }

    void Start()
    {
        originalScale = transform.localScale;
        originalRotation = transform.rotation;

        if (zoomScrollbar != null)
        {
            // Gán callback khi kéo thanh
            zoomScrollbar.onValueChanged.AddListener(OnScrollbarChanged);
            // Khởi tạo giá trị theo scale hiện tại
            zoomScrollbar.value = GetZoomScrollbarValue(transform.localScale.x);
        }
    }

    void Update()
    {
        PaintRotateAndZoomFunction();

        // Nhấn R để reset
        if (Input.GetKeyDown(KeyCode.R))
        {
            SmoothResetTransform();
        }
    }

    void PaintRotateAndZoomFunction()
    {
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
        // PC: xoay bằng chuột phải
        if (Input.GetMouseButton(1))
        {
            float rotateAmount = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            transform.Rotate(Vector3.up, -rotateAmount, Space.World);
        }

        // PC: Zoom bằng cuộn chuột
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            Vector3 scale = transform.localScale;
            scale += Vector3.one * scroll * zoomSpeed * 100f;
            scale = ClampScale(scale);
            transform.localScale = scale;

            // Cập nhật thanh zoom
            if (zoomScrollbar != null)
            {
                updatingFromScroll = true;
                zoomScrollbar.value = GetZoomScrollbarValue(scale.x);
                updatingFromScroll = false;
            }
        }
#else
        // Mobile: xoay bằng 1 ngón
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                float rotateAmount = touch.deltaPosition.x * rotationSpeed * Time.deltaTime * 0.5f;
                transform.Rotate(Vector3.up, -rotateAmount, Space.World);
            }
        }

        // Mobile: zoom bằng 2 ngón
        if (Input.touchCount == 2)
        {
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            Vector2 prevPos1 = touch1.position - touch1.deltaPosition;
            Vector2 prevPos2 = touch2.position - touch2.deltaPosition;

            float prevMag = (prevPos1 - prevPos2).magnitude;
            float currMag = (touch1.position - touch2.position).magnitude;

            float delta = currMag - prevMag;

            Vector3 scale = transform.localScale;
            scale += Vector3.one * delta * zoomSpeed;
            scale = ClampScale(scale);
            transform.localScale = scale;

            // Update scrollbar
            if (zoomScrollbar != null)
            {
                updatingFromScroll = true;
                zoomScrollbar.value = GetZoomScrollbarValue(scale.x);
                updatingFromScroll = false;
            }
        }
#endif
    }

    void OnScrollbarChanged(float value)
    {
        if (updatingFromScroll) return; // Tránh vòng lặp

        float targetScale = Mathf.Lerp(minScale, maxScale, value);
        transform.localScale = new Vector3(targetScale, targetScale, targetScale);
    }

    Vector3 ClampScale(Vector3 scale)
    {
        float clamped = Mathf.Clamp(scale.x, minScale, maxScale);
        return new Vector3(clamped, clamped, clamped);
    }

    float GetZoomScrollbarValue(float currentScale)
    {
        return Mathf.InverseLerp(minScale, maxScale, currentScale);
    }

    public void SmoothResetTransform()
    {
        if (resetCoroutine != null) StopCoroutine(resetCoroutine);
        resetCoroutine = StartCoroutine(DoSmoothReset());
    }

    IEnumerator DoSmoothReset()
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
}
