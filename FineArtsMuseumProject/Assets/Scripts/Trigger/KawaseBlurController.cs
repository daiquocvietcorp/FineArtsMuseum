using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class KawaseBlurController : MonoBehaviour
{
    #region KawaseBlur

    public UniversalRendererData rendererData; // Gán trong Inspector
    private KawaseBlur kawaseBlur;

    void Start()
    {
        // Lấy Kawase Blur Feature từ Renderer Data
        if (rendererData != null)
        {
            foreach (var feature in rendererData.rendererFeatures)
            {
                if (feature is KawaseBlur)
                {
                    kawaseBlur = feature as KawaseBlur;
                    break;
                }
            }
        }

        if (kawaseBlur == null)
        {
            Debug.LogError("Không tìm thấy Kawase Blur Feature trong Renderer Data.");
            return;
        }
    }

    public void EnableBlur(bool enable)
    {
        if (kawaseBlur != null)
        {
            kawaseBlur.SetActive(enable);
        }
    }

    public void SetBlurPasses(int passes)
    {
        if (kawaseBlur != null)
        {
            kawaseBlur.settings.blurPasses = passes; // Số lần lặp của hiệu ứng mờ
        }
    }

    public void SetBlurDownsample(int downsample)
    {
        if (kawaseBlur != null)
        {
            kawaseBlur.settings.downsample = downsample; // Mức độ lan rộng của làm mờ
        }
    }
    
    public void SetCopyToFramebuffer(bool copyToFB)
    {
        if (kawaseBlur != null)
        {
            kawaseBlur.settings.copyToFramebuffer = copyToFB;
        }
    }

    #endregion
    
}