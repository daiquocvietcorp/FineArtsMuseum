using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class IdleScreenManager : MonoBehaviour
{
    public float freeTime = 30f;
    private float idleTimer = 0f;

    public GameObject idleScreen;

    [Header("Tranh 1")]
    public CanvasGroup group1;
    public Image image1;
    public TextMeshProUGUI artworkNameText1;
    public TextMeshProUGUI authorNameText1;
    public Sprite artworkSprite1;
    public string artworkName1;
    public string authorName1;

    [Header("Tranh 2")]
    public CanvasGroup group2;
    public Image image2;
    public TextMeshProUGUI artworkNameText2;
    public TextMeshProUGUI authorNameText2;
    public Sprite artworkSprite2;
    public string artworkName2;
    public string authorName2;

    [Header("Button")]
    public Button nextButton;
    
    
    public float fadeDuration = 0.5f;

    private bool isShowingSecond = false;
    
    private bool isAnimating = false;
    
    public Button exitButton;
    public Animator idleScreenAnimator;
    
    public StartController startController;
    
    [Header("Fade Out Settings")]
    public float fadeOutDuration = 0.5f;
    public Vector3 targetScale = new Vector3(1.2f, 1.2f, 1f);
    
    [Header("Background")]
    public Image idleScreenBackground;

    void Start()
    {
        // if (PlatformManager.Instance.IsTomko || PlatformManager.Instance.IsStandalone)
        // {
        //     //Set RectTransform PosX of TextMeshProUGUI artworkNameText2 to value
        //     RectTransform rectTransform = artworkNameText2.GetComponent<RectTransform>();
        //     RectTransform rectTransform2 = authorNameText2.GetComponent<RectTransform>();
        //     Vector2 newPos = rectTransform.anchoredPosition;
        //     newPos.x = -390.9f; // Set to desired value
        //     rectTransform.anchoredPosition = newPos;
        //     rectTransform2.anchoredPosition = newPos;
        // }
        //
        // if (PlatformManager.Instance.IsCloud || PlatformManager.Instance.IsMobile)
        // {
        //     //Set RectTransform PosX of TextMeshProUGUI artworkNameText2 to value
        //     RectTransform rectTransform = artworkNameText2.GetComponent<RectTransform>();
        //     RectTransform rectTransform2 = authorNameText2.GetComponent<RectTransform>();
        //     Vector2 newPos = rectTransform.anchoredPosition;
        //     newPos.x = -390.9f; // Set to desired value
        //     rectTransform.anchoredPosition = newPos;
        //     rectTransform2.anchoredPosition = newPos;
        // }
        
        if (!PlatformManager.Instance.IsTomko)
        {
            nextButton.onClick.AddListener(OnNextButtonClicked);
            exitButton.onClick.RemoveAllListeners();
            exitButton.onClick.AddListener(OnExitButtonClicked);
            HideIdleScreen();
            return;
        }
        
        if (SceneLog.IsFirstScene)
        {
            idleScreen.SetActive(true);
            nextButton.onClick.AddListener(OnNextButtonClicked);
            exitButton.onClick.AddListener(StartApplication);
            if(PlatformManager.Instance.IsTomko) return;
            exitButton.onClick.RemoveAllListeners();
            exitButton.onClick.AddListener(OnExitButtonClicked);
            HideIdleScreen();
        }
        else
        {
            startController.StartApplication();
            idleScreen.SetActive(true);
            nextButton.onClick.AddListener(OnNextButtonClicked);
            exitButton.onClick.RemoveAllListeners();
            exitButton.onClick.AddListener(OnExitButtonClicked);
            HideIdleScreen();
        }
    }

    private void StartApplication()
    {
        startController.StartApplication();
        HideIdleScreen();
        exitButton.onClick.RemoveAllListeners();
        exitButton.onClick.AddListener(OnExitButtonClicked);
        
    }

    void OnExitButtonClicked()
    {
        if (idleScreenAnimator != null)
        {
            idleScreenAnimator.SetBool("Fade", true);
        }
    }
    
    public void OnFadeOutComplete()
    {
        Debug.Log("Fade out animation finished. Start fading content + scale.");

        // Xác định group đang hiển thị
        CanvasGroup currentGroup = isShowingSecond ? group2 : group1;
        StartCoroutine(FadeAndScaleOut(currentGroup));
    }
    
    IEnumerator FadeAndScaleOut(CanvasGroup activeGroup)
    {
        float timer = 0f;
        Vector3 originalScale = idleScreen.transform.localScale;
        Color bgStartColor = idleScreenBackground.color;

        // Reset nếu cần
        idleScreen.transform.localScale = Vector3.one;
        activeGroup.alpha = 1f;

        while (timer < fadeOutDuration)
        {
            float t = timer / fadeOutDuration;

            activeGroup.alpha = Mathf.Lerp(1f, 0f, t);
            idleScreen.transform.localScale = Vector3.Lerp(Vector3.one, targetScale, t);

            // Giảm alpha của background Image
            Color fadedColor = bgStartColor;
            fadedColor.a = Mathf.Lerp(1f, 0f, t);
            idleScreenBackground.color = fadedColor;

            timer += Time.deltaTime;
            yield return null;
        }

        // Reset lại alpha background
        idleScreenBackground.color = bgStartColor;

        // Reset lại các giá trị
        activeGroup.alpha = 1f;
        idleScreen.transform.localScale = Vector3.one;

        if (idleScreenAnimator != null)
            idleScreenAnimator.SetBool("Fade", false);

        HideIdleScreen();
    }

    void Update()
    {
        if(PlatformManager.Instance.IsMobile || PlatformManager.Instance.IsCloud || PlatformManager.Instance.IsWebGL) return;
        if (!idleScreen.activeSelf)
        {
            if (Input.anyKey || Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
            {
                idleTimer = 0f; // Reset timer nếu chưa bật idle
            }
            else
            {
                idleTimer += Time.deltaTime;

                if (idleTimer >= freeTime)
                {
                    ShowIdleScreen(); // Chỉ show 1 lần, không bị tắt nữa
                }
            }
        }
    }

    void ShowIdleScreen()
    {
        idleScreen.SetActive(true);
        idleScreen.transform.localScale = Vector3.one;
        isShowingSecond = false;
        SetGroupActive(group1, true);
        SetGroupActive(group2, false);

        image1.sprite = artworkSprite1;
        artworkNameText1.text = artworkName1;
        authorNameText1.text = authorName1;

        image2.sprite = artworkSprite2;
        artworkNameText2.text = artworkName2;
        authorNameText2.text = authorName2;

        group1.alpha = 1;
        group2.alpha = 0;
    }

    void HideIdleScreen()
    {
        idleScreen.SetActive(false);
        idleTimer = 0f;
    }

    void OnNextButtonClicked()
    {
        if (isAnimating) return;

        if (isShowingSecond)
        {
            StartCoroutine(FadeGroups(group2, group1));
            isShowingSecond = false;
        }
        else
        {
            StartCoroutine(FadeGroups(group1, group2));
            isShowingSecond = true;
        }
    }

    IEnumerator FadeGroups(CanvasGroup from, CanvasGroup to)
    {
        float timer = 0f;
        isAnimating = true;

        SetGroupActive(to, true);

        while (timer < fadeDuration)
        {
            float t = timer / fadeDuration;
            from.alpha = Mathf.Lerp(1, 0, t);
            to.alpha = Mathf.Lerp(0, 1, t);
            timer += Time.deltaTime;
            yield return null;
        }

        from.alpha = 0;
        to.alpha = 1;

        SetGroupActive(from, false);
        isAnimating = false;
    }

    void SetGroupActive(CanvasGroup group, bool active)
    {
        group.interactable = active;
        group.blocksRaycasts = active;
        group.gameObject.SetActive(active);
    }
}