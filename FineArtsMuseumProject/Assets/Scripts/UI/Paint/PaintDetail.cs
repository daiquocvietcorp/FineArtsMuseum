using System;
using InputController;
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
    
    [field: Header("Paint ID")]
    [field: SerializeField] private string PaintID { get; set; }

    public TriggerPaintDetail triggerPaintDetail;

    public string GetPaintID()
    {
        return PaintID;
    }
    
    void Start()
    {
        // Gán sự kiện cho nút Close
        closeButton.onClick.AddListener(() =>
        {
            ClosePanel();
            
            if (!PlatformManager.Instance.IsMobile && !PlatformManager.Instance.IsCloud) return;
            InputManager.Instance.EnableJoystick();
            InputManager.Instance.EnableJoystickRotation();
        });

        // Gán sự kiện cho nút Volumn
        volumnButton.onClick.AddListener(ToggleVolumn);

        // Lấy component Image của nút Volumn
        volumnImage = volumnButton.GetComponent<Image>();

        // Gán sprite ban đầu
        UpdateVolumnSprite();
    }

    private void OnEnable()
    {
        if (triggerPaintDetail.triggerPainting != null)
            AudioSubtitleManager.Instance.PlayAudioWithSubtitle(triggerPaintDetail.triggerPainting.GetComponent<TriggerZoneDynamic>().triggerId);
    }

    private void OnDisable()
    {
        volumnButton.image.sprite = volumnOnSprite;
        isVolumnOn = true;
    }


    public void ClosePanel()
    {
        volumnButton.image.sprite = volumnOnSprite;
        isVolumnOn = true;
        AudioSubtitleManager.Instance.StopAudioAndClearSubtitle();
        triggerPaintDetail.ResetFade();
        gameObject.SetActive(false);
    }

    void ToggleVolumn()
    {
        isVolumnOn = !isVolumnOn;
        UpdateVolumnSprite();

        if (isVolumnOn)
        {
            // Bật âm thanh kèm phụ đề
            AudioSubtitleManager.Instance.PlayAudioWithSubtitle(triggerPaintDetail.triggerPainting.GetComponent<TriggerZoneDynamic>().triggerId);
        }
        else
        {
            // Tắt âm thanh và phụ đề
            AudioSubtitleManager.Instance.StopAudioAndClearSubtitle();
        }

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