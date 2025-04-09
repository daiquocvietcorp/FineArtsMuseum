using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ArcSlider : MonoBehaviour, IDragHandler
{
    public RectTransform thumb;
    public float radius = 200f;
    [Range(0f, 1f)]
    public float t = 0f; // 0 = 12h, 1 = 9h

    public float minAngle = 180f;
    public float maxAngle = 0f;

    void Start()
    {
        UpdateThumbPosition();
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            transform as RectTransform, 
            eventData.position, 
            eventData.pressEventCamera, 
            out localPos
        );

        float angle = Mathf.Atan2(localPos.y, localPos.x) * Mathf.Rad2Deg;

        // Clamp theo hướng mong muốn: 90° (12h) đến 180° (9h)
        angle = Mathf.Clamp(angle, maxAngle, minAngle);

        // Gán t từ góc
        t = Mathf.InverseLerp(maxAngle, minAngle, angle);
        UpdateThumbPosition();
    }

    void UpdateThumbPosition()
    {
        float angle = Mathf.Lerp(maxAngle, minAngle, t) * Mathf.Deg2Rad;
        float x = Mathf.Cos(angle) * radius;
        float y = Mathf.Sin(angle) * radius;
        thumb.anchoredPosition = new Vector2(x, y);
    }
}