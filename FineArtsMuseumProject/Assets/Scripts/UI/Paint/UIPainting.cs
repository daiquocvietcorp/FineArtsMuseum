using System;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using Trigger;
using Unity.VisualScripting;
using UnityEngine.Serialization;
using UnityEngine.Video;
using Sequence = DG.Tweening.Sequence;

public class UIPainting : UIBasic
{
    public Button guideButton;
    public Button zoomButton;
    public Button aiButton;
    public Button refreshButton;

    public Button guideButton_mobile;
    public Button zoomButton_mobile;
    public Button aiButton_mobile;
    public Button refreshButton_mobile;
    
    public Button guideButton_vr;
    public Button zoomButton_vr;
    public Button aiButton_vr;
    public Button refreshButton_vr;
    
    public Button guideButton_tomko;
    public Button zoomButton_tomko;
    public Button aiButton_tomko;
    public Button refreshButton_tomko;
    
    public bool isGuide = false;
    public bool isZoom = false;
    public bool isAI = false;

    public TextMeshProUGUI AIAnimationCautionText;
    public Image AIAnimationIcon;
    public TextMeshProUGUI AIAnimationCautionText_mobile;
    public Image AIAnimationIcon_mobile;
    public TextMeshProUGUI AIAnimationCautionText_vr;
    public Image AIAnimationIcon_vr;
    public TextMeshProUGUI AIAnimationCautionText_tomko;
    public Image AIAnimationIcon_tomko;

    [SerializeField] private GameObject MagnifierCanvas;
    
    [FormerlySerializedAs("guideDefaultSprite")] public Sprite guideButtonDefaultSprite;
    [FormerlySerializedAs("zoomDefaultSprite")] public Sprite zoomButtonDefaultSprite;

    public List<VideoClip> VideoClips;
    public VideoPlayer VideoPlayer;

    public Texture tranhDefaultSprite;
    public GameObject tranh;
    public RenderTexture videoRenderTexture;

    public GameObject BlinkCanvas;
    
    public Sprite guideRotateViSprite;
    public Sprite guideRotateEnglishSprite;
    public Sprite zoomViSprite;
    public Sprite zoomEnglishSprite;
    public Sprite zoomViSpriteTomko;
    public Sprite zoomEnglishSpriteTomko;
    public Sprite guideZoomKinhViSprite;
    public Sprite guideZoomKinhEnglishSprite;
    public Sprite guideAIGenViSprite;
    public Sprite guideAIGenEnglishSprite;

    public Image guideRotateImage;
    public Image guideZoomImage;
    
    public Image guideRotateImage_mobile;
    public Image guideZoomImage_mobile;
    
    public Image guideRotateImage_vr;
    public Image guideZoomImage_vr;
    
    public Image guideRotateImage_tomko;
    public Image guideZoomImage_tomko;
    
    public Image guideZoomKinhImage;
    public Image guideAIGenImage; 
    
    public Image guideZoomKinhImage_mobile;
    public Image guideAIGenImage_mobile;
    
    public Image guideZoomKinhImage_vr;
    public Image guideAIGenImage_vr;
    
    public Image guideZoomKinhImage_tomko;
    public Image guideAIGenImage_tomko; 
    
    public MagnifierHover magnifierHover_mobile;
    public MagnifierHover magnifierHover_other;
    
    [HideInInspector]public MagnifierHover magnifierHover;
    
    public Vector3 guideRotateDefaultPosition;
    public Vector3 guideZoomDefaultPosition;

    public float durartion = 9f;
    
    public PaintRotateAndZoom paintRotateAndZoom;
    
    private Coroutine _guideRotateCoroutine;
    private Coroutine _blinkCoroutine;
    private Coroutine _aiCautionCoroutine;
    
    
    
    [field: Header("Paint ID")]
    [field: SerializeField] private string PaintID { get; set; }

