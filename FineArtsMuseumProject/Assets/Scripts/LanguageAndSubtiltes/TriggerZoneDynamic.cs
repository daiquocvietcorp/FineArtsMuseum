using System.Collections;
using Audio;
using UnityEngine;

public class TriggerZoneDynamic: MonoBehaviour
{
    public string triggerId; // ID của vùng trigger, tương ứng với JSON

    private bool playerInside = false;
    private Coroutine triggerCoroutine;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !playerInside)
        {
            Debug.Log("Trigger AM Thanh ne");
            playerInside = true;
            triggerCoroutine = StartCoroutine(TriggerEvent());
            //var audioData = AudioSubtitleManager.Instance.GetAudioClipData(triggerId);
            //AudioManager.Instance.EnterAudioArea(audioData.);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;

            // Hủy Coroutine nếu player rời đi trước khi audio phát
            if (triggerCoroutine != null)
            {
                StopCoroutine(triggerCoroutine);
            }

            // Dừng Audio và Xóa Subtitle
            AudioSubtitleManager.Instance.StopAudioAndClearSubtitle();
            AudioSubtitleManager.Instance.CloseArtPanelButton();
        }
    }

    IEnumerator TriggerEvent()
    {
        Debug.Log("Vao ham Trigger Event");
        yield return new WaitForSeconds(0f); // Delay 0.5s trước khi chạy
        AudioSubtitleManager.Instance.StartArtPanelButton(triggerId);
    }
}
