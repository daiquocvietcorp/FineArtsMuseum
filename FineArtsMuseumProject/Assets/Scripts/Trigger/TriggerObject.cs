using System;
using System.Collections;
using System.Collections.Generic;
using InputController;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

public class TriggerObject : MonoBehaviour, IPointerDownHandler
{
    // Các thành phần UI liên quan
    public GameObject blurGameObject;
    public PaintRotateAndZoom interactiveObject;
    // public TextMeshProUGUI ObjectNameText;
    // public TextMeshProUGUI ObjectDescriptionText;
    public GameObject objectDetail;
    public Button CloseButton;
    public Button ShowGuideButton;
    public Image RotateImage;
    public Sprite RotateSpriteVi;
    public Sprite RotateSpriteEng;
    
    public Image ZoomImage;
    public Image ZoomFingerImage;
    public Image ZoomArrowImage;
    public TextMeshProUGUI ZoomText;
    public Button SoundButton;
    public Sprite SoundOn;
    public Sprite SoundOff;
    public Sprite GuideOn;
    public Sprite GuideOff;

    // Biến kiểm tra trạng thái
    private bool isBlur = false;
    private bool isGuidePlaying = false;
    private bool isSoundOn = true;
    private Coroutine guideCoroutine;

    public float instructionDuration = 10f;

    // Các thành phần liên quan đến trạng thái nhân vật
    public CharacterStateMachine CharacterStateMachine;
    private TriggerZoneStatic triggerZoneStatic;

    private void Start()
    {
        // Thiết lập các nút bấm và các sự kiện
        CloseButton.onClick.AddListener(() => TurnOffBlur());
        ShowGuideButton.onClick.AddListener(ToggleGuide);
        SoundButton.onClick.AddListener(ToggleSound);

        // Khởi tạo ban đầu
        CloseButton.gameObject.SetActive(false);
        triggerZoneStatic = GetComponent<TriggerZoneStatic>();
        SetInitialUIState();

        // Thiết lập hình ảnh và âm thanh
        SoundButton.image.sprite = SoundOn;
    }

    // Hàm điều chỉnh âm thanh bật/tắt
    private void ToggleSound()
    {
        if (isSoundOn)
        {
            // Tắt âm thanh
            AudioSubtitleManager.Instance.StopAudioAndClearSubtitle();
            SoundButton.image.sprite = SoundOff;
            isSoundOn = false;
        }
        else
        {
            // Bật âm thanh
            AudioSubtitleManager.Instance.PlayAudioWithSubtitle(triggerZoneStatic.triggerId);
            SoundButton.image.sprite = SoundOn;
            isSoundOn = true;
        }
    }

    // Hàm điều chỉnh hướng dẫn
    private void ToggleGuide()
    {
        if (isGuidePlaying)
        {
            // Dừng hướng dẫn
            StopGuide();
        }
        else
        {
            // Bắt đầu hướng dẫn
            StartGuide();
        }
    }

    // Bắt đầu chuỗi hướng dẫn
    private void StartGuide()
    {
        guideCoroutine = StartCoroutine(PlayGuideSequence());
        isGuidePlaying = true;
        ShowGuideButton.image.sprite = GuideOn;
    }

    // Dừng chuỗi hướng dẫn
    private void StopGuide()
    {
        if (guideCoroutine != null)
            StopCoroutine(guideCoroutine);

        guideCoroutine = null;
        isGuidePlaying = false;
        ShowGuideButton.image.sprite = GuideOff;

        // Ẩn các thành phần hướng dẫn
        HideGuideUIElements();
    }

    // Hàm chơi chuỗi hướng dẫn
    private IEnumerator PlayGuideSequence()
    {
        string currentLanguage = PlayerPrefs.GetString("Language", "vi");
        if (currentLanguage == "vi")
        {
            RotateImage.sprite = RotateSpriteVi;
        }
        else if(currentLanguage == "en")
        {
            RotateImage.sprite = RotateSpriteEng;
        }
        
        yield return FadeImageIn(RotateImage);
        yield return new WaitForSeconds(instructionDuration);
        yield return FadeImageOut(RotateImage);

        yield return FadeInZoomGroup();
        yield return new WaitForSeconds(instructionDuration);
        yield return FadeOutZoomGroup();

        // Kết thúc hướng dẫn
        StopGuide();
    }

    // Hàm fade UI thành phần
    private IEnumerator FadeUI(Graphic uiElement, float from, float to, float duration)
    {
        if (uiElement == null) yield break;

        Color color = uiElement.color;
        float time = 0;

        uiElement.gameObject.SetActive(true);

        while (time < duration)
        {
            time += Time.deltaTime;
            color.a = Mathf.Lerp(from, to, time / duration);
            uiElement.color = color;
            yield return null;
        }

        color.a = to;
        uiElement.color = color;

        if (to == 0f)
            uiElement.gameObject.SetActive(false);
    }

