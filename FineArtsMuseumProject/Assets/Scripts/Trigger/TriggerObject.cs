using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TriggerObject : MonoBehaviour, IPointerDownHandler
{
    public GameObject blurGameObject;
    bool isBlur = false;

    public InteractiveObjectControl interactiveObject;
    public TextMeshProUGUI ObjectNameText;
    public TextMeshProUGUI ObjectDescriptionText;
    public Button CloseButton;

    public Scrollbar scrollbar;       // Gán Scrollbar từ Inspector// Gán vật thể cần di chuyển
    public float minOffsetZ = 0f;       // Z offset nhỏ nhất
    public float maxOffsetZ = 10f;      // Z offset lớn nhất

    private float initialZ;             // Vị trí Z cao nhất
    
    //Sua lai thanh thang Controller cua Thanh
    public BasicTestPlayerController PlayerController;
    public CharacterStateMachine CharacterStateMachine;
    

    private TriggerZoneStatic triggerZoneStatic;
    
    private void Start()
    {
        CloseButton.onClick.AddListener(()=> TurnOffBlur());
        CloseButton.gameObject.SetActive(false);
        triggerZoneStatic = GetComponent<TriggerZoneStatic>();
    }

    private void Update()
    {
        if (interactiveObject != null && scrollbar != null)
        {
            // Đảo chiều: scrollbar.value = 0 → offset = max, scrollbar.value = 1 → offset = min
            float reversedValue = 1f - scrollbar.value;
            float zOffset = Mathf.Lerp(minOffsetZ, maxOffsetZ, reversedValue);

            Vector3 pos = interactiveObject.transform.localPosition;
            pos.z = initialZ + zOffset;
            interactiveObject.transform.localPosition = pos;
        }
    }

    // private void OnTriggerEnter(Collider other)
    // {
    //     blurGameObject.SetActive(true);
    // }
    //
    // private void OnTriggerExit(Collider other)
    // {
    //     blurGameObject.SetActive(false);
    // }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown");
        if (!isBlur)
        {
            CharacterStateMachine.gameObject.layer = LayerMask.NameToLayer("Default");
            blurGameObject.SetActive(true);
            scrollbar.gameObject.SetActive(true);
            interactiveObject.gameObject.SetActive(true);
            CloseButton.gameObject.SetActive(true);
            interactiveObject.gameObject.transform.SetParent(UnityEngine.Camera.main.transform);
            interactiveObject.gameObject.transform.localPosition = Vector3.forward;
            interactiveObject.gameObject.transform.localScale = new Vector3(.5f, .5f, .5f);
            ObjectNameText.gameObject.SetActive(true);
            ObjectDescriptionText.gameObject.SetActive(true);
            ObjectNameText.text = "Khối Hình Vuông Màu Xanh";
            ObjectDescriptionText.text = "Đây là một khối vuông huyền thoại, chứa vật chất nguy hiểm nhất quả đất, đụng vào rụng tay thấy bà";
            isBlur = true;
            //UnityEngine.Camera.main.transform.LookAt(this.transform);
            //PlayerController.enabled = false;
            CharacterStateMachine.enabled = false;
            
            AudioSubtitleManager.Instance.PlayAudioWithSubtitle(triggerZoneStatic.triggerId);
        }
        else
        {
            TurnOffBlur();
        }
    }

    public void TurnOffBlur()
    {
        CharacterStateMachine.gameObject.layer = LayerMask.NameToLayer("IgnoreBlur");
        ObjectNameText.gameObject.SetActive(false);
        ObjectDescriptionText.gameObject.SetActive(false);
        CloseButton.gameObject.SetActive(false);
        blurGameObject.SetActive(false);
        scrollbar.gameObject.SetActive(false);
        interactiveObject.gameObject.SetActive(false);
        // UnityEngine.Camera.main.transform.localPosition = Vector3.zero;
        // UnityEngine.Camera.main.transform.localRotation = Quaternion.Euler(Vector3.zero);
        isBlur = false;
        //PlayerController.enabled = true;
        CharacterStateMachine.enabled = true;
    }
}