    public string GetPaintID()
    {
        return PaintID;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        if (PlatformManager.Instance.IsStandalone || PlatformManager.Instance.IsWebGL)
        {
            guideButton.onClick.AddListener(GuidePaintingClicked);
            zoomButton.onClick.AddListener(ZoomPaintingClicked);
            aiButton.onClick.AddListener(AIPaintingClicked);
            refreshButton.onClick.AddListener(RefreshPaintingClicked);
            
            guideRotateDefaultPosition = guideRotateImage.transform.localPosition;
            guideZoomDefaultPosition = guideZoomImage.transform.localPosition;
            magnifierHover = magnifierHover_other;
        }
        
        if (PlatformManager.Instance.IsMobile || PlatformManager.Instance.IsCloud)
        {
            guideButton_mobile.onClick.AddListener(GuidePaintingClicked);
            zoomButton_mobile.onClick.AddListener(ZoomPaintingClicked);
            aiButton_mobile.onClick.AddListener(AIPaintingClicked);
            refreshButton_mobile.onClick.AddListener(RefreshPaintingClicked);
            
            guideRotateDefaultPosition = guideRotateImage_mobile.transform.localPosition;
            guideZoomDefaultPosition = guideZoomImage_mobile.transform.localPosition;
            magnifierHover = magnifierHover_mobile;
        }
        if (PlatformManager.Instance.IsVR)
        {
            guideButton_vr.onClick.AddListener(GuidePaintingClicked);
            zoomButton_vr.onClick.AddListener(ZoomPaintingClicked);
            aiButton_vr.onClick.AddListener(AIPaintingClicked);
            refreshButton_vr.onClick.AddListener(RefreshPaintingClicked);
            
            // guideRotateDefaultPosition = guideRotateImage_vr.transform.localPosition;
            // guideZoomDefaultPosition = guideZoomImage_vr.transform.localPosition;
            // magnifierHover = magnifierHover_other;
        }
        if (PlatformManager.Instance.IsTomko)
        {
            guideButton_tomko.onClick.AddListener(GuidePaintingClicked);
            zoomButton_tomko.onClick.AddListener(ZoomPaintingClicked);
            aiButton_tomko.onClick.AddListener(AIPaintingClicked);
            refreshButton_tomko.onClick.AddListener(RefreshPaintingClicked);
            
            guideRotateDefaultPosition = guideRotateImage_tomko.transform.localPosition;
            guideZoomDefaultPosition = guideZoomImage_tomko.transform.localPosition;
            magnifierHover = magnifierHover_other;
        }
        
    }

    private Sequence _guideSequence;

    public void SetDefaultZoom()
    {
        isZoom = false;
        if(zoomButton)zoomButton.GetComponent<UIButtonHoverSprite>().SetSelected(isZoom);
        if(zoomButton_mobile)zoomButton_mobile.GetComponent<UIButtonHoverSprite>().SetSelected(isZoom);
        if(zoomButton_vr)zoomButton_vr.GetComponent<UIButtonHoverSprite>().SetSelected(isZoom);
        if(zoomButton_tomko)zoomButton_tomko.GetComponent<UIButtonHoverSprite>().SetSelected(isZoom);
        magnifierHover.enabled = false;
        PaintingDetailManager.Instance.SetZoomPainting(false);
        paintRotateAndZoom.enabled = true;
        
    }
    
