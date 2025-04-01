using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButtonHoverSprite : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image targetImage; // Image cần đổi sprite
    public Sprite defaultSprite;
    public Sprite hoverSprite;
    public Sprite selectedSprite;

    private bool isSelected = false;
    private bool isHover = false;

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        
        UpdateSprite();
    }

    private void UpdateSprite()
    {
        if (targetImage != null)
        {
            if (isSelected)
                targetImage.sprite = selectedSprite;
            else
            {
                if (isHover)
                {
                    targetImage.sprite = hoverSprite;
                }
                else
                {
                    targetImage.sprite = defaultSprite;
                }
            }
                
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHover = true;
        if (!isSelected && targetImage != null && hoverSprite != null)
        {
            targetImage.sprite = hoverSprite;
        }
        else if(isSelected && targetImage != null && hoverSprite != null)
        {
            targetImage.sprite = selectedSprite;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHover = false;
        if (!isSelected && targetImage != null && defaultSprite != null)
        {
            targetImage.sprite = defaultSprite;
        }
        else if (isSelected && targetImage != null && defaultSprite != null)
        {
            targetImage.sprite = selectedSprite;
        }
    }
}