    // Hàm fade nhóm zoom
    private IEnumerator FadeInZoomGroup()
    {
        yield return StartCoroutine(FadeUI(ZoomImage, 0f, 1f, 1f));
        StartCoroutine(FadeUI(ZoomFingerImage, 0f, 1f, 1f));
        StartCoroutine(FadeUI(ZoomArrowImage, 0f, 1f, 1f));
        StartCoroutine(FadeUI(ZoomText, 0f, 1f, 1f));
    }

    private IEnumerator FadeOutZoomGroup()
    {
        StartCoroutine(FadeUI(ZoomFingerImage, 1f, 0f, 1f));
        StartCoroutine(FadeUI(ZoomArrowImage, 1f, 0f, 1f));
        StartCoroutine(FadeUI(ZoomText, 1f, 0f, 1f));
        yield return StartCoroutine(FadeUI(ZoomImage, 1f, 0f, 1f));
    }

    // Fade in cho image
    private IEnumerator FadeImageIn(Image img)
    {
        img.gameObject.SetActive(true);
        Color color = img.color;
        color.a = 0f;
        img.color = color;

        float duration = 1f;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, time / duration);
            img.color = color;
            yield return null;
        }

        color.a = 1f;
        img.color = color;
    }

    // Fade out cho image
    private IEnumerator FadeImageOut(Image img)
    {
        Color color = img.color;
        float duration = 1f;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, time / duration);
            img.color = color;
            yield return null;
        }

        color.a = 0f;
        img.color = color;
        img.gameObject.SetActive(false);
    }

    // Xử lý sự kiện khi người dùng nhấn vào đối tượng
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown");
        if (!isBlur)
        {
            ActivateInteractiveMode();
        }
        else
        {
            TurnOffBlur();
        }
    }

    // Bật chế độ làm mờ và hiển thị các UI liên quan
    private void ActivateInteractiveMode()
    {
        CharacterStateMachine.gameObject.layer = LayerMask.NameToLayer("Default");
        blurGameObject.SetActive(true);
        interactiveObject.gameObject.SetActive(true);
        CloseButton.gameObject.SetActive(true);
        objectDetail.SetActive(true);
        interactiveObject.gameObject.transform.SetParent(UnityEngine.Camera.main.transform);
        interactiveObject.gameObject.transform.localPosition = Vector3.forward;
        interactiveObject.gameObject.transform.localScale = new Vector3(.5f, .5f, .5f);
        //ObjectNameText.gameObject.SetActive(true);
        //ObjectDescriptionText.gameObject.SetActive(true);
        isBlur = true;

        // Vô hiệu hóa các chức năng liên quan đến nhân vật và điều khiển
        CharacterManager.Instance.DisableCharacter();
        InputManager.Instance.DisableInput();
        AudioSubtitleManager.Instance.PlayAudioWithSubtitle(triggerZoneStatic.triggerId);
    }

    // Tắt chế độ làm mờ và ẩn các UI liên quan
    public void TurnOffBlur()
    {
        CharacterStateMachine.gameObject.layer = LayerMask.NameToLayer("Default");
        //ObjectNameText.gameObject.SetActive(false);
        objectDetail.SetActive(false);
        //ObjectDescriptionText.gameObject.SetActive(false);
        CloseButton.gameObject.SetActive(false);
        blurGameObject.SetActive(false);
        interactiveObject.gameObject.SetActive(false);
        isBlur = false;

        // Dừng hướng dẫn và ẩn các thành phần liên quan
        StopGuide();

        // Reset âm thanh và nhân vật
        SoundButton.image.sprite = SoundOn;
        isSoundOn = true;

        // Bật lại các chức năng liên quan đến nhân vật và điều khiển
        CharacterManager.Instance.EnableCharacter();
        InputManager.Instance.EnableInput();
        AudioSubtitleManager.Instance.StopAudioAndClearSubtitle();
    }

    // Thiết lập trạng thái ban đầu cho UI
    private void SetInitialUIState()
    {
        RotateImage.gameObject.SetActive(false);
        ZoomImage.gameObject.SetActive(false);
        ZoomFingerImage.gameObject.SetActive(false);
        ZoomArrowImage.gameObject.SetActive(false);
        ZoomText.gameObject.SetActive(false);
    }

    // Ẩn tất cả các phần tử hướng dẫn
    private void HideGuideUIElements()
    {
        RotateImage.gameObject.SetActive(false);
        ZoomImage.gameObject.SetActive(false);
    }
}
