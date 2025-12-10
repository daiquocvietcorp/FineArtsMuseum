using System;
using System.Collections;
using System.Collections.Generic;
using DesignPatterns;
using Slider;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UI;

[System.Serializable]
public partial class SubtitleData
{
    public string en;
    public string vi;
}

[System.Serializable]
public class AudioClipData
{
    public string id;
    public string audioPath_en;
    public string audioPath_vi;
    public SubtitleData subtitle;
    public string type; // "dynamic" hoặc "static"
}

[System.Serializable]
public class AudioDataList
{
    public List<AudioClipData> clips;
}

public class AudioSubtitleManager : MonoSingleton<AudioSubtitleManager>
{
    private void OnApplicationQuit()
    {
        //PlayerPrefs.SetString("Language", "vi");
    }

    public static AudioSubtitleManager Instance; // Singleton để dễ gọi từ TriggerZone
    public AudioSource audioSource;
    //public TMP_Text dynamicSubtitleText;
    public List<TMP_Text> staticSubtitleText;
    public float minTimePerLine = 1.0f; // Thời gian tối thiểu để hiển thị mỗi dòng

    // public List<Button> buttons;
    [SerializeField] private Button TriggerButton;
    
    private AudioDataList audioData;
    public string currentLanguage;
    private Coroutine subtitleCoroutine;
    private string currentPlayingAudioId = "";
    
    // Toggle để chọn ngôn ngữ
    public Toggle toggleEnglish;
    public Toggle toggleVietnamese;
    
    public AudioClip ambientSound;
    public float ambientVolume = 0.6f;
    private bool _isPlayingAmbientSound = false;
    private bool _isPlayingAudio = false;
    private bool _isPlayingRoomSound = false;

    [SerializeField] private string audioRoomId;
    
    private Coroutine repeatCoroutine;
    
    private bool _isInitialized = false;
    
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        if(_isInitialized) return;
        _isInitialized = true;
        
