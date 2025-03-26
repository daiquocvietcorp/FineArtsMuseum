using UnityEngine;
using UnityEngine.UI;

public class PaintDetail : MonoBehaviour
{
    [Header("Buttons")]
    public Button closeButton;
    public Button volumnButton;

    [Header("Volumn Sprites")]
    public Sprite volumnOnSprite;
    public Sprite volumnOffSprite;

    private bool isVolumnOn = true;
    private Image volumnImage;
    
    public TriggerPaintDetail triggerPaintDetail;

    void Start()
    {
        // Gán sự kiện cho nút Close
        closeButton.onClick.AddListener(ClosePanel);

        // Gán sự kiện cho nút Volumn
        volumnButton.onClick.AddListener(ToggleVolumn);

        // Lấy component Image của nút Volumn
        volumnImage = volumnButton.GetComponent<Image>();

        // Gán sprite ban đầu
        UpdateVolumnSprite();
    }

    void ClosePanel()
    {
        triggerPaintDetail.ResetFade();
        gameObject.SetActive(false);
    }

    void ToggleVolumn()
    {
        isVolumnOn = !isVolumnOn;
        UpdateVolumnSprite();

        // Ở đây bạn có thể thêm code bật/tắt âm thanh thực tế nếu cần
        Debug.Log("Volumn is now " + (isVolumnOn ? "On" : "Off"));
    }

    void UpdateVolumnSprite()
    {
        if (volumnImage != null)
        {
            volumnImage.sprite = isVolumnOn ? volumnOnSprite : volumnOffSprite;
        }
    }
}