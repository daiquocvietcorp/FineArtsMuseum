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

    public BasicTestPlayerController PlayerController;
    public CharacterStateMachine CharacterStateMachine;
    //Sua lai thanh thang Controller cua Thanh

    private TriggerZoneStatic triggerZoneStatic;
    
    private void Start()
    {
        CloseButton.onClick.AddListener(()=> TurnOffBlur());
        CloseButton.gameObject.SetActive(false);
        triggerZoneStatic = GetComponent<TriggerZoneStatic>();
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
            blurGameObject.SetActive(true);
            interactiveObject.gameObject.SetActive(true);
            CloseButton.gameObject.SetActive(true);
            interactiveObject.gameObject.transform.SetParent(UnityEngine.Camera.main.transform);
            interactiveObject.gameObject.transform.localPosition = new Vector3(0, 0, 1);
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
        ObjectNameText.gameObject.SetActive(false);
        ObjectDescriptionText.gameObject.SetActive(false);
        CloseButton.gameObject.SetActive(false);
        blurGameObject.SetActive(false);
        interactiveObject.gameObject.SetActive(false);
        // UnityEngine.Camera.main.transform.localPosition = Vector3.zero;
        // UnityEngine.Camera.main.transform.localRotation = Quaternion.Euler(Vector3.zero);
        isBlur = false;
        //PlayerController.enabled = true;
        CharacterStateMachine.enabled = true;
    }
}
