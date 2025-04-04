using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Serialization;
using UnityEngine.Video;

public class UIPainting : UIBasic
{
    public Button guideButton;
    public Button zoomButton;
    public Button aiButton;
    
    public bool isGuide = false;
    public bool isZoom = false;
    public bool isAI = false;

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
    
    public MagnifierHover magnifierHover;
    
    public Vector3 guideRotateDefaultPosition;
    public Vector3 guideZoomDefaultPosition;

    public float durartion = 9f;
    
    public PaintRotateAndZoom paintRotateAndZoom;
    
    private Coroutine _guideRotateCoroutine;
    private Coroutine _blinkCoroutine;
    
    // Start is called before the first frame update
    void Start()
    {
        guideButton.onClick.AddListener(GuidePaintingClicked);
        zoomButton.onClick.AddListener(ZoomPaintingClicked);
        aiButton.onClick.AddListener(AIPaintingClicked);
        
        guideRotateDefaultPosition = guideRotateImage.transform.localPosition;
        guideZoomDefaultPosition = guideRotateImage.transform.localPosition;
    }

    private Sequence _guideSequence;

    public void SetDefaultAll()
    {
        isGuide = false;
        isZoom = false;
        isAI = false;
        
        guideButton.GetComponent<UIButtonHoverSprite>().SetSelected(isGuide);
        zoomButton.GetComponent<UIButtonHoverSprite>().SetSelected(isZoom);
        aiButton.GetComponent<AIHoverEffect>().SetDefaultSprite();
        
        StopGuideSequence();
        SetGuideImageOff();
        
        paintRotateAndZoom.SmoothOriginResetTransform();
        paintRotateAndZoom.enabled = false;
        
        BlinkCanvas.SetActive(false);
        VideoPlayer.Stop();
        VideoPlayer.gameObject.SetActive(false);
        tranh.GetComponent<Renderer>().material.mainTexture = tranhDefaultSprite;
        if (_blinkCoroutine != null)
        {
            StopCoroutine(_blinkCoroutine);
            _blinkCoroutine = null;
        }
    }
    
public void GuidePaintingClicked()
{
    isAI = false;
    isZoom = false;

    aiButton.GetComponent<AIHoverEffect>().SetDefaultSprite();
    zoomButton.GetComponent<UIButtonHoverSprite>().SetSelected(isZoom);
    magnifierHover.enabled = false;

    BlinkCanvas.SetActive(false);
    VideoPlayer.Stop();
    VideoPlayer.gameObject.SetActive(false);
    tranh.GetComponent<Renderer>().material.mainTexture = tranhDefaultSprite;
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
        //paintRotateAndZoom.enabled = true;
        guideButton.GetComponent<UIButtonHoverSprite>().SetSelected(isGuide);
    }
    else
    {
        //guideButton.image.sprite = guideSelectedSprite;
        StartGuideSequence();
        isGuide = true;
        paintRotateAndZoom.SmoothAverageResetTransform();
        //paintRotateAndZoom.enabled = false;
        guideButton.GetComponent<UIButtonHoverSprite>().SetSelected(isGuide);
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
    guideZoomImage.transform.localPosition = guideZoomDefaultPosition;
    guideRotateImage.transform.localPosition = guideRotateDefaultPosition;

    guideZoomImage.DOKill();
    guideRotateImage.DOKill();

    guideZoomImage.DOFade(1f, 0f);
    guideRotateImage.DOFade(1f, 0f);

    guideZoomImage.gameObject.SetActive(false);
    guideRotateImage.gameObject.SetActive(false);
}

private void StartGuideSequence()
{
    string currentLanguage = PlayerPrefs.GetString("Language", "vi");
    if (currentLanguage == "vi")
    {
        guideRotateImage.sprite = guideViSprite;
        guideZoomImage.sprite = zoomViSprite;
    }
    else if (currentLanguage == "en")
    {
        guideRotateImage.sprite = guideEnglishSprite;
        guideZoomImage.sprite = zoomEnglishSprite;
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
    guideRotateImage.transform.localPosition = rotateStartPos;
    guideZoomImage.transform.localPosition = zoomStartPos;

    guideRotateImage.DOFade(0f, 0f);
    guideZoomImage.DOFade(0f, 0f);

    guideRotateImage.gameObject.SetActive(true);
    guideZoomImage.gameObject.SetActive(false);

    _guideSequence = DOTween.Sequence();

    // Rotate Guide xuất hiện
    _guideSequence.Append(guideRotateImage.DOFade(1f, 0.5f));
    _guideSequence.Join(guideRotateImage.rectTransform.DOLocalMoveX(rotateEndPos.x, 0.5f));

    // Giữ một khoảng thời gian
    _guideSequence.AppendInterval(durartion);

    // Rotate Guide biến mất
    _guideSequence.Append(guideRotateImage.DOFade(0f, 0.5f));
    _guideSequence.Join(guideRotateImage.rectTransform.DOLocalMoveX(rotateOutPos.x, 0.5f));
    _guideSequence.AppendCallback(() => guideRotateImage.gameObject.SetActive(false));

    // Zoom Guide xuất hiện
    _guideSequence.AppendCallback(() =>
    {
        guideZoomImage.transform.localPosition = zoomStartPos;
        guideZoomImage.DOFade(0f, 0f);
        guideZoomImage.gameObject.SetActive(true);
    });
    _guideSequence.Append(guideZoomImage.DOFade(1f, 0.5f));
    _guideSequence.Join(guideZoomImage.rectTransform.DOLocalMoveX(zoomEndPos.x, 0.5f));

    // Giữ một khoảng thời gian
    _guideSequence.AppendInterval(durartion);

    // Zoom Guide biến mất
    _guideSequence.Append(guideZoomImage.DOFade(0f, 0.5f));
    _guideSequence.Join(guideZoomImage.rectTransform.DOLocalMoveX(zoomOutPos.x, 0.5f));
    _guideSequence.AppendCallback(() => guideZoomImage.gameObject.SetActive(false));

    // Kết thúc
    _guideSequence.AppendCallback(() => guideButton.image.sprite = guideButtonDefaultSprite);
    _guideSequence.AppendCallback(() => isGuide = false);
    _guideSequence.AppendCallback(() => paintRotateAndZoom.SmoothAverageResetTransform());
    _guideSequence.AppendCallback(() => paintRotateAndZoom.enabled = true);

    _guideSequence.SetAutoKill(true);
}
    
    public void ZoomPaintingClicked()
    {
        isGuide = false;
        isAI = false;
        
        aiButton.GetComponent<AIHoverEffect>().SetDefaultSprite();
        guideButton.GetComponent<UIButtonHoverSprite>().SetSelected(isGuide);

        BlinkCanvas.SetActive(false);
        VideoPlayer.Stop();
        VideoPlayer.gameObject.SetActive(false);
        tranh.GetComponent<Renderer>().material.mainTexture = tranhDefaultSprite;
        if (_blinkCoroutine != null)
        {
            StopCoroutine(_blinkCoroutine);
            _blinkCoroutine = null;
        }
        
        StopGuideSequence();
        SetGuideImageOff();
        
        if (isZoom)
        {
            magnifierHover.enabled = false;
            isZoom = false;
            //paintRotateAndZoom.enabled = true;
            MagnifierCanvas.gameObject.SetActive(false);
            zoomButton.GetComponent<UIButtonHoverSprite>().SetSelected(isZoom);
        }
        else
        {
            magnifierHover.enabled = true;
            isZoom = true;
            paintRotateAndZoom.SmoothAverageResetTransform();
            //paintRotateAndZoom.enabled = false;
            MagnifierCanvas.gameObject.SetActive(true);

            zoomButton.GetComponent<UIButtonHoverSprite>().SetSelected(isZoom);
        }
    }
    
    public void AIPaintingClicked()
    {
        isGuide = false;
        isZoom = false;

        guideButton.GetComponent<UIButtonHoverSprite>().SetSelected(isGuide);
        zoomButton.GetComponent<UIButtonHoverSprite>().SetSelected(isZoom);

        magnifierHover.enabled = false;

        StopGuideSequence();
        SetGuideImageOff();

        if (isAI)
        {
            aiButton.GetComponent<AIHoverEffect>().isSelected = false;
            aiButton.GetComponent<AIHoverEffect>().OnClickedButton();

            isAI = false;
            //paintRotateAndZoom.enabled = true;

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
        }
        else
        {
            aiButton.GetComponent<AIHoverEffect>().isSelected = true;
            aiButton.GetComponent<AIHoverEffect>().OnClickedButton();

            isAI = true;
            paintRotateAndZoom.SmoothAverageResetTransform();
            //paintRotateAndZoom.enabled = false;

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
