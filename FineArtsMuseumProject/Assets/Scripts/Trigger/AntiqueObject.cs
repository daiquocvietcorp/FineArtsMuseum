

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

    [Header("Settings")]
    public float instructionDuration = 10f;
    public Vector3 interactObjectLocalPosition = Vector3.forward;
    public Vector2 animationImageAnchoredPos;

    [Header("Dependencies")]
    public CharacterStateMachine CharacterStateMachine;
    public  TriggerZoneStatic triggerZoneStatic;

    private bool isBlur = false;
    private bool isGuidePlaying = false;
    private bool isSoundOn = true;
    private Coroutine guideCoroutine;

    private void Start()
    {
        CloseButton.onClick.AddListener(TurnOffBlur);
        ShowGuideButton.onClick.AddListener(ToggleGuide);
        SoundButton.onClick.AddListener(ToggleSound);

        //triggerZoneStatic = GetComponent<TriggerZoneStatic>();
        SetInitialUIState();
        SoundButton.image.sprite = SoundOn;
        //CloseButton.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        ActivateInteractiveMode();
        StartCoroutine(DelayedStartGuide());
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
        CharacterStateMachine.gameObject.layer = LayerMask.NameToLayer("Default");
        blurGameObject.SetActive(true);
        interactiveObject.gameObject.SetActive(true);
        CloseButton.gameObject.SetActive(true);

        interactiveObject.transform.SetParent(Camera.main.transform);
        interactiveObject.transform.localPosition = interactObjectLocalPosition;

        // 👉 Gán scale theo trung bình giữa min và max
        float avgScale = (interactiveObject.minScale + interactiveObject.maxScale) / 2f;
        interactiveObject.transform.localScale = new Vector3(avgScale, avgScale, avgScale);
        interactiveObject.zoomScrollbar.value = interactiveObject.GetZoomScrollbarValue(avgScale);

        isBlur = true;
        CharacterManager.Instance.DisableCharacter();
        InputManager.Instance.DisableInput();

        Debug.Log("triggerZoneStatic.triggerId:" + triggerZoneStatic.triggerId);
        AudioSubtitleManager.Instance.PlayAudioWithSubtitle(triggerZoneStatic.triggerId);
    }

    public void TurnOffBlur()
    {
        CharacterStateMachine.gameObject.layer = LayerMask.NameToLayer("Default");
        //objectDetail.SetActive(false);
        CloseButton.gameObject.SetActive(false);
        blurGameObject.SetActive(false);
        interactiveObject.gameObject.SetActive(false);
        isBlur = false;

        StopGuide();
        SoundButton.image.sprite = SoundOn;
        isSoundOn = true;

        CharacterManager.Instance.EnableCharacter();
        InputManager.Instance.EnableInput();
        AudioSubtitleManager.Instance.StopAudioAndClearSubtitle();
        UIManager.Instance.EnableUI("UI_NAVIGATION");
        AntiqueManager.Instance.DisableAntiqueDetail(AntiqueID);

    }

    private void ToggleSound()
    {
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
        string currentLanguage = PlayerPrefs.GetString("Language", "vi");
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
