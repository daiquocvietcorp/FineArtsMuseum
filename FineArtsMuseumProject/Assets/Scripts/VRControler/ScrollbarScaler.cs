using UnityEngine;
using UnityEngine.UI;

public class ScrollbarScaler : MonoBehaviour
{
    [Header("Scrollbar điều khiển")]
    public Scrollbar scaleScrollbar;

    [Header("Đối tượng cần scale")]
    public Transform targetObject;

    [Header("Giới hạn scale")]
    public float minScale = 0.5f;
    public float maxScale = 2.0f;

    void Start()
    {
        if (scaleScrollbar != null)
        {
            // Gắn sự kiện khi kéo scrollbar
            scaleScrollbar.onValueChanged.AddListener(OnScrollChanged);
            // Đặt scale ban đầu theo vị trí scrollbar hiện tại
            OnScrollChanged(scaleScrollbar.value);
        }
    }

    void OnScrollChanged(float value)
    {
        if (targetObject != null)
        {
            float scale = Mathf.Lerp(minScale, maxScale, value);
            Debug.Log("scale: " + scale);
            targetObject.localScale = Vector3.one * scale;
        }
    }
}