        LoadJsonData();
        LoadLanguage(); 
        // AssignButtonEvents();
        // ShowStaticSubtitle();
        // staticSubtitlePanel.SetActive(false);
        if (toggleEnglish != null)
            toggleEnglish.onValueChanged.AddListener((isOn) => OnToggleChanged(isOn, "en"));
        if (toggleVietnamese != null)
            toggleVietnamese.onValueChanged.AddListener((isOn) => OnToggleChanged(isOn, "vi"));
        _isPlayingAmbientSound = false;
        _isPlayingAudio = false;
        if (PlatformManager.Instance.IsVR)
        {
            UIManager.Instance.ActionUI("UI_SOUND");
            
        }
        //TurnAmbientSoundTurnAmbientSound();
    }
    // void AssignButtonEvents()
    // {
    //     foreach (Button btn in buttons)
    //     {
    //         string buttonId = btn.name;
    //         AudioClipData clipData = GetClipDataById(buttonId);
    //         if (clipData != null)
    //         {
    //             btn.onClick.AddListener(() => PlayAudioWithSubtitle(buttonId));
    //         }
    //         else
    //         {
    //             Debug.LogWarning($"No matching audio found for button: {buttonId}");
    //         }
    //     }
    // }

    public void ToggleLanguage(string lang)
    {
        if (!_isInitialized)
        {
            Initialize();
        }
        
        if (lang == "vi")
        {
            toggleVietnamese.isOn = true;
            toggleEnglish.isOn = false;
        }
        else
        {
            toggleVietnamese.isOn = false;
            toggleEnglish.isOn = true;
        }
    }
    
    public void StartArtPanelButton(string id)
    {
        Debug.Log("Vao ham StartArtPanelButton");
        AudioClipData clipData = GetClipDataById(id);
        if (clipData != null && TriggerButton != null)
        {
            Debug.Log("Vao dieu kien 2");
            TriggerButton.gameObject.SetActive(true);
            TriggerButton.onClick.AddListener(() => PlayAudioWithSubtitle(id));
            PlayAudioWithSubtitle(id);
            _isPlayingAudio = true;
        }
        else
        {
            Debug.LogWarning($"No matching audio found for button: {id}");
        }
    }
    
    public AudioClipData GetAudioClipData(string id)
    {
        return GetClipDataById(id);
    }
    
    public void CloseArtPanelButton()
    {
        if(TriggerButton != null)
            TriggerButton.gameObject.SetActive(false);
    }
    void OnToggleChanged(bool isOn, string toggleString)
    {
        if (isOn) // Kiểm tra xem Toggle có được bật hay không
        {
            ChangeLanguage(toggleString);
        }
    }
    public void ChangeLanguage(string lang)
    {
        SceneLog.IsVietnamese = lang == "vi";
        
        currentLanguage = lang;
        Debug.Log("Language changed to: " + currentLanguage);
        ShowStaticSubtitle();
        SlideManager.Instance.ChangeLanguage(currentLanguage);
        
        if (audioSource.isPlaying && _isPlayingAudio)
        {
            string currentAudioId = GetCurrentPlayingAudioId();
            StopAudioAndClearSubtitle(); // Dừng audio hiện tại
            if (!string.IsNullOrEmpty(currentAudioId))
            {
                PlayAudioWithSubtitle(currentAudioId); // Phát lại với ngôn ngữ mới
            }
        }
    }
    public string GetCurrentPlayingAudioId()
    {
        return currentPlayingAudioId;
    }
    void LoadJsonData()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("json/subtitles");
        if (jsonFile != null)
        {
            string jsonText = jsonFile.text;
            audioData = JsonUtility.FromJson<AudioDataList>(jsonText);
        }
        else
        {
            Debug.LogError("JSON file not found in Resources/json/subtitles");
        }
    }
    
    void LoadLanguage()
    {
        // Mặc định là Tiếng Việt nếu chưa chọn
        //currentLanguage = PlayerPrefs.GetString("Language", "vi");
        
        currentLanguage = SceneLog.IsVietnamese ? "vi" : "en";
        toggleEnglish.isOn = (currentLanguage == "en");
        toggleVietnamese.isOn = (currentLanguage == "vi");
        ShowStaticSubtitle();
    }

    
    public void PlayAudioWithSubtitle(string id)
    {
        Debug.Log("Play Audio with Subtitle: " + id);

        // Dừng lại toàn bộ trước khi phát lại
        StopAudioAndClearSubtitle();

        // Nếu đang bật ambient thì tắt
        if (_isPlayingAmbientSound || _isPlayingRoomSound)
        {
            audioSource.Stop();
        }

        currentPlayingAudioId = id;
        AudioClipData clipData = GetClipDataById(id);
        if (clipData == null)
        {
            Debug.LogWarning($"No matching audio found for trigger ID: {id}");
            return;
        }

        string audioPath = (currentLanguage == "vi") ? clipData.audioPath_vi : clipData.audioPath_en;
        AudioClip clip = Resources.Load<AudioClip>(audioPath);
        if (clip == null)
        {
            Debug.LogError("Audio clip not found: " + audioPath);
            return;
        }

        audioSource.clip = clip;
        audioSource.loop = false;
        audioSource.Play();
        _isPlayingAudio = true;

        // Bắt đầu lặp
        if (repeatCoroutine != null)
            StopCoroutine(repeatCoroutine);
        repeatCoroutine = StartCoroutine(RepeatAudioAfterDelay(id, clip.length));

        // Phụ đề
        if (clipData.type == "static")
        {
            
            ShowStaticSubtitle();
        }
        // else if (clipData.type == "dynamic")
        // {
        //     if (subtitleCoroutine != null)
        //         StopCoroutine(subtitleCoroutine);
        //     subtitleCoroutine = StartCoroutine(DisplayDynamicSubtitle(clipData.subtitle, clip.length));
        // }
    }

    
    IEnumerator RepeatAudioAfterDelay(string id, float clipDuration)
    {
        yield return new WaitForSeconds(clipDuration + 10f); // Đợi hết clip + 10s

        Debug.Log("Audio finished, waiting 10 seconds... Replaying.");

        // Gọi lại chính nó
        PlayAudioWithSubtitle(id);
    }
    
    public bool TurnAmbientSound()
    {
        if (_isPlayingAmbientSound)
        {
            if(_isPlayingAudio || _isPlayingRoomSound) return false;
            audioSource.Stop();
            _isPlayingAmbientSound = false;
            return true;
        }
        else
        {
            if(_isPlayingAudio || _isPlayingRoomSound) return false;
            audioSource.clip = ambientSound;
            audioSource.volume = ambientVolume;
            audioSource.loop = true;
            audioSource.Play();
            _isPlayingAmbientSound = true;
            return true;
        }
    }

    void ShowStaticSubtitle()
    {
        foreach (var vText in staticSubtitleText)
        {
            string textId = vText.name; // Lấy tên của TMP_Text để làm ID

            // Tìm subtitle có ID trùng với tên của TMP_Text
            AudioClipData clipData = GetClipDataById(textId);
            if (clipData != null)
            {
                vText.text = currentLanguage == "vi" ? clipData.subtitle.vi : clipData.subtitle.en;
            }
            else
            {
                vText.text = ""; // Xóa nội dung nếu không có dữ liệu phù hợp
                Debug.LogWarning($"No matching subtitle found for ID: {textId}");
            }
            vText.ForceMeshUpdate();
            LayoutRebuilder.ForceRebuildLayoutImmediate(vText.rectTransform);
        }
    }
    
    AudioClipData GetClipDataById(string id)
    {
        foreach (var clip in audioData.clips)
        {
            if (clip.id == id) return clip;
        }
        return null;
    }
    public void StopAudioAndClearSubtitle()
    {
        if(_isPlayingAudio)
        {
            audioSource.Stop();
            _isPlayingAudio = false;

            if (_isPlayingRoomSound)
            {
                TurnOnRoomAudio();
            }
            else if (_isPlayingAmbientSound)
            {
                audioSource.clip = ambientSound;
                audioSource.volume = ambientVolume;
                audioSource.loop = true;
                audioSource.Play();
            }
        }

        if (subtitleCoroutine != null)
        {
            StopCoroutine(subtitleCoroutine);
            subtitleCoroutine = null;
        }

        if (repeatCoroutine != null)
        {
            StopCoroutine(repeatCoroutine);
            repeatCoroutine = null;
        }

        _isPlayingAudio = false;
        //dynamicSubtitleText.text = ""; // Xóa subtitle động
        currentPlayingAudioId = "";
    }

    public bool TurnRoomAudio()
    {
        //if(_isPlayingAudio) return false;
        
        if (!_isPlayingRoomSound)
        {
            _isPlayingRoomSound = true;
            
            if(_isPlayingAudio) return true;
            TurnOnRoomAudio();
            return true;
        }
        else
        {
            _isPlayingRoomSound = false;
            
            if(_isPlayingAudio) return true;
            
            TurnOffRoomAudio();
            return true;
        }
    }
    
    private void TurnOnRoomAudio()
    {
        if (!_isPlayingRoomSound || _isPlayingAudio) return;
        
        var clipData = GetClipDataById(audioRoomId);
        if (clipData == null)
        {
            Debug.LogWarning($"No matching audio found for trigger ID: {audioRoomId}");
            return;
        }
            
        var audioPath = (currentLanguage == "vi") ? clipData.audioPath_vi : clipData.audioPath_en;
        var clip = Resources.Load<AudioClip>(audioPath);
            
        if (clip == null)
        {
            Debug.LogError("Audio clip not found: " + audioPath);
            return;
        }

        audioSource.clip = clip;
        audioSource.loop = false;
        audioSource.Play();
        
        if (repeatCoroutine != null)
            StopCoroutine(repeatCoroutine);
        repeatCoroutine = StartCoroutine(RepeatRoomAudioAfterDelay(clip.length));
    }

    private IEnumerator RepeatRoomAudioAfterDelay(float clipDuration)
    {
        yield return new WaitForSeconds(clipDuration + 10f); // Đợi hết clip + 10s

        Debug.Log("Audio finished, waiting 10 seconds... Replaying.");

        // Gọi lại chính nó
        TurnOnRoomAudio();
    }
    
    private void TurnOffRoomAudio()
    {
        if (!_isPlayingRoomSound || _isPlayingAudio) return;
        audioSource.Stop();
        audioSource.clip = null;
        
        if(!_isPlayingAmbientSound) return;
        audioSource.clip = ambientSound;
        audioSource.volume = ambientVolume;
        audioSource.loop = true;
        audioSource.Play();
    }
}
