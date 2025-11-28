using System;
using System.Collections;
using Camera;
using Trigger;
using UnityEngine;
using UnityEngine.EventSystems;

public class TriggerObject : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public string antiqueID;

    private Vector2 _pointerDownPos;
    private bool _isDragging = false;

    // Ngưỡng coi là drag (tùy chỉnh)
    private const float DRAG_THRESHOLD = 15f;

    public float interactableDistance = 1.7f;

    public void OnPointerDown(PointerEventData eventData)
    {
        _pointerDownPos = eventData.position;
        _isDragging = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (Vector2.Distance(eventData.position, _pointerDownPos) > DRAG_THRESHOLD)
        {
            _isDragging = true; 
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Nếu kéo → không xử lý click
        if (_isDragging)
        {
            // Debug.Log("Bỏ click, vì người dùng đang xoay camera.");
            return;
        }

        // Nếu không kéo → click hợp lệ → chạy logic cũ
        if (Vector3.Distance(UnityEngine.Camera.main.transform.position, transform.position) < interactableDistance)
        {
            if (!string.IsNullOrEmpty(antiqueID))
            {
                AIVideoManager.Instance.PreSetVideo(antiqueID);

                if (PlatformManager.Instance.IsMobile || PlatformManager.Instance.IsTomko || PlatformManager.Instance.IsCloud)
                {
                    StartCoroutine(PlayObjectAnimation());
                }
                else
                {
                    AntiqueManager.Instance.EnableAntiqueDetail(antiqueID);
                }

                CameraManager.Instance.cameraFollowPlayer.SetCanControl(false);
            }
        }
    }
    
    

    private IEnumerator PlayObjectAnimation()
    {
        //_hoverEffect.FakePointerEnter();
        //yield return new WaitForSeconds(1f);
        yield return new WaitForSeconds(0f);
        //_hoverEffect.FakePointerExit();
        AntiqueManager.Instance.EnableAntiqueDetail(antiqueID);
    }

    public void FakePointerDown()
    {
        if (antiqueID != null || antiqueID != "")
        {
            AntiqueManager.Instance.EnableAntiqueDetail(antiqueID);
        }
    }

    
}