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

    public void SetDefaultSprite()
    {
        buttonImage.sprite = defaultSprite;
        isSelected = false;
        isHovered = false;
    }
    
    public void OnClickedButton()
    {
        if (!isSelected)
        {
            if (isHovered)
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
            }
                
        }
        else
        {
            if (isHovered)
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
            }
            
            
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
        
        // _enterSequence = DOTween.Sequence();
        // _enterSequence.Append(buttonTransform.DOSizeDelta(new Vector2(defaultWidth, height), duration));
        // _enterSequence.AppendCallback(() =>
        // {
        //     if (buttonText != null) buttonText.text = hoverText;
        //     if (!isSelected)
        //     {
        //         if (hoverSprite != null)
        //         {
        //             buttonImage.sprite = hoverSprite;
        //         }
        //     }
        //     else
        //     {
        //         if (hoverSpriteSelected != null)
        //         {
        //             buttonImage.sprite = hoverSpriteSelected;
        //         }
        //     }
        // });
        // _enterSequence.Join(DOTween.To(() => buttonImage.pixelsPerUnitMultiplier,
        //     x => buttonImage.pixelsPerUnitMultiplier = x,
        //     defaultPPU,
        //     duration));
        // _enterSequence.SetAutoKill(false);
        //
        // _exitSequence = DOTween.Sequence();
        // _exitSequence.AppendCallback(() =>
        // {
        //     buttonTransform.DOSizeDelta(originalSize, duration).SetEase(easing);
        //     if (buttonText != null) buttonText.text = defaultText;
        //
        //     DOTween.To(() => buttonImage.pixelsPerUnitMultiplier,
        //         x => buttonImage.pixelsPerUnitMultiplier = x,
        //         defaultPPU,
        //         duration).SetEase(easing);
        //
        //     if (!isSelected)
        //     {
        //         if (defaultSprite != null)
        //         {
        //             buttonImage.sprite = defaultSprite;
        //         }
        //     }
        //     else
        //     {
        //         if (defaultSpriteSelected != null)
        //         {
        //             buttonImage.sprite = defaultSpriteSelected;
        //         }
        //     }
        // });
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // _enterSequence.Restart();
        // return;
        isHovered = true;
        buttonTransform.DOSizeDelta(new Vector2(hoverWidth, height), duration).SetEase(easing);
        if (buttonText != null) buttonText.text = hoverText;

        DOTween.To(() => buttonImage.pixelsPerUnitMultiplier,
            x => buttonImage.pixelsPerUnitMultiplier = x,
            hoverPPU,
            duration).SetEase(easing);

        if (!isSelected)
        {
            if (hoverSprite != null)
            {
                buttonImage.sprite = hoverSprite;
            }
        }
        else
        {
            if (hoverSpriteSelected != null)
            {
                buttonImage.sprite = hoverSpriteSelected;
            }
        }
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // _exitSequence.Restart();
        // return;
        isHovered = false;
        buttonTransform.DOSizeDelta(originalSize, duration).SetEase(easing);
        if (buttonText != null) buttonText.text = defaultText;

        DOTween.To(() => buttonImage.pixelsPerUnitMultiplier,
            x => buttonImage.pixelsPerUnitMultiplier = x,
            defaultPPU,
            duration).SetEase(easing);

        if (!isSelected)
        {
            if (defaultSprite != null)
            {
                buttonImage.sprite = defaultSprite;
            }
        }
        else
        {
            if (defaultSpriteSelected != null)
            {
                buttonImage.sprite = defaultSpriteSelected;
            }
        }
        
    }
}