    public void SetDefaultAll()
    {
        isGuide = false;
        isZoom = false;
        isAI = false;
        
        paintRotateAndZoom.canRotate = true;
        
        if(PlatformManager.Instance.IsStandalone || PlatformManager.Instance.IsWebGL)
        {
            guideButton.GetComponent<UIButtonHoverSprite>().SetSelected(isGuide);
            zoomButton.GetComponent<UIButtonHoverSprite>().SetSelected(isZoom);
            aiButton.GetComponent<AIHoverEffect>().SetDefaultSprite();
        }
        
        if(PlatformManager.Instance.IsMobile || PlatformManager.Instance.IsCloud)
        {
            guideButton_mobile.GetComponent<UIButtonHoverSprite>().SetSelected(isGuide);
            zoomButton_mobile.GetComponent<UIButtonHoverSprite>().SetSelected(isZoom);
            aiButton_mobile.GetComponent<AIHoverEffect>().SetDefaultSprite();
        }
        if(PlatformManager.Instance.IsVR)
        {
            guideButton_vr.GetComponent<UIButtonHoverSprite>().SetSelected(isGuide);
            zoomButton_vr.GetComponent<UIButtonHoverSprite>().SetSelected(isZoom);
            aiButton_vr.GetComponent<AIHoverEffect>().SetDefaultSprite();
        }

        if (PlatformManager.Instance.IsTomko)
        {
            guideButton_tomko.GetComponent<UIButtonHoverSprite>().SetSelected(isGuide);
            zoomButton_tomko.GetComponent<UIButtonHoverSprite>().SetSelected(isZoom);
            aiButton_tomko.GetComponent<AIHoverEffect>().SetDefaultSprite();
        }
        StopGuideSequence();
        SetGuideImageOff();
        
        paintRotateAndZoom.SmoothOriginResetTransform();
        paintRotateAndZoom.enabled = false;
        
        BlinkCanvas.SetActive(false);
        VideoPlayer.Stop();
        VideoPlayer.gameObject.SetActive(false);
        tranh.GetComponent<Renderer>().material.mainTexture = tranhDefaultSprite;
        // Gán texture phát sáng
        tranh.GetComponent<Renderer>().material.SetTexture("_EmissionMap", tranhDefaultSprite);
        
        if (_blinkCoroutine != null)
        {
            StopCoroutine(_blinkCoroutine);
            _blinkCoroutine = null;
        }
        
        TurnOffCautionTextAndImage();
    }
    
public void GuidePaintingClicked()
{
    isAI = false;
    isZoom = false;

    paintRotateAndZoom.canRotate = true;
    
    if(PlatformManager.Instance.IsStandalone || PlatformManager.Instance.IsWebGL)
    {
        zoomButton.GetComponent<UIButtonHoverSprite>().SetSelected(isZoom);
        aiButton.GetComponent<AIHoverEffect>().SetDefaultSprite();
    }
    
    if(PlatformManager.Instance.IsVR)
    {
        zoomButton_vr.GetComponent<UIButtonHoverSprite>().SetSelected(isZoom);
        aiButton_vr.GetComponent<AIHoverEffect>().SetDefaultSprite();
    }

    
    if(PlatformManager.Instance.IsMobile || PlatformManager.Instance.IsCloud)
    {
        zoomButton_mobile.GetComponent<UIButtonHoverSprite>().SetSelected(isZoom);
        aiButton_mobile.GetComponent<AIHoverEffect>().SetDefaultSprite();
    }
    
    if (PlatformManager.Instance.IsTomko)
    {
        zoomButton_tomko.GetComponent<UIButtonHoverSprite>().SetSelected(isZoom);
        aiButton_tomko.GetComponent<AIHoverEffect>().SetDefaultSprite();
    }
    
    TurnOffCautionTextAndImage();
    
    magnifierHover.enabled = false;
    PaintingDetailManager.Instance.SetZoomPainting(false);

    BlinkCanvas.SetActive(false);
    VideoPlayer.Stop();
    VideoPlayer.gameObject.SetActive(false);
    tranh.GetComponent<Renderer>().material.mainTexture = tranhDefaultSprite;
    // Gán texture phát sáng
    tranh.GetComponent<Renderer>().material.SetTexture("_EmissionMap", tranhDefaultSprite);
    if (_blinkCoroutine != null)
    {
        StopCoroutine(_blinkCoroutine);
        _blinkCoroutine = null;
    }
    
    
    
    if (isGuide)
    {
        //guideButton.image.sprite = guideDefaultSprite;
        StopGuideSequence();
        SetGuideImageOff();
        isGuide = false;
        paintRotateAndZoom.enabled = true;
        if(PlatformManager.Instance.IsStandalone || PlatformManager.Instance.IsWebGL)
        {
            guideButton.GetComponent<UIButtonHoverSprite>().SetSelected(isGuide);
        }
        else if(PlatformManager.Instance.IsMobile || PlatformManager.Instance.IsCloud)
        {
            guideButton_mobile.GetComponent<UIButtonHoverSprite>().SetSelected(isGuide);
        }
        else if(PlatformManager.Instance.IsVR)
        {
            guideButton_vr.GetComponent<UIButtonHoverSprite>().SetSelected(isGuide);
        }
        else if(PlatformManager.Instance.IsTomko)
        {
            guideButton_tomko.GetComponent<UIButtonHoverSprite>().SetSelected(isGuide);
        }
    }
    else
    {
        UIPaintingManager.Instance.EnableUIPainting(PaintID);
        //guideButton.image.sprite = guideSelectedSprite;
        StartGuideSequence();
        isGuide = true;
        paintRotateAndZoom.SmoothAverageResetTransform();
        paintRotateAndZoom.enabled = true;
        
        if(PlatformManager.Instance.IsStandalone || PlatformManager.Instance.IsWebGL)
        {
            guideButton.GetComponent<UIButtonHoverSprite>().SetSelected(isGuide);
        }
        else if(PlatformManager.Instance.IsMobile || PlatformManager.Instance.IsCloud)
        {
            guideButton_mobile.GetComponent<UIButtonHoverSprite>().SetSelected(isGuide);
        }
        else if(PlatformManager.Instance.IsVR || PlatformManager.Instance.IsWebGL)
        {
            guideButton_vr.GetComponent<UIButtonHoverSprite>().SetSelected(isGuide);
        }
        else if(PlatformManager.Instance.IsTomko)
        {
            guideButton_tomko.GetComponent<UIButtonHoverSprite>().SetSelected(isGuide);
        }
    }
}

private void StopGuideSequence()
{
    if (_guideSequence != null && _guideSequence.IsActive())
    {
        _guideSequence.Kill();
        _guideSequence = null;
    }
}

private void SetGuideImageOff()
{
    Image guideZoom = null;
    Image guideRotate = null;
    Image zoomKinh = null;
    Image aiGen    = null;
    
    if(PlatformManager.Instance.IsStandalone || PlatformManager.Instance.IsWebGL)
    {
        guideZoom = guideZoomImage;
        guideRotate = guideRotateImage;
        zoomKinh = guideZoomKinhImage;
        aiGen = guideAIGenImage;

    }
        
    if(PlatformManager.Instance.IsMobile || PlatformManager.Instance.IsCloud)
    {
        guideZoom = guideZoomImage_mobile;
        guideRotate = guideRotateImage_mobile;
        zoomKinh = guideZoomKinhImage_mobile;
        aiGen = guideAIGenImage_mobile;
    }
    if(PlatformManager.Instance.IsVR)
    {
        guideZoom = guideZoomImage_vr;
        guideRotate = guideRotateImage_vr;
        zoomKinh = guideZoomKinhImage_vr;
        aiGen = guideAIGenImage_vr;
    }

    if (PlatformManager.Instance.IsTomko)
    {
        guideZoom = guideZoomImage_tomko;
        guideRotate = guideRotateImage_tomko;
        zoomKinh = guideZoomKinhImage_tomko;
        aiGen = guideAIGenImage_tomko;
    }
    
    guideZoom.transform.localPosition = guideZoomDefaultPosition;
    guideRotate.transform.localPosition = guideRotateDefaultPosition;
    zoomKinh.transform.localPosition = guideZoomDefaultPosition;
    aiGen.transform.localPosition = guideRotateDefaultPosition;
    

    guideZoom.DOKill();
    guideRotate.DOKill();
    zoomKinh.DOKill();
    aiGen.DOKill();

    guideZoom.DOFade(1f, 0f);
    guideRotate.DOFade(1f, 0f);
    zoomKinh.DOFade(1f, 0f);
    aiGen.DOFade(1f, 0f);
    
    guideZoom.gameObject.SetActive(false);
    guideRotate.gameObject.SetActive(false);
    zoomKinh.gameObject.SetActive(false);
    aiGen.gameObject.SetActive(false);
}

private void StartGuideSequence()
{
    isGuide = true; // Bật flag ngay khi bắt đầu sequence
    
    // Thiết lập sprite cho các ảnh theo ngôn ngữ

    string currentLanguage = SceneLog.IsVietnamese ? "vi" : "en";
    
    if (currentLanguage == "vi")
    {
        // Ảnh chính
        guideRotateImage.sprite = guideRotateViSprite;
        guideZoomImage.sprite = zoomViSprite;
        guideZoomKinhImage.sprite = guideZoomKinhViSprite;
        guideAIGenImage.sprite = guideAIGenViSprite;
        
        guideRotateImage_mobile.sprite = guideRotateViSprite;
        guideZoomImage_mobile.sprite = zoomViSprite;
        guideZoomKinhImage_mobile.sprite = guideZoomKinhViSprite;
        guideAIGenImage_mobile.sprite = guideAIGenViSprite;
        
        guideRotateImage_tomko.sprite = guideRotateViSprite;
        guideZoomImage_tomko.sprite = zoomViSpriteTomko;
        guideZoomKinhImage_tomko.sprite = guideZoomKinhViSprite;
        guideAIGenImage_tomko.sprite = guideAIGenViSprite;
        // Ảnh phụ (giả sử bạn đã gán sprite từ Inspector)
        // Nếu cần xử lý riêng theo ngôn ngữ thì bổ sung tại đây
    }
    else if (currentLanguage == "en")
    {
        guideRotateImage.sprite = guideRotateEnglishSprite;
        guideZoomImage.sprite = zoomEnglishSprite;
        guideZoomKinhImage.sprite = guideZoomKinhEnglishSprite;
        guideAIGenImage.sprite = guideAIGenEnglishSprite;
        
        guideRotateImage_mobile.sprite = guideRotateEnglishSprite;
        guideZoomImage_mobile.sprite = zoomEnglishSprite;
        guideZoomKinhImage_mobile.sprite = guideZoomKinhEnglishSprite;
        guideAIGenImage_mobile.sprite = guideAIGenEnglishSprite;
        
        guideRotateImage_tomko.sprite = guideRotateEnglishSprite;
        guideZoomImage_tomko.sprite = zoomEnglishSpriteTomko;
        guideZoomKinhImage_tomko.sprite = guideZoomKinhEnglishSprite;
        guideAIGenImage_tomko.sprite = guideAIGenEnglishSprite;
    }
    
    // Chọn đối tượng theo nền tảng cho 4 nhóm
    Image _rotateImage = null, _zoomImage = null, _zoomKinhImage = null, _aiGenImage = null;
    if (PlatformManager.Instance.IsStandalone || PlatformManager.Instance.IsWebGL)
    {
        _rotateImage = guideRotateImage;
        _zoomImage = guideZoomImage;
        _zoomKinhImage = guideZoomKinhImage;
        _aiGenImage = guideAIGenImage;
    }
    else if (PlatformManager.Instance.IsMobile || PlatformManager.Instance.IsCloud)
    {
        _rotateImage = guideRotateImage_mobile;
        _zoomImage = guideZoomImage_mobile;
        _zoomKinhImage = guideZoomKinhImage_mobile;
        _aiGenImage = guideAIGenImage_mobile;
    }
    else if (PlatformManager.Instance.IsVR)
    {
        _rotateImage = guideRotateImage_vr;
        _zoomImage = guideZoomImage_vr;
        _zoomKinhImage = guideZoomKinhImage_vr;
        _aiGenImage = guideAIGenImage_vr;
    }
    else if (PlatformManager.Instance.IsTomko)
    {
        _rotateImage = guideRotateImage_tomko;
        _zoomImage = guideZoomImage_tomko;
        _zoomKinhImage = guideZoomKinhImage_tomko;
        _aiGenImage = guideAIGenImage_tomko;
    }

    // Xác định các vị trí cho hiệu ứng của từng nhóm.
    // Nhóm 1 và nhóm 4 sử dụng vị trí dựa trên guideRotateDefaultPosition
    // Nhóm 2 và nhóm 3 sử dụng vị trí dựa trên guideZoomDefaultPosition
    Vector3 rotateStartPos = guideRotateDefaultPosition;
    rotateStartPos.x -= 5f;
    Vector3 rotateEndPos = guideRotateDefaultPosition;
    Vector3 rotateOutPos = guideRotateDefaultPosition;
    rotateOutPos.x += 5f;
    
    Vector3 zoomStartPos = guideZoomDefaultPosition;
    zoomStartPos.x -= 5f;
    Vector3 zoomEndPos = guideZoomDefaultPosition;
    Vector3 zoomOutPos = guideZoomDefaultPosition;
    zoomOutPos.x += 5f;

    // Reset trạng thái ban đầu cho 4 nhóm: đặt vị trí và alpha = 0, ẩn GameObject
    _rotateImage.transform.localPosition = guideRotateDefaultPosition;
    _rotateImage.DOFade(0f, 0f);
    _rotateImage.gameObject.SetActive(false);
    
    _zoomImage.transform.localPosition = guideZoomDefaultPosition;
    _zoomImage.DOFade(0f, 0f);
    _zoomImage.gameObject.SetActive(false);

    _zoomKinhImage.transform.localPosition = guideZoomDefaultPosition;
    _zoomKinhImage.DOFade(0f, 0f);
    _zoomKinhImage.gameObject.SetActive(false);

    _aiGenImage.transform.localPosition = guideRotateDefaultPosition;
    _aiGenImage.DOFade(0f, 0f);
    _aiGenImage.gameObject.SetActive(false);

    // Tạo DOTween Sequence chính, chạy 4 nhóm nối tiếp
    _guideSequence = DOTween.Sequence();

    // ========== Nhóm 1: guideRotate ==========
    _guideSequence.AppendCallback(() => {
        _rotateImage.gameObject.SetActive(true);
        _rotateImage.transform.localPosition = rotateStartPos;
    });
    _guideSequence.Append(_rotateImage.DOFade(1f, 1f));
    _guideSequence.Join(_rotateImage.rectTransform.DOLocalMoveX(rotateEndPos.x, 1f));
    _guideSequence.AppendInterval(durartion);
    _guideSequence.Append(_rotateImage.DOFade(0f, 1f));
    _guideSequence.Join(_rotateImage.rectTransform.DOLocalMoveX(rotateOutPos.x, 1f));
    _guideSequence.AppendCallback(() => { _rotateImage.gameObject.SetActive(false); });

    // ========== Nhóm 2: guideZoom ==========
    _guideSequence.AppendCallback(() => {
        _zoomImage.gameObject.SetActive(true);
        _zoomImage.transform.localPosition = zoomStartPos;
    });
    _guideSequence.Append(_zoomImage.DOFade(1f, 1f));
    _guideSequence.Join(_zoomImage.rectTransform.DOLocalMoveX(zoomEndPos.x, 1f));
    _guideSequence.AppendInterval(durartion);
    _guideSequence.Append(_zoomImage.DOFade(0f, 1f));
    _guideSequence.Join(_zoomImage.rectTransform.DOLocalMoveX(zoomOutPos.x, 1f));
    _guideSequence.AppendCallback(() => { _zoomImage.gameObject.SetActive(false); });

    // ========== Nhóm 3: guideZoomKinh ==========
    _guideSequence.AppendCallback(() => {
        _zoomKinhImage.gameObject.SetActive(true);
        _zoomKinhImage.transform.localPosition = zoomStartPos;
    });
    _guideSequence.Append(_zoomKinhImage.DOFade(1f, 1f));
    _guideSequence.Join(_zoomKinhImage.rectTransform.DOLocalMoveX(zoomEndPos.x, 1f));
    _guideSequence.AppendInterval(durartion);
    _guideSequence.Append(_zoomKinhImage.DOFade(0f, 1f));
    _guideSequence.Join(_zoomKinhImage.rectTransform.DOLocalMoveX(zoomOutPos.x, 1f));
    _guideSequence.AppendCallback(() => { _zoomKinhImage.gameObject.SetActive(false); });

    // ========== Nhóm 4: guideAIGen ==========
    _guideSequence.AppendCallback(() => {
        _aiGenImage.gameObject.SetActive(true);
        _aiGenImage.transform.localPosition = rotateStartPos; // Sử dụng vị trí của rotate, có thể thay đổi nếu cần
    });
    _guideSequence.Append(_aiGenImage.DOFade(1f, 1f));
    _guideSequence.Join(_aiGenImage.rectTransform.DOLocalMoveX(rotateEndPos.x, 1f));
    _guideSequence.AppendInterval(durartion);
    _guideSequence.Append(_aiGenImage.DOFade(0f, 1f));
    _guideSequence.Join(_aiGenImage.rectTransform.DOLocalMoveX(rotateOutPos.x, 1f));
    _guideSequence.AppendCallback(() => { _aiGenImage.gameObject.SetActive(false); });

    // Sau khi kết thúc sequence, reset lại trạng thái
    _guideSequence.AppendCallback(() => {
        isGuide = false;
        paintRotateAndZoom.SmoothAverageResetTransform();
        paintRotateAndZoom.enabled = true;
    });
    // Cập nhật lại trạng thái của button guide
    _guideSequence.AppendCallback(() =>
    {
        if (PlatformManager.Instance.IsStandalone || PlatformManager.Instance.IsWebGL)
            guideButton.GetComponent<UIButtonHoverSprite>().SetSelected(false);
        if (PlatformManager.Instance.IsMobile || PlatformManager.Instance.IsCloud)
            guideButton_mobile.GetComponent<UIButtonHoverSprite>().SetSelected(false);
        if (PlatformManager.Instance.IsVR)
            guideButton_vr.GetComponent<UIButtonHoverSprite>().SetSelected(false);
        if (PlatformManager.Instance.IsTomko)
            guideButton_tomko.GetComponent<UIButtonHoverSprite>().SetSelected(false);
    });
    _guideSequence.SetAutoKill(true);
}


