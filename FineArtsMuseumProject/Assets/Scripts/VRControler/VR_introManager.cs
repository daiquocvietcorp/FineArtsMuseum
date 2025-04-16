using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class VR_introManager : MonoBehaviour
{
    [Header("Canvas")]
    [SerializeField] private GameObject introCanvas;   // Intro Canvas, sẽ chỉ tắt sau khi chạy xong fade out của GameObject cuối trong sequence
    [Header("Chuỗi GameObject & Audio cho từng ngôn ngữ")]
    public GameObject[] sequenceObjects;
    public AudioClip[] sequenceAudiosEnglish;
    public AudioClip[] sequenceAudiosVietnamese;

    [Header("Audio Giới Thiệu")]
    [SerializeField] private AudioClip introAudioVI;
    [SerializeField] private AudioClip introAudioEN;

    [Header("Audio Fade & Delay Settings")]
    [SerializeField] private float fadeInDuration = 1f;      // Thời gian fade in cho audio
    [SerializeField] private float fadeOutDuration = 1f;     // Thời gian fade out cho audio
    [SerializeField] private float clipDelay = 0.5f;           // Delay nửa giây sau mỗi clip

    [Header("UI Fade Settings")]
    [SerializeField] private float uiFadeDuration = 1f;        // Thời gian fade UI (cho introCanvas và startCanvas)

    // AudioSource ambient (nhạc ambient) cần được gán từ Inspector (không bị ảnh hưởng)
    [SerializeField] private AudioSource ambientAudioSource;

    // AudioSource dùng riêng để phát phần giới thiệu và sequence (được tạo động)
    private AudioSource sequenceAudioSource;

    // CanvasGroup để áp dụng hiệu ứng fade trên UI
    private CanvasGroup introCanvasGroup;
    private CanvasGroup startCanvasGroup;
     
    [SerializeField] Button skipButton; 

    // Lưu handle coroutine sequence (nếu cần) – ở đây sử dụng StopAllCoroutines() cho đơn giản.
    // private Coroutine sequenceCoroutine; 

    private void Start()
    {
        if (!PlatformManager.Instance.IsVR) return;
        
        skipButton.onClick.AddListener(SkipSequence);
        // Tạo AudioSource phụ cho sequence giới thiệu (ambient music chạy trên ambientAudioSource)
        sequenceAudioSource = gameObject.AddComponent<AudioSource>();

        // Lấy hoặc thêm CanvasGroup cho introCanvas
        if (introCanvas != null)
        {
            introCanvasGroup = introCanvas.GetComponent<CanvasGroup>();
            if (introCanvasGroup == null)
                introCanvasGroup = introCanvas.AddComponent<CanvasGroup>();
            introCanvasGroup.alpha = 1f;
        }

        // Lấy hoặc thêm CanvasGroup cho startCanvas; khởi đầu alpha = 0
       
        StartCoroutine(PlayIntroAndSequence());
    }

    IEnumerator PlayIntroAndSequence()
    {
        // Lấy ngôn ngữ từ PlayerPrefs ("vi" hoặc "en", mặc định là "vi")
        string currentLanguage = SceneLog.IsVietnamese ? "vi" : "en";

        // 1. Phát audio giới thiệu với hiệu ứng fade in/out
        AudioClip introClip = (currentLanguage == "vi") ? introAudioVI : introAudioEN;
        if (introClip != null)
        {
            yield return StartCoroutine(PlayClipWithFade(sequenceAudioSource, introClip, fadeInDuration, fadeOutDuration));
            yield return new WaitForSeconds(clipDelay);
        }
        else
        {
            Debug.LogWarning("Chưa gán audio giới thiệu cho ngôn ngữ: " + currentLanguage);
        }

        // 2. Chạy chuỗi các GameObject với audio tương ứng
        AudioClip[] selectedSequenceAudios = (currentLanguage == "vi") ? sequenceAudiosVietnamese : sequenceAudiosEnglish;
        int count = Mathf.Min(sequenceObjects.Length, selectedSequenceAudios.Length);
        for (int i = 0; i < count; i++)
        {
            GameObject obj = sequenceObjects[i];
            AudioClip clip = selectedSequenceAudios[i];

            if (obj != null && clip != null)
            {
                // Kích hoạt GameObject hiện tại
                obj.SetActive(true);

                // Phát audio clip với fade in/out
                yield return StartCoroutine(PlayClipWithFade(sequenceAudioSource, clip, fadeInDuration, fadeOutDuration));

                // Sau khi clip chạy xong, tắt GameObject đó
                obj.SetActive(false);

                yield return new WaitForSeconds(clipDelay);
            }
            else
            {
                Debug.LogWarning("Thiếu GameObject hoặc Audio tại index " + i);
            }
        }

        // 3. Sau khi hết chuỗi:
        // Fade out introCanvas và fade in startCanvas (UI)
        if (introCanvasGroup != null)
        {
            yield return StartCoroutine(FadeCanvasGroup(introCanvasGroup, 1f, 0f, uiFadeDuration));
            introCanvas.SetActive(false);
        }
        if (startCanvasGroup != null)
        {
            yield return StartCoroutine(FadeCanvasGroup(startCanvasGroup, 0f, 1f, uiFadeDuration));
        }
    }

    /// <summary>
    /// Phát audio clip với hiệu ứng fade in, chờ phần nội dung clip, sau đó fade out và gọi Stop() để dừng clip.
    /// Nếu clip quá ngắn so với tổng thời gian fade, tự điều chỉnh fade durations.
    /// </summary>
    IEnumerator PlayClipWithFade(AudioSource source, AudioClip clip, float fadeInTime, float fadeOutTime)
    {
        // Nếu clip quá ngắn so với fade durations
        if (clip.length < fadeInTime + fadeOutTime)
        {
            Debug.LogWarning("Clip " + clip.name + " quá ngắn so với thời gian fade. Tự điều chỉnh fade durations.");
            float half = clip.length / 2f;
            fadeInTime = half;
            fadeOutTime = half;
        }

        source.volume = 0f;
        source.clip = clip;
        source.Play();

        // --- Fade In ---
        float timer = 0f;
        while (timer < fadeInTime)
        {
            source.volume = Mathf.Lerp(0f, 1f, timer / fadeInTime);
            timer += Time.deltaTime;
            yield return null;
        }
        source.volume = 1f;

        // Chờ phần nội dung của clip (trừ fade in/out)
        float waitTime = Mathf.Max(0f, clip.length - fadeInTime - fadeOutTime);
        yield return new WaitForSeconds(waitTime);

        // --- Fade Out ---
        timer = 0f;
        while (timer < fadeOutTime)
        {
            source.volume = Mathf.Lerp(1f, 0f, timer / fadeOutTime);
            timer += Time.deltaTime;
            yield return null;
        }
        source.volume = 0f;

        source.Stop();
        yield break;
    }

    /// <summary>
    /// Coroutine để fade alpha của CanvasGroup từ startAlpha đến endAlpha trong thời gian duration.
    /// </summary>
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
        yield break;
    }

    /// <summary>
    /// Hàm SkipSequence() dùng để bỏ qua toàn bộ chuỗi giới thiệu.
    /// Khi gọi hàm này, các coroutine liên quan sẽ bị dừng, audio sequence dừng phát,
    /// các GameObject của chuỗi sẽ bị tắt, introCanvas sẽ ẩn ngay và startCanvas bật lên.
    /// </summary>
    public void SkipSequence()
    {
        // Dừng tất cả các coroutine đang chạy
        StopAllCoroutines();

        // Dừng phát audio của sequence (nếu đang phát)
        if (sequenceAudioSource != null)
        {
            sequenceAudioSource.Stop();
        }

        // Tắt ngay các GameObject trong sequence (nếu có đang active)
        if (sequenceObjects != null)
        {
            foreach (GameObject obj in sequenceObjects)
            {
                if (obj != null)
                {
                    obj.SetActive(false);
                }
            }
        }

        // Ẩn introCanvas ngay lập tức
        if (introCanvas != null)
        {
            introCanvas.SetActive(false);
            if (introCanvasGroup != null)
            {
                introCanvasGroup.alpha = 0f;
            }
        }

        // Bật startCanvas ngay lập tức
    }


}
