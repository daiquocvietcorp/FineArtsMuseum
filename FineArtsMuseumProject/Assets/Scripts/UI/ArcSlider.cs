using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ArcSlider : MonoBehaviour, IDragHandler
{
    public RectTransform thumb;
    public RectTransform dragArea;
    public Image fillArea;
    public float radius = 422f; // bán kính hình tròn
    [Range(0f, 1f)] public float t = 0.5f;

    // Cung từ trái → đỉnh → phải: 180° đến 0°
    public float startAngle = 180f; 
    public float endAngle = 0f;

    void Start()
    {
        UpdateThumbPosition();
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            dragArea, eventData.position, eventData.pressEventCamera, out localPos
        );

        float angle = Mathf.Atan2(localPos.y, localPos.x) * Mathf.Rad2Deg;

        // Giữ góc trong khoảng từ 180° đến 0° (tức là cung phía trên)
        angle = Mathf.Clamp(angle, endAngle, startAngle);
        t = Mathf.InverseLerp(startAngle, endAngle, angle);

        UpdateThumbPosition();
    }

    void UpdateThumbPosition()
    {
        float angle = Mathf.Lerp(startAngle, endAngle, t) * Mathf.Deg2Rad;
        float x = Mathf.Cos(angle) * radius;
        float y = Mathf.Sin(angle) * radius;
        thumb.anchoredPosition = new Vector2(x, y);
        fillArea.fillAmount = t;
    }
}