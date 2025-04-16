using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;

public class AIHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    
    
    public RectTransform buttonTransform;
    public Image buttonImage;
    public TextMeshProUGUI buttonText;

    [Header("Text")]
    public string defaultText = "AI";
    public string hoverText = "AI DIỄN HOẠT";
    public string hoverTextEnglish = "AI ANIMATION";

    [Header("Button Size")]
    public float defaultWidth = 60f;
    public float hoverWidth = 160f;
    public float height = 60f;

    [Header("Pixels Per Unit")]
    public float defaultPPU = 1f;
    public float hoverPPU = 2f;

    [Header("Tween Settings")]
    public float duration = 0.3f;
    public Ease easing = Ease.OutBack;
    
    [Header("Sprites")]
    public Sprite defaultSprite;
    public Sprite hoverSprite;
    
    public Sprite defaultSpriteSelected;
    public Sprite hoverSpriteSelected;

    public bool isSelected = false;
    public bool isHovered = false;
    
    private Vector2 originalSize;
    private Sequence _enterSequence;
    private Sequence _exitSequence;
    
    private Tween _sizeTween;
    private Tween _ppuTween;

    public void SetDefaultSprite()
    {
        DisposePanel();
        buttonImage.sprite = defaultSprite;
        isSelected = false;
        isHovered = false;
        
    }
    
    public void OnClickedButton()
    {
        if (!isSelected)
        {
            /*if (isHovered)
            {
                if (hoverSprite != null)
                {
                    buttonImage.sprite = hoverSprite;
                }
            }
            else
            {
                if (defaultSprite != null)
                {
                    buttonImage.sprite = defaultSprite;
                }
            }*/
            DisposePanel();
            buttonImage.sprite = defaultSprite;
        }
        else
        {
            /*if (isHovered)
            {
                if (hoverSpriteSelected != null)
                {
                    buttonImage.sprite = hoverSpriteSelected;
                }
            }
            else
            {
                if (defaultSpriteSelected != null)
                {
                    buttonImage.sprite = defaultSpriteSelected;
                }
            }*/
            ExpandPanel();
            buttonImage.sprite = hoverSpriteSelected;
        }
    }

    void Start()
    {
        if (buttonTransform == null) buttonTransform = GetComponent<RectTransform>();
        if (buttonImage == null) buttonImage = GetComponent<Image>();
        if (buttonText != null)
        {
            buttonText.text = defaultText;
        }

        originalSize = new Vector2(defaultWidth, height);
        buttonTransform.sizeDelta = originalSize;
        buttonImage.pixelsPerUnitMultiplier = defaultPPU;
        
        if (defaultSprite != null)
        {
            buttonImage.sprite = defaultSprite;
        }
    }

    

    public void OnPointerEnter(PointerEventData eventData)
    {
        //if(PlatformManager.Instance.IsStandalone || PlatformManager.Instance.IsMobile || PlatformManager.Instance.IsCloud || PlatformManager.Instance.IsTomko) return;
        
        return;
        ExpandPanel();
    }

    private void ExpandPanel()
    {
        isHovered = true;

        // Stop coroutine nếu đang chạy
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        // Kill tween cũ nếu đang hoạt động
        _sizeTween?.Kill();
        _ppuTween?.Kill();

        // Reset text trước khi typing
        buttonText.text = "";

        // Bắt đầu typing
        string currentLanguage = SceneLog.IsVietnamese ? "vi" : "en";
        string targetText = (currentLanguage == "vi") ? hoverText : hoverTextEnglish;
        typingCoroutine = StartCoroutine(AnimateTypingText(targetText, duration));

        // Animate button size
        _sizeTween = buttonTransform.DOSizeDelta(new Vector2(hoverWidth, height), duration)
            .SetEase(easing);

        // Animate pixels per unit
        _ppuTween = DOTween.To(() => buttonImage.pixelsPerUnitMultiplier,
            x => buttonImage.pixelsPerUnitMultiplier = x,
            hoverPPU,
            duration).SetEase(easing);

        if (hoverSpriteSelected != null) buttonImage.sprite = hoverSpriteSelected;
        
        return;
        // Sprite change
        if (!isSelected)
        {
            if (hoverSprite != null) buttonImage.sprite = hoverSprite;
        }
        else
        {
            if (hoverSpriteSelected != null) buttonImage.sprite = hoverSpriteSelected;
        }
    }

    private Coroutine typingCoroutine;

    private IEnumerator AnimateTypingText(string fullText, float totalDuration)
    {
        if (buttonText == null) yield break;

        buttonText.text = "";
        float delay = totalDuration / fullText.Length;

        for (int i = 0; i < fullText.Length; i++)
        {
            buttonText.text += fullText[i];
            yield return new WaitForSeconds(delay);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        return;
        DisposePanel();
    }

    private void DisposePanel()
    {
        isHovered = false;

        // Stop typing
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        // Kill tween cũ nếu còn
        _sizeTween?.Kill();
        _ppuTween?.Kill();

        // Reset về trạng thái ban đầu
        buttonTransform.DOSizeDelta(originalSize, duration).SetEase(easing);
        DOTween.To(() => buttonImage.pixelsPerUnitMultiplier,
            x => buttonImage.pixelsPerUnitMultiplier = x,
            defaultPPU,
            duration).SetEase(easing);

        if (buttonText != null)
        {
            buttonText.text = defaultText;
        }
        
        if (defaultSprite != null) buttonImage.sprite = defaultSprite;
        
        return;
        if (!isSelected)
        {
            if (defaultSprite != null) buttonImage.sprite = defaultSprite;
        }
        else
        {
            if (defaultSpriteSelected != null) buttonImage.sprite = defaultSpriteSelected;
        }
    }
}