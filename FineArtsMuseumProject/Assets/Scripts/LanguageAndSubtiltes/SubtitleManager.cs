using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using Newtonsoft.Json;

public class SubtitleManager : MonoBehaviour
{
    public List<TextMeshProUGUI> subtitleTexts; // Danh sách tất cả TextMeshPro trong scene
    public AudioSource audioSource;
    public string language = "vi";

    private Dictionary<string, TextMeshProUGUI> subtitleFields = new Dictionary<string, TextMeshProUGUI>();
    private Dictionary<string, SubtitleEntry> subtitles;
    private Dictionary<string, Coroutine> activeCoroutines = new Dictionary<string, Coroutine>();

    void Start()
    {
        // Tự động ánh xạ tên GameObject với TextMeshProUGUI
        foreach (var text in subtitleTexts)
        {
            subtitleFields[text.gameObject.name] = text;
        }

        LoadSubtitles();
        StartAllSubtitles();
    }

    public void SetLanguage(string newLanguage)
    {
        language = newLanguage;
        StopAllSubtitles();
        LoadSubtitles();
        StartAllSubtitles();
    }

    void LoadSubtitles()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("json/subtitles");
        if (jsonFile == null)
        {
            Debug.LogError("Không tìm thấy file subtitles.json trong Resources!");
            return;
        }

        string jsonData = jsonFile.text;
        Dictionary<string, Dictionary<string, SubtitleEntry>> subtitleData =
            JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, SubtitleEntry>>>(jsonData);

        if (subtitleData.ContainsKey(language))
        {
            subtitles = subtitleData[language];
        }
        else
        {
            Debug.LogError("Không tìm thấy phụ đề cho ngôn ngữ: " + language);
        }
    }

    void StartAllSubtitles()
    {
        if (subtitles == null) return;

        foreach (var entry in subtitles)
        {
            string fieldKey = entry.Key;
            SubtitleEntry subtitleEntry = entry.Value;

            if (!subtitleFields.ContainsKey(fieldKey)) continue;

            TextMeshProUGUI textUI = subtitleFields[fieldKey];

            if (subtitleEntry.type == "dynamic")
            {
                activeCoroutines[fieldKey] = StartCoroutine(DisplaySubtitles(textUI, subtitleEntry.content));
            }
            else if (subtitleEntry.type == "static")
            {
                textUI.text = subtitleEntry.staticText;
            }
        }
    }

    void StopAllSubtitles()
    {
        foreach (var coroutine in activeCoroutines.Values)
        {
            if (coroutine != null)
                StopCoroutine(coroutine);
        }
        activeCoroutines.Clear();
    }

    IEnumerator DisplaySubtitles(TextMeshProUGUI textUI, List<SubtitleData> subtitleList)
    {
        int currentIndex = 0;

        while (currentIndex < subtitleList.Count)
        {
            SubtitleData subtitle = subtitleList[currentIndex];

            // Đợi đúng thời gian của câu thoại tiếp theo
            yield return new WaitForSeconds(subtitle.startTime - audioSource.time);

            // Thay thế đoạn text cũ bằng câu thoại mới
            textUI.text = subtitle.text;

            // Đợi hết thời gian hiển thị
            yield return new WaitForSeconds(subtitle.endTime - subtitle.startTime);

            currentIndex++;
        }

        // Khi hết tất cả phụ đề, giữ lại câu cuối hoặc xóa hết (tùy chỉnh)
        textUI.text = ""; // Nếu muốn giữ câu cuối, hãy comment dòng này
    }
}

[System.Serializable]
public class SubtitleEntry
{
    public string type; // "dynamic" hoặc "static"
    public List<SubtitleData> content; // Nếu là dynamic
    public string staticText; // Nếu là static
}

[System.Serializable]
public class SubtitleData
{
    public float startTime;
    public float endTime;
    public string text;
}