    public void RefreshPaintingClicked()
    {
        SetDefaultAll();
        //paintRotateAndZoom.SmoothAverageResetTransform();
        //paintRotateAndZoom.enabled = true;
        SetDefaultZoom();
        TurnOffCautionTextAndImage();
        PaintingDetailManager.Instance.ResetView();
        
    }
    
    private IEnumerator StartAICautionAnimation()
    {
        // Lấy đối tượng UI dựa trên nền tảng
        TextMeshProUGUI cautionText = null;
        Image cautionIcon = null;

        if (PlatformManager.Instance.IsStandalone || PlatformManager.Instance.IsWebGL)
        {
            cautionText = AIAnimationCautionText;
            cautionIcon = AIAnimationIcon;
        }
        else if (PlatformManager.Instance.IsMobile || PlatformManager.Instance.IsCloud)
        {
            cautionText = AIAnimationCautionText_mobile;
            cautionIcon = AIAnimationIcon_mobile;
        }
        else if (PlatformManager.Instance.IsVR)
        {
            cautionText = AIAnimationCautionText_vr;
            cautionIcon = AIAnimationIcon_vr;
        }
        else if (PlatformManager.Instance.IsTomko)
        {
            cautionText = AIAnimationCautionText_tomko;
            cautionIcon = AIAnimationIcon_tomko;
        }

        // Đảm bảo các đối tượng được kích hoạt và đặt alpha ban đầu là 0
        if (cautionText != null)
        {
            cautionText.gameObject.SetActive(true);
            Color textColor = cautionText.color;
            textColor.a = 0f;
            cautionText.color = textColor;
        }
        if (cautionIcon != null)
        {
            cautionIcon.gameObject.SetActive(true);
            Color iconColor = cautionIcon.color;
            iconColor.a = 0f;
            cautionIcon.color = iconColor;
        }

        int repeatCount = 3;
        for (int i = 0; i < repeatCount; i++)
        {
            // Fade in trong 1 giây
            if (cautionText != null)
                cautionText.DOFade(1f, 1f);
            if (cautionIcon != null)
                cautionIcon.DOFade(1f, 1f);
            yield return new WaitForSeconds(1f);

            // Giữ nguyên trong 5 giây
            yield return new WaitForSeconds(5f);

            // Fade out trong 1 giây
            if (cautionText != null)
                cautionText.DOFade(0f, 1f);
            if (cautionIcon != null)
                cautionIcon.DOFade(0f, 1f);
            yield return new WaitForSeconds(1f);

            // Nếu chưa phải lần lặp cuối, đợi 3 giây trước khi lặp lại
            if (i < repeatCount - 1)
            {
                yield return new WaitForSeconds(3f);
            }
        }
        // Sau 3 lần, đảm bảo các đối tượng được ẩn
        if (cautionText != null)
            cautionText.DOFade(0f, 0.5f);
        if (cautionIcon != null)
            cautionIcon.DOFade(0f, 0.5f);
        yield break;
    }
    
