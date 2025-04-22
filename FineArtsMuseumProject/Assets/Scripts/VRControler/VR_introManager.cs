using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class VR_introManager : MonoBehaviour
{
    [Header("Canvas")]  
    [SerializeField]
    private GameObject introCanvas;   // Canvas giới thiệu, sẽ tắt sau khi Intro hoàn thành
    [SerializeField]
    private GameObject startCanvas;   // Canvas chính sẽ hiển thị sau khi Intro xong

    [Header("Sequence Objects & Audio")]
    public GameObject[] sequenceObjects;          // Chuỗi các GameObject cần hiển thị tuần tự
    public AudioClip[] sequenceAudiosEnglish;     // Audio tương ứng cho chuỗi khi ngôn ngữ English
    public AudioClip[] sequenceAudiosVietnamese;  // Audio tương ứng cho chuỗi khi ngôn ngữ Vietnamese

    [Header("Intro Audio")]
    [SerializeField]
    private AudioClip introAudioVI;  // Audio giới thiệu tiếng Việt
    [SerializeField]
    private AudioClip introAudioEN;  // Audio giới thiệu tiếng Anh
    
    [Header("Outro Audio")]
    [SerializeField]
    private AudioClip outtroAudioVI;  // Audio kết thúc tiếng Việt
    [SerializeField]
    private AudioClip outtroAudioEN;  // Audio kết thúc tiếng Anh

    [Header("VR Caution Settings")]
    [SerializeField]
    private CanvasGroup cautionCanvasGroup;  // CanvasGroup hiển thị cảnh báo VR
    [SerializeField]
    private AudioClip cautionAudioVI;  // Audio cảnh báo tiếng Việt
    [SerializeField]
    private AudioClip cautionAudioEN;  // Audio cảnh báo tiếng Anh
    [SerializeField]
    private int cautionRepeatCount = 3;      // Số lần fade cảnh báo
    [SerializeField]
    private float cautionShowDuration = 1f;  // Thời gian hiển thị mỗi lần fade (giây)

    [Header("Audio Fade & Delay")]
    [SerializeField]
    private float fadeInDuration = 1f;   // Thời gian fade in audio (giây)
    [SerializeField]
    private float fadeOutDuration = 1f;  // Thời gian fade out audio (giây)
    [SerializeField]
    private float clipDelay = 0.5f;      // Delay sau mỗi clip (giây)

    [Header("UI Fade Settings")]
    [SerializeField]
    private float uiFadeDuration = 1f;   // Thời gian fade UI (giây)

    [Header("Controls & AudioSources")]
    [SerializeField]
    private Button skipButton;           // Nút bỏ qua sequence giới thiệu
    [SerializeField]
    private AudioSource ambientAudioSource; // AudioSource phát nhạc nền (ambient)

    private AudioSource sequenceAudioSource;  // AudioSource động để phát audio sequence
    private CanvasGroup introCanvasGroup;     // CanvasGroup của introCanvas để fade UI
    private CanvasGroup startCanvasGroup;     // CanvasGroup của startCanvas để fade UI

    private void Start()
    {
        // Chỉ chạy trên VR
        if (!PlatformManager.Instance.IsVR)
            return;

        // Đăng ký sự kiện SkipSequence
        skipButton.onClick.AddListener(SkipSequence);

        // Tạo AudioSource phụ cho sequence audio
        sequenceAudioSource = gameObject.AddComponent<AudioSource>();

        // Cấu hình CanvasGroup cho introCanvas
        if (introCanvas != null)
        {
            introCanvasGroup = introCanvas.GetComponent<CanvasGroup>() 
                ?? introCanvas.AddComponent<CanvasGroup>();
            introCanvasGroup.alpha = 1f;
        }

        // Cấu hình CanvasGroup cho startCanvas, khởi đầu ẩn
        if (startCanvas != null)
        {
            startCanvasGroup = startCanvas.GetComponent<CanvasGroup>() 
                ?? startCanvas.AddComponent<CanvasGroup>();
            startCanvasGroup.alpha = 0f;
            startCanvas.SetActive(false);
        }

        // Cấu hình Caution VR CanvasGroup, khởi đầu ẩn và kích hoạt object
        if (cautionCanvasGroup != null)
        {
            cautionCanvasGroup.alpha = 0f;
            cautionCanvasGroup.gameObject.SetActive(true);
        }

        // Bắt đầu trình tự cảnh báo VR rồi tới intro + sequence
        StartCoroutine(ShowCautionThenSequence());
    }

    /// <summary>
    /// Hiển thị cảnh báo VR với fade in/out lặp nhiều lần, phát audio một lần, sau đó chạy logic giới thiệu và sequence.
    /// </summary>
    IEnumerator ShowCautionThenSequence()
    {
        string currentLanguage = SceneLog.IsVietnamese ? "vi" : "en";
        AudioClip cautionClip = (currentLanguage == "vi") ? cautionAudioVI : cautionAudioEN;

        if (cautionCanvasGroup != null)
        {
            for (int i = 0; i < cautionRepeatCount; i++)
            {
                if (i == 0 && cautionClip != null)
                {
                    sequenceAudioSource.clip = cautionClip;
                    sequenceAudioSource.Play();
                }
                yield return StartCoroutine(FadeCanvasGroup(cautionCanvasGroup, 0f, 1f, uiFadeDuration));
                yield return new WaitForSeconds(cautionShowDuration);
                yield return StartCoroutine(FadeCanvasGroup(cautionCanvasGroup, 1f, 0f, uiFadeDuration));
                yield return new WaitForSeconds(cautionShowDuration);
            }

            if (cautionClip != null && cautionClip.length > cautionRepeatCount * (uiFadeDuration * 2 + cautionShowDuration * 2))
            {
                yield return new WaitForSeconds(cautionClip.length - cautionRepeatCount * (uiFadeDuration * 2 + cautionShowDuration * 2));
            }

            yield return new WaitForSeconds(1f);
            cautionCanvasGroup.alpha = 0f;
            cautionCanvasGroup.gameObject.SetActive(false);
        }

        yield return StartCoroutine(PlayIntroAndSequence());
    }

    /// <summary>
    /// Phát audio giới thiệu, chuỗi GameObject với audio tương ứng, rồi outro audio.
    /// </summary>
    IEnumerator PlayIntroAndSequence()
    {
        string currentLanguage = SceneLog.IsVietnamese ? "vi" : "en";
        AudioClip introClip = (currentLanguage == "vi") ? introAudioVI : introAudioEN;

        if (introClip != null)
        {
            yield return StartCoroutine(
                PlayClipWithFade(sequenceAudioSource, introClip, fadeInDuration, fadeOutDuration)
            );
            yield return new WaitForSeconds(clipDelay);
        }
        else
        {
            Debug.LogWarning("Thiếu audio giới thiệu cho ngôn ngữ: " + currentLanguage);
        }

        AudioClip[] selectedSequenceAudios =
            (currentLanguage == "vi") ? sequenceAudiosVietnamese : sequenceAudiosEnglish;
        int count = Mathf.Min(sequenceObjects.Length, selectedSequenceAudios.Length);

        for (int i = 0; i < count; i++)
        {
            var obj = sequenceObjects[i];
            var clip = selectedSequenceAudios[i];
            if (obj != null && clip != null)
            {
                obj.SetActive(true);
                yield return StartCoroutine(
                    PlayClipWithFade(sequenceAudioSource, clip, fadeInDuration, fadeOutDuration)
                );
                obj.SetActive(false);
                yield return new WaitForSeconds(clipDelay);
            }
            else
            {
                Debug.LogWarning("Thiếu object hoặc audio tại index " + i);
            }
        }

        // Phát outro audio sau khi sequence hoàn tất
        AudioClip outroClip = (currentLanguage == "vi") ? outtroAudioVI : outtroAudioEN;
        if (outroClip != null)
        {
            yield return StartCoroutine(
                PlayClipWithFade(sequenceAudioSource, outroClip, fadeInDuration, fadeOutDuration)
            );
        }
        else
        {
            Debug.LogWarning("Thiếu outro audio cho ngôn ngữ: " + currentLanguage);
        }

        // Hide introCanvas và show startCanvas
        if (introCanvasGroup != null)
        {
            yield return StartCoroutine(FadeCanvasGroup(introCanvasGroup, 1f, 0f, uiFadeDuration));
            introCanvas.SetActive(false);
        }
        if (startCanvasGroup != null)
        {
            startCanvas.SetActive(true);
            yield return StartCoroutine(FadeCanvasGroup(startCanvasGroup, 0f, 1f, uiFadeDuration));
        }
    }

    IEnumerator PlayClipWithFade(AudioSource source, AudioClip clip, float fadeInTime, float fadeOutTime)
    {
        if (clip.length < fadeInTime + fadeOutTime)
        {
            float half = clip.length / 2f;
            fadeInTime = half;
            fadeOutTime = half;
        }

        source.volume = 0f;
        source.clip = clip;
        source.Play();

        float timer = 0f;
        while (timer < fadeInTime)
        {
            source.volume = Mathf.Lerp(0f, 1f, timer / fadeInTime);
            timer += Time.deltaTime;
            yield return null;
        }
        source.volume = 1f;

        float waitTime = Mathf.Max(0f, clip.length - fadeInTime - fadeOutTime);
        yield return new WaitForSeconds(waitTime);

        timer = 0f;
        while (timer < fadeOutTime)
        {
            source.volume = Mathf.Lerp(1f, 0f, timer / fadeOutTime);
            timer += Time.deltaTime;
            yield return null;
        }
        source.volume = 0f;
        source.Stop();
    }

    IEnumerator FadeCanvasGroup(CanvasGroup group, float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;
        group.alpha = startAlpha;
        while (elapsed < duration)
        {
            group.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        group.alpha = endAlpha;
    }

    public void SkipSequence()
    {
        StopAllCoroutines();
        if (sequenceAudioSource != null)
            sequenceAudioSource.Stop();

        if (sequenceObjects != null)
        {
            foreach (var obj in sequenceObjects)
                if (obj != null)
                    obj.SetActive(false);
        }

        if (introCanvas != null)
            introCanvas.SetActive(false);
        if (cautionCanvasGroup != null)
        {
            cautionCanvasGroup.alpha = 0f;
            cautionCanvasGroup.gameObject.SetActive(false);
        }
        if (startCanvas != null)
        {
            startCanvas.SetActive(true);
            if (startCanvasGroup != null)
                startCanvasGroup.alpha = 1f;
        }
    }
}
