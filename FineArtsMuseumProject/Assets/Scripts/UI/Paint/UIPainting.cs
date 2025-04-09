using System;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using Trigger;
using UnityEngine.Serialization;
using UnityEngine.Video;

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
    
    public Sprite guideViSprite;
    public Sprite guideEnglishSprite;
    public Sprite zoomViSprite;
    public Sprite zoomEnglishSprite;

    public Image guideRotateImage;
    public Image guideZoomImage;
    
    public Image guideRotateImage_mobile;
    public Image guideZoomImage_mobile;
    
    public Image guideRotateImage_vr;
    public Image guideZoomImage_vr;
    
    public Image guideRotateImage_tomko;
    public Image guideZoomImage_tomko;
    
    public MagnifierHover magnifierHover;
    
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
        }
        
        if (PlatformManager.Instance.IsMobile || PlatformManager.Instance.IsCloud)
        {
            guideButton_mobile.onClick.AddListener(GuidePaintingClicked);
            zoomButton_mobile.onClick.AddListener(ZoomPaintingClicked);
            aiButton_mobile.onClick.AddListener(AIPaintingClicked);
            refreshButton_mobile.onClick.AddListener(RefreshPaintingClicked);
            
            guideRotateDefaultPosition = guideRotateImage_mobile.transform.localPosition;
            guideZoomDefaultPosition = guideZoomImage_mobile.transform.localPosition;
        }
        if (PlatformManager.Instance.IsVR)
        {
            guideButton_vr.onClick.AddListener(GuidePaintingClicked);
            zoomButton_vr.onClick.AddListener(ZoomPaintingClicked);
            aiButton_vr.onClick.AddListener(AIPaintingClicked);
            refreshButton_vr.onClick.AddListener(RefreshPaintingClicked);
            
            guideRotateDefaultPosition = guideRotateImage_vr.transform.localPosition;
            guideZoomDefaultPosition = guideZoomImage_vr.transform.localPosition;
        }
        if (PlatformManager.Instance.IsTomko)
        {
            guideButton_tomko.onClick.AddListener(GuidePaintingClicked);
            zoomButton_tomko.onClick.AddListener(ZoomPaintingClicked);
            aiButton_tomko.onClick.AddListener(AIPaintingClicked);
            refreshButton_tomko.onClick.AddListener(RefreshPaintingClicked);
            
            guideRotateDefaultPosition = guideRotateImage_tomko.transform.localPosition;
            guideZoomDefaultPosition = guideZoomImage_tomko.transform.localPosition;
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
    
    if(PlatformManager.Instance.IsStandalone || PlatformManager.Instance.IsWebGL)
    {
        guideZoom = guideZoomImage;
        guideRotate = guideRotateImage;
    }
        
    if(PlatformManager.Instance.IsMobile || PlatformManager.Instance.IsCloud)
    {
        guideZoom = guideZoomImage_mobile;
        guideRotate = guideRotateImage_mobile;
    }
    if(PlatformManager.Instance.IsVR)
    {
        guideZoom = guideZoomImage_vr;
        guideRotate = guideRotateImage_vr;
    }

    if (PlatformManager.Instance.IsTomko)
    {
        guideZoom = guideZoomImage_tomko;
        guideRotate = guideRotateImage_tomko;
    }
    
    guideZoom.transform.localPosition = guideZoomDefaultPosition;
    guideRotate.transform.localPosition = guideRotateDefaultPosition;

    guideZoom.DOKill();
    guideRotate.DOKill();

    guideZoom.DOFade(1f, 0f);
    guideRotate.DOFade(1f, 0f);

    guideZoom.gameObject.SetActive(false);
    guideRotate.gameObject.SetActive(false);
}