    public void ZoomPaintingClicked()
    {
        isGuide = false;
        isAI = false;
        
        TurnOffCautionTextAndImage();
        
        if(PlatformManager.Instance.IsStandalone || PlatformManager.Instance.IsWebGL)
        {
            guideButton.GetComponent<UIButtonHoverSprite>().SetSelected(isGuide);
            aiButton.GetComponent<AIHoverEffect>().SetDefaultSprite();
        }
        
        if(PlatformManager.Instance.IsMobile || PlatformManager.Instance.IsCloud)
        {
            guideButton_mobile.GetComponent<UIButtonHoverSprite>().SetSelected(isGuide);
            aiButton_mobile.GetComponent<AIHoverEffect>().SetDefaultSprite();
        }
        
        if(PlatformManager.Instance.IsVR)
        {
            guideButton_vr.GetComponent<UIButtonHoverSprite>().SetSelected(isGuide);
            aiButton_vr.GetComponent<AIHoverEffect>().SetDefaultSprite();
        }
        
        if(PlatformManager.Instance.IsTomko)
        {
            guideButton_tomko.GetComponent<UIButtonHoverSprite>().SetSelected(isGuide);
            aiButton_tomko.GetComponent<AIHoverEffect>().SetDefaultSprite();
        }

        BlinkCanvas.SetActive(false);
        VideoPlayer.Stop();
        VideoPlayer.gameObject.SetActive(false);
        
        tranh.GetComponent<Renderer>().material.mainTexture = tranhDefaultSprite;
        // Gán texture phát sáng
        tranh.GetComponent<Renderer>().material.SetTexture("_EmissionMap", tranhDefaultSprite);
        
        if (_blinkCoroutine != null)
        {
            StopCoroutine(_blinkCoroutine);
            _blinkCoroutine = null;
        }
        
        StopGuideSequence();
        SetGuideImageOff();
        
        if (isZoom)
        {
            PaintingDetailManager.Instance.SetZoomPainting(false);
            UIPaintingManager.Instance.DisableUIPainting(PaintID);
            magnifierHover.enabled = false;
            
            isZoom = false;
            paintRotateAndZoom.canRotate = true;
            if(PlatformManager.Instance.IsStandalone || PlatformManager.Instance.IsWebGL)
            {
                zoomButton.GetComponent<UIButtonHoverSprite>().SetSelected(isZoom);
            }
            
            if(PlatformManager.Instance.IsMobile || PlatformManager.Instance.IsCloud)
            {
                zoomButton_mobile.GetComponent<UIButtonHoverSprite>().SetSelected(isZoom);
            }
            
            if(PlatformManager.Instance.IsVR)
            {
                zoomButton_vr.GetComponent<UIButtonHoverSprite>().SetSelected(isZoom);

            	MagnifierCanvas.gameObject.SetActive(false);
            }
            
            if(PlatformManager.Instance.IsTomko)
            {
                zoomButton_tomko.GetComponent<UIButtonHoverSprite>().SetSelected(isZoom);
            }

        }
        else
        {
            PaintingDetailManager.Instance.SetZoomPainting(true);
            UIPaintingManager.Instance.EnableUIPainting(PaintID);
            magnifierHover.enabled = true;
            isZoom = true;
            paintRotateAndZoom.SmoothAverageResetTransform();
            paintRotateAndZoom.canRotate = false;


             if(PlatformManager.Instance.IsVR)
            {
                zoomButton_vr.GetComponent<UIButtonHoverSprite>().SetSelected(isZoom);
            	MagnifierCanvas.gameObject.SetActive(true);
            }
            
            if(PlatformManager.Instance.IsStandalone || PlatformManager.Instance.IsWebGL)
            {
                zoomButton.GetComponent<UIButtonHoverSprite>().SetSelected(isZoom);
            }
            
            if(PlatformManager.Instance.IsMobile || PlatformManager.Instance.IsCloud)
            {
                zoomButton_mobile.GetComponent<UIButtonHoverSprite>().SetSelected(isZoom);
            }
            
            if(PlatformManager.Instance.IsTomko)
            {
                zoomButton_tomko.GetComponent<UIButtonHoverSprite>().SetSelected(isZoom);
            }
        }
    }

    public void TurnOffCautionTextAndImage()
    {
        if (_aiCautionCoroutine != null)
        {
            StopCoroutine(_aiCautionCoroutine);
            
            _aiCautionCoroutine = null;
        }
        
        if(AIAnimationCautionText) AIAnimationCautionText.gameObject.SetActive(false);
        if(AIAnimationCautionText_mobile) AIAnimationCautionText_mobile.gameObject.SetActive(false);
        if(AIAnimationCautionText_vr) AIAnimationCautionText_vr.gameObject.SetActive(false);
        if(AIAnimationCautionText_tomko) AIAnimationCautionText_tomko.gameObject.SetActive(false);
                
        if(AIAnimationIcon) AIAnimationIcon.gameObject.SetActive(false);
        if(AIAnimationIcon_mobile) AIAnimationIcon_mobile.gameObject.SetActive(false);
        if(AIAnimationIcon_vr) AIAnimationIcon_vr.gameObject.SetActive(false);
        if(AIAnimationIcon_tomko) AIAnimationIcon_tomko.gameObject.SetActive(false);
    }
    
    public void AIPaintingClicked()
    {
        isGuide = false;
        isZoom = false;

        paintRotateAndZoom.canRotate = true;
        
        if (PlatformManager.Instance.IsStandalone || PlatformManager.Instance.IsWebGL)
        {
            guideButton.GetComponent<UIButtonHoverSprite>().SetSelected(isGuide);
            zoomButton.GetComponent<UIButtonHoverSprite>().SetSelected(isZoom);
        }
        if (PlatformManager.Instance.IsMobile || PlatformManager.Instance.IsCloud)
        {
            guideButton_mobile.GetComponent<UIButtonHoverSprite>().SetSelected(isGuide);
            zoomButton_mobile.GetComponent<UIButtonHoverSprite>().SetSelected(isZoom);
        }
        if (PlatformManager.Instance.IsVR)
        {
            guideButton_vr.GetComponent<UIButtonHoverSprite>().SetSelected(isGuide);
            zoomButton_vr.GetComponent<UIButtonHoverSprite>().SetSelected(isZoom);
        }
        if (PlatformManager.Instance.IsTomko)
        {
            guideButton_tomko.GetComponent<UIButtonHoverSprite>().SetSelected(isGuide);
            zoomButton_tomko.GetComponent<UIButtonHoverSprite>().SetSelected(isZoom);
        }

        magnifierHover.enabled = false;
        PaintingDetailManager.Instance.SetZoomPainting(false);
        StopGuideSequence();
        SetGuideImageOff();

        if (isAI)
        {
            // Nếu AI đang bật thì tắt chức năng
            if (PlatformManager.Instance.IsStandalone || PlatformManager.Instance.IsWebGL)
            {
                aiButton.GetComponent<AIHoverEffect>().isSelected = false;
                aiButton.GetComponent<AIHoverEffect>().OnClickedButton();
            }
            if (PlatformManager.Instance.IsMobile || PlatformManager.Instance.IsCloud)
            {
                aiButton_mobile.GetComponent<AIHoverEffect>().isSelected = false;
                aiButton_mobile.GetComponent<AIHoverEffect>().OnClickedButton();
            }
            if (PlatformManager.Instance.IsVR)
            {
                aiButton_vr.GetComponent<AIHoverEffect>().isSelected = false;
                aiButton_vr.GetComponent<AIHoverEffect>().OnClickedButton();
            }
            if (PlatformManager.Instance.IsTomko)
            {
                aiButton_tomko.GetComponent<AIHoverEffect>().isSelected = false;
                aiButton_tomko.GetComponent<AIHoverEffect>().OnClickedButton();
            }

            isAI = false;
            paintRotateAndZoom.enabled = true;

            BlinkCanvas.SetActive(false);
            VideoPlayer.Stop();
            VideoPlayer.gameObject.SetActive(false);
            tranh.GetComponent<Renderer>().material.mainTexture = tranhDefaultSprite;
            tranh.GetComponent<Renderer>().material.SetTexture("_EmissionMap", tranhDefaultSprite);

            if (_blinkCoroutine != null)
            {
                StopCoroutine(_blinkCoroutine);
                _blinkCoroutine = null;
            }
            TurnOffCautionTextAndImage();
            
        }
        else
        {
            // Bật chế độ AI
            UIPaintingManager.Instance.EnableUIPainting(PaintID);
            if (PlatformManager.Instance.IsStandalone || PlatformManager.Instance.IsWebGL)
            {
                aiButton.GetComponent<AIHoverEffect>().isSelected = true;
                aiButton.GetComponent<AIHoverEffect>().OnClickedButton();
            }
            if (PlatformManager.Instance.IsMobile || PlatformManager.Instance.IsCloud)
            {
                aiButton_mobile.GetComponent<AIHoverEffect>().isSelected = true;
                aiButton_mobile.GetComponent<AIHoverEffect>().OnClickedButton();
            }
            if (PlatformManager.Instance.IsVR)
            {
                aiButton_vr.GetComponent<AIHoverEffect>().isSelected = true;
                aiButton_vr.GetComponent<AIHoverEffect>().OnClickedButton();
            }
            if (PlatformManager.Instance.IsTomko)
            {
                aiButton_tomko.GetComponent<AIHoverEffect>().isSelected = true;
                aiButton_tomko.GetComponent<AIHoverEffect>().OnClickedButton();
            }

            isAI = true;
            paintRotateAndZoom.SmoothAverageResetTransform();
            paintRotateAndZoom.enabled = true;

            

            if (_blinkCoroutine != null)
            {
                StopCoroutine(_blinkCoroutine);
            }
            _blinkCoroutine = StartCoroutine(StartBlinkAndPlayVideo());
        }
    }

    private IEnumerator StartBlinkAndPlayVideo()
    {
        
        BlinkCanvas.SetActive(true);
        VideoPlayer.gameObject.SetActive(false);

        if (VideoClips.Count <= 0) yield break;
        VideoPlayer.gameObject.SetActive(true);
        VideoPlayer.clip = VideoClips[UnityEngine.Random.Range(0, VideoClips.Count)];
        VideoPlayer.Prepare();
        yield return new WaitUntil(() => VideoPlayer.isPrepared);
        yield return new WaitForSeconds(3f);
        BlinkCanvas.SetActive(false);
        _aiCautionCoroutine = StartCoroutine(StartAICautionAnimation());
        VideoPlayer.Play();
        tranh.GetComponent<Renderer>().material.mainTexture = videoRenderTexture;
        tranh.GetComponent<Renderer>().material.SetTexture("_EmissionMap", videoRenderTexture);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
