using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class UIPainting : UIBasic
{
    public Button guideButton;
    public Button zoomButton;
    public Button aiButton;
    
    public bool isGuide = false;
    public bool isZoom = false;
    public bool isAI = false;
    
    public Sprite guideDefaultSprite;
    public Sprite guideSelectedSprite;
    
    public Sprite zoomDefaultSprite;
    public Sprite zoomSelectedSprite;

    public Image guideRotateImage;
    public Image guideZoomImage;
    
    public MagnifierHover magnifierHover;
    
    public Vector3 guideRotateDefaultPosition;
    public Vector3 guideZoomDefaultPosition;

    public float durartion = 9f;
    
    private Coroutine _guideRotateCoroutine;
    
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

public void GuidePaintingClicked()
{
    isAI = false;
    isZoom = false;

    aiButton.GetComponent<AIHoverEffect>().SetDefaultSprite();
    zoomButton.image.sprite = zoomDefaultSprite;
    magnifierHover.enabled = false;

    if (isGuide)
    {
        guideButton.image.sprite = guideDefaultSprite;
        StopGuideSequence();
        SetGuideImageOff();
        isGuide = false;
    }
    else
    {
        guideButton.image.sprite = guideSelectedSprite;
        StartGuideSequence();
        isGuide = true;
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
    _guideSequence.AppendCallback(() => guideButton.image.sprite = guideDefaultSprite);
    _guideSequence.AppendCallback(() => isGuide = false);

    _guideSequence.SetAutoKill(true);
}
    
    public void ZoomPaintingClicked()
    {
        isGuide = false;
        isAI = false;
        
        aiButton.GetComponent<AIHoverEffect>().SetDefaultSprite();
        guideButton.image.sprite = guideDefaultSprite;

        
        
        StopGuideSequence();
        SetGuideImageOff();
        
        if (isZoom)
        {
            zoomButton.image.sprite = zoomDefaultSprite;
            magnifierHover.enabled = false;
            isZoom = false;
        }
        else
        {
            magnifierHover.enabled = true;
            zoomButton.image.sprite = zoomSelectedSprite;
            isZoom = true;
        }
    }
    
    public void AIPaintingClicked()
    {
        isGuide = false;
        isZoom = false;
        
        guideButton.image.sprite = guideDefaultSprite;
        zoomButton.image.sprite = zoomDefaultSprite;
        
        magnifierHover.enabled = false;
        
        StopGuideSequence();
        SetGuideImageOff();
        
        if (isAI)
        {
            aiButton.GetComponent<AIHoverEffect>().isSelected = false;
            aiButton.GetComponent<AIHoverEffect>().OnClickedButton();
            
            isAI = false;
        }
        else
        {
            
            aiButton.GetComponent<AIHoverEffect>().isSelected = true;
            aiButton.GetComponent<AIHoverEffect>().OnClickedButton();
            isAI = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