private void StartGuideSequence()
{
    isGuide = true; // Bật flag ngay khi bắt đầu sequence
    
    string currentLanguage = PlayerPrefs.GetString("Language", "vi");
    if (currentLanguage == "vi")
    {
        guideRotateImage.sprite = guideViSprite;
        guideZoomImage.sprite = zoomViSprite;
        
        guideRotateImage_mobile.sprite = guideViSprite;
        guideZoomImage_mobile.sprite = zoomViSprite;
        
        // guideRotateImage_vr.sprite = guideViSprite;
        // guideZoomImage_vr.sprite = zoomViSprite;
        
        guideRotateImage_tomko.sprite = guideViSprite;
        guideZoomImage_tomko.sprite = zoomViSprite;
    }
    else if (currentLanguage == "en")
    {
        guideRotateImage.sprite = guideEnglishSprite;
        guideZoomImage.sprite = zoomEnglishSprite;
        
        guideRotateImage_mobile.sprite = guideViSprite;
        guideZoomImage_mobile.sprite = zoomViSprite;
        
        // guideRotateImage_vr.sprite = guideViSprite;
        // guideZoomImage_vr.sprite = zoomViSprite;
        
        guideRotateImage_tomko.sprite = guideViSprite;
        guideZoomImage_tomko.sprite = zoomViSprite;
    }
    
    // Tính toán vị trí cho hiệu ứng trục X
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

    // Reset trạng thái ban đầu
    Image guideZoom = null;
    Image guideRotate = null;
    
    if(PlatformManager.Instance.IsStandalone || PlatformManager.Instance.IsWebGL)
    {
        guideZoom = guideZoomImage;
        guideRotate = guideRotateImage;
    }
        
    if(PlatformManager.Instance.IsMobile || PlatformManager.Instance.IsCloud)
    {
        guideZoom = guideZoomImage_mobile;
        guideRotate = guideRotateImage_mobile;
    }
    if(PlatformManager.Instance.IsVR)
    {
        guideZoom = guideZoomImage_vr;
        guideRotate = guideRotateImage_vr;
    }

    if (PlatformManager.Instance.IsTomko)
    {
        guideZoom = guideZoomImage_tomko;
        guideRotate = guideRotateImage_tomko;
    }
    
    guideZoom.transform.localPosition = guideZoomDefaultPosition;
    guideRotate.transform.localPosition = guideRotateDefaultPosition;

    guideZoom.DOFade(0f, 0f);
    guideRotate.DOFade(0f, 0f);

    guideZoom.gameObject.SetActive(false);
    guideRotate.gameObject.SetActive(true);

    _guideSequence = DOTween.Sequence();

    // Rotate Guide xuất hiện
    _guideSequence.Append(guideRotate.DOFade(1f, 1f));
    _guideSequence.Join(guideRotate.rectTransform.DOLocalMoveX(rotateEndPos.x, 1f));

    // Giữ một khoảng thời gian
    _guideSequence.AppendInterval(durartion);

    // Rotate Guide biến mất
    _guideSequence.Append(guideRotate.DOFade(0f, 1f));
    _guideSequence.Join(guideRotate.rectTransform.DOLocalMoveX(rotateOutPos.x, 1f));
    _guideSequence.AppendCallback(() => guideRotate.gameObject.SetActive(false));

    // Zoom Guide xuất hiện
    _guideSequence.AppendCallback(() =>
    {
        guideZoom.transform.localPosition = zoomStartPos;
        guideZoom.DOFade(0f, 0f);
        guideZoom.gameObject.SetActive(true);
    });
    _guideSequence.Append(guideZoom.DOFade(1f, 1f));
    _guideSequence.Join(guideZoom.rectTransform.DOLocalMoveX(zoomEndPos.x, 1f));

    // Giữ một khoảng thời gian
    _guideSequence.AppendInterval(durartion);

    // Zoom Guide biến mất
    _guideSequence.Append(guideZoom.DOFade(0f, 1f));
    _guideSequence.Join(guideZoom.rectTransform.DOLocalMoveX(zoomOutPos.x, 1f));
    _guideSequence.AppendCallback(() => guideZoom.gameObject.SetActive(false));

    // Kết thúc
    _guideSequence.AppendCallback(() => isGuide = false);
    _guideSequence.AppendCallback(() => paintRotateAndZoom.SmoothAverageResetTransform());
    _guideSequence.AppendCallback(() => paintRotateAndZoom.enabled = true);

// Di chuyển update UI vào đây
    _guideSequence.AppendCallback(() =>
    {
        if (PlatformManager.Instance.IsStandalone || PlatformManager.Instance.IsWebGL)
        {
            guideButton.GetComponent<UIButtonHoverSprite>().SetSelected(false);
        }
    
        if (PlatformManager.Instance.IsMobile || PlatformManager.Instance.IsCloud)
        {
            guideButton_mobile.GetComponent<UIButtonHoverSprite>().SetSelected(false);
        }
    
        if (PlatformManager.Instance.IsVR)
        {
            guideButton_vr.GetComponent<UIButtonHoverSprite>().SetSelected(false);
        }
    
        if (PlatformManager.Instance.IsTomko)
        {
            guideButton_tomko.GetComponent<UIButtonHoverSprite>().SetSelected(false);
        }
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
        
        if(PlatformManager.Instance.IsVR || PlatformManager.Instance.IsWebGL)
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

        yield return new WaitForSeconds(3f);
        tranh.GetComponent<Renderer>().material.mainTexture = videoRenderTexture;
        tranh.GetComponent<Renderer>().material.SetTexture("_EmissionMap", videoRenderTexture);
        yield return new WaitForSeconds(3f);
        // Khởi chạy hiệu ứng cảnh báo của AI dựa trên nền tảng (fade in, hold, fade out, delay) lặp lại 3 lần
        _aiCautionCoroutine = StartCoroutine(StartAICautionAnimation());
        // tranh.GetComponent<Renderer>().material.mainTexture = videoRenderTexture;
        // tranh.GetComponent<Renderer>().material.SetTexture("_EmissionMap", videoRenderTexture);
        BlinkCanvas.SetActive(false);

        if (VideoClips.Count > 0)
        {
            VideoPlayer.clip = VideoClips[UnityEngine.Random.Range(0, VideoClips.Count)];
            VideoPlayer.gameObject.SetActive(true);
            VideoPlayer.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
