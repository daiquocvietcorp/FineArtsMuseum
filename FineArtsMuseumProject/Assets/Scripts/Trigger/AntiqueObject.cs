

using System;
using System.Net;
using DG.Tweening;
using Slider;
using UI;

namespace Trigger
{ 
    using System.Collections;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    using InputController;
    using Player;

public class AntiqueObject : MonoBehaviour
{
    [field: Header("Antique ID")]
    [field: SerializeField] private string AntiqueID { get; set; }

    [Header("UI Elements")]
    public GameObject blurGameObject;
    public PaintRotateAndZoom interactiveObject;
    public Button CloseButton;
    public Button ShowGuideButton;
    public Button RefreshButton;
    public Image RotateImage;
    public Sprite RotateSpriteVi;
    public Sprite RotateSpriteEng;
    

    public Image ZoomImage;
    public Image ZoomFingerImage;
    public Image ZoomArrowImage;
    
    public Sprite ZoomSpriteTomko;
    public Sprite ZoomSprite;
    public TextMeshProUGUI ZoomText;
    public Button SoundButton;
    public Sprite SoundOn;
    public Sprite SoundOff;
    public Sprite GuideOn;
    public Sprite GuideOff;

    [Header("Settings")]
    public float instructionDuration = 10f;
    public Vector3 interactObjectLocalPosition = Vector3.forward;
    public Vector3 interactObjectLocalRotation = Vector3.zero;
    
    public Vector2 animationImageAnchoredPos;

    [Header("Dependencies")]
    public CharacterStateMachine CharacterStateMachine;
    public TriggerZoneStatic triggerZoneStatic;

    public bool isAntique = true;
    private bool isBlur = false;
    private bool isGuidePlaying = false;
    private bool isSoundOn = true;
    
    public bool openInfomationUI = true;
    
    public bool isSetRotation = false;
    
    private Coroutine guideCoroutine;

    public RectTransform rect;
    
    public bool hasInformationUI = false;

    public bool hasGuide = true;

    [SerializeField] private Transform InteractiveObjectLocation;
    
    private void Start()
    {
        if (PlatformManager.Instance.IsTomko)
        {
            ZoomArrowImage.sprite = ZoomSpriteTomko;
        }
        else
        {
            ZoomArrowImage.sprite = ZoomSprite;
        }
        
        CloseButton.onClick.AddListener(TurnOffBlur);
        ShowGuideButton.onClick.AddListener(ToggleGuide);
        SoundButton.onClick.AddListener(ToggleSound);
        RefreshButton.onClick.AddListener(Refresh);

        //triggerZoneStatic = GetComponent<TriggerZoneStatic>();
        SetInitialUIState();
        SoundButton.image.sprite = SoundOn;
        //CloseButton.gameObject.SetActive(false);
    }

    public void Refresh()
    {
        interactiveObject.SmoothAverageResetTransform();
        AntiqueManager.Instance.ResetSlider();
    }
    
    private void OnEnable()
    {
        ActivateInteractiveMode();

        if (hasGuide)
        {
            if (!SceneLog.IsShowedGuideObject)
            {
                
                StartCoroutine(DelayedStartGuide());
                SceneLog.IsShowedGuideObject = true;
            }
        }
        else
        {
            ShowGuideButton.gameObject.SetActive(false);
        }
        if (!PlatformManager.Instance.IsVR)
        {
            rect.transform.position += new Vector3(0f, 0f, 0f);
            rect.ForceUpdateRectTransforms();
        }
        
    }
    
    private IEnumerator DelayedStartGuide()
    {
        yield return null; // đợi 1 frame
        StartGuide();
    }

    public string GetAntiqueID()
    {
        return AntiqueID;
    }

    public void ActivateInteractiveMode()
    {
        if (!PlatformManager.Instance.IsVR)
        {
            CharacterStateMachine.gameObject.layer = LayerMask.NameToLayer("Default");
            blurGameObject.SetActive(true);


        }

        if (PlatformManager.Instance.IsStandalone || PlatformManager.Instance.IsWebGL)
        {
            var closeRect = CloseButton.GetComponent<RectTransform>();
            closeRect.anchoredPosition = new Vector2(-159.8477f, -80.69809f);
            closeRect.sizeDelta = new Vector2(36.2659f, 35.8804f);
            
            var refreshRect = RefreshButton.GetComponent<RectTransform>();
            refreshRect.anchoredPosition = new Vector2(-208.7518f, -80.69809f);
            refreshRect.sizeDelta = new Vector2(36.2659f, 35.8804f);
            
            var guideRect = ShowGuideButton.GetComponent<RectTransform>();
            guideRect.anchoredPosition = new Vector2(-257.402f, -80.69809f);
            guideRect.sizeDelta = new Vector2(36.2659f, 35.8804f);
        }
        else if (PlatformManager.Instance.IsVR)
        {
            var closeRect = CloseButton.GetComponent<RectTransform>();
            closeRect.anchoredPosition = new Vector2(-370f, -288f);
            closeRect.sizeDelta = new Vector2(35f, 35f);
            closeRect.localScale = new Vector3(2f, 2f, 2f);
            
            var refreshRect = RefreshButton.GetComponent<RectTransform>();
            refreshRect.anchoredPosition = new Vector2(-457.3499f, -288f);
            refreshRect.sizeDelta = new Vector2(35f, 35f);
            refreshRect.localScale = new Vector3(2f, 2f, 2f);
            
            var guideRect = ShowGuideButton.GetComponent<RectTransform>();
            guideRect.anchoredPosition = new Vector2(-547f, -288f);
            guideRect.sizeDelta = new Vector2(35f, 35f);
            guideRect.localScale = new Vector3(2f, 2f, 2f);
            
        }
        else if(PlatformManager.Instance.IsMobile || PlatformManager.Instance.IsCloud)
        {
            
            var closeRect = CloseButton.GetComponent<RectTransform>();
            closeRect.anchoredPosition = new Vector2(-98.1f, -80.69809f);
            closeRect.sizeDelta = new Vector2(70f, 70f);
            
            var refreshRect = RefreshButton.GetComponent<RectTransform>();
            refreshRect.anchoredPosition = new Vector2(-183.3f, -80.69809f);
            refreshRect.sizeDelta = new Vector2(70f, 70f);
            
            var guideRect = ShowGuideButton.GetComponent<RectTransform>();
            guideRect.anchoredPosition = new Vector2(-268.3f, -80.69809f);
            guideRect.sizeDelta = new Vector2(70f, 70f);
        }
        else if(PlatformManager.Instance.IsTomko)
        {
            var closeRect = CloseButton.GetComponent<RectTransform>();
            closeRect.anchoredPosition = new Vector2(-98.1f, -80.69809f);
            closeRect.sizeDelta = new Vector2(57.6f, 57.6f);
            
            var refreshRect = RefreshButton.GetComponent<RectTransform>();
            refreshRect.anchoredPosition = new Vector2(-183.3f, -80.69809f);
            refreshRect.sizeDelta = new Vector2(57.6f, 57.6f);
            
            var guideRect = ShowGuideButton.GetComponent<RectTransform>();
            guideRect.anchoredPosition = new Vector2(-268.3f, -80.69809f);
            guideRect.sizeDelta = new Vector2(57.6f, 57.6f);
        }
        
        interactiveObject.gameObject.SetActive(true);
        CloseButton.gameObject.SetActive(true);

        
        
        if (!PlatformManager.Instance.IsVR)
        {
            
            // 👉 Gán scale theo trung bình giữa min và max
            float startScale = (interactiveObject.minScale);
            interactiveObject.transform.localScale = new Vector3(startScale, startScale, startScale);
            if(interactiveObject.zoomScrollbar) interactiveObject.zoomScrollbar.value = interactiveObject.GetZoomScrollbarValue(startScale);
            isBlur = true;
            interactiveObject.transform.SetParent(Camera.main.transform, false);
            interactiveObject.transform.localPosition = interactObjectLocalPosition;
            if (isSetRotation)
            {
                interactiveObject.transform.localEulerAngles = interactObjectLocalRotation;
            }
        }
        else
        {
            InteractiveObjectLocation.gameObject.SetActive(true);
            interactiveObject.transform.SetParent(InteractiveObjectLocation);
            interactiveObject.transform.localPosition = new Vector3(0, 0, 0);
            Debug.Log(interactObjectLocalPosition);
            //interactiveObject.transform.localScale = new Vector3(1, 1, 1);

        }
        
        if (openInfomationUI)
        {
            rect.gameObject.SetActive(true);
        }
        else
        {
            rect.gameObject.SetActive(false);
        }
        
        CharacterManager.Instance.DisableCharacter();
        InputManager.Instance.DisableInput();

        Debug.Log("triggerZoneStatic.triggerId:" + triggerZoneStatic.triggerId);
        
        if(hasInformationUI)
            AudioSubtitleManager.Instance.PlayAudioWithSubtitle(triggerZoneStatic.triggerId);
    }

    public void TurnOffBlur()
    {
        interactiveObject.transform.SetParent(null);
        // interactiveObject.GetComponent<BoxCollider>().enabled = false;
        // interactiveObject.transform.GetChild(0).transform.gameObject.SetActive(false);
        //objectDetail.SetActive(false);
        if (PlatformManager.Instance.IsVR)
            InteractiveObjectLocation.gameObject.SetActive(false);

        CloseButton.gameObject.SetActive(false);
        if (!PlatformManager.Instance.IsVR)
        {
            blurGameObject.SetActive(false);
            CharacterStateMachine.gameObject.layer = LayerMask.NameToLayer("Default");
        }
        
        interactiveObject.gameObject.SetActive(false);
        isBlur = false;

        StopGuide();
        SoundButton.image.sprite = SoundOn;
        isSoundOn = true;

        CharacterManager.Instance.EnableCharacter();
        InputManager.Instance.EnableInput();
        
        if(hasInformationUI)
            AudioSubtitleManager.Instance.StopAudioAndClearSubtitle();
        
        UIManager.Instance.EnableUI("UI_NAVIGATION");
        AntiqueManager.Instance.DisableAntiqueDetail(AntiqueID);
        
        if(!isAntique)
            SlideManager.Instance.SetPointerCollider(true);
    }

    private void ToggleSound()
    {
        if(!hasInformationUI)
        {
            SoundButton.image.sprite = SoundOff;
            isSoundOn = false;
            return;
        }
        
        if (isSoundOn)
        {
            AudioSubtitleManager.Instance.StopAudioAndClearSubtitle();
            SoundButton.image.sprite = SoundOff;
            isSoundOn = false;
        }
        else
        {
            AudioSubtitleManager.Instance.PlayAudioWithSubtitle(triggerZoneStatic.triggerId);
            SoundButton.image.sprite = SoundOn;
            isSoundOn = true;
        }
    }

    private void ToggleGuide()
    {
        if (isGuidePlaying)
        {
            StopGuide();
        }
        else
        {
            StartGuide();
        }
    }

    private void StartGuide()
    {
        RotateImage.rectTransform.anchoredPosition = animationImageAnchoredPos;
        ZoomImage.rectTransform.anchoredPosition = animationImageAnchoredPos;
        
        guideCoroutine = StartCoroutine(PlayGuideSequence());
        isGuidePlaying = true;
        ShowGuideButton.image.sprite = GuideOn;
    }

    private void StopGuide()
    {
        if (guideCoroutine != null)
            StopCoroutine(guideCoroutine);

        guideCoroutine = null;
        isGuidePlaying = false;
        ShowGuideButton.image.sprite = GuideOff;
        HideGuideUIElements();
    }

    private IEnumerator PlayGuideSequence()
    {
        string currentLanguage = SceneLog.IsVietnamese ? "vi" : "en";
        RotateImage.sprite = currentLanguage == "en" ? RotateSpriteEng : RotateSpriteVi;

        yield return FadeImageIn(RotateImage);
        yield return new WaitForSeconds(instructionDuration);
        yield return FadeImageOut(RotateImage);

        yield return FadeInZoomGroup();
        yield return new WaitForSeconds(instructionDuration);
        yield return FadeOutZoomGroup();

        StopGuide();
    }

    private IEnumerator FadeImageIn(Image img)
    {
        img.gameObject.SetActive(true);
        Color color = img.color;
        color.a = 0f;
        float time = 0;
        float duration = 1f;

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

    private IEnumerator FadeImageOut(Image img)
    {
        Color color = img.color;
        float time = 0;
        float duration = 1f;

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

    private IEnumerator FadeInZoomGroup()
    {
        yield return FadeUI(ZoomImage, 0f, 1f, 1f);
        StartCoroutine(FadeUI(ZoomFingerImage, 0f, 1f, 1f));
        StartCoroutine(FadeUI(ZoomArrowImage, 0f, 1f, 1f));
        StartCoroutine(FadeUI(ZoomText, 0f, 1f, 1f));
    }

    private IEnumerator FadeOutZoomGroup()
    {
        StartCoroutine(FadeUI(ZoomFingerImage, 1f, 0f, 1f));
        StartCoroutine(FadeUI(ZoomArrowImage, 1f, 0f, 1f));
        StartCoroutine(FadeUI(ZoomText, 1f, 0f, 1f));
        yield return FadeUI(ZoomImage, 1f, 0f, 1f);
    }

    private void SetInitialUIState()
    {
        RotateImage.gameObject.SetActive(false);
        ZoomImage.gameObject.SetActive(false);
        ZoomFingerImage.gameObject.SetActive(false);
        ZoomArrowImage.gameObject.SetActive(false);
        ZoomText.gameObject.SetActive(false);
    }

    private void HideGuideUIElements()
    {
        RotateImage.gameObject.SetActive(false);
        ZoomImage.gameObject.SetActive(false);
    }
}

}
