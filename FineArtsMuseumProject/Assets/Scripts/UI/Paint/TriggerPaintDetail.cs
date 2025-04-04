using System.Collections;
using System.Collections.Generic;
using Trigger;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TriggerPaintDetail : MonoBehaviour,IPointerDownHandler
{
    public GameObject paintDescription;
    public GameObject paintObject;
    public GameObject wsObject;
    public GameObject playerObject;

    public TriggerPainting triggerPainting;

    [field: SerializeField] private string paintingTriggerId;
    
    public int paintId;
    
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("!");
        //paintDescription.SetActive(true);
        PaintingManager.Instance.EnablePaintDetail(paintingTriggerId);
        StartFade();
    }

    public void FakePointerDown()
    {
        //paintDescription.SetActive(true);
        
        Debug.Log("DEBUG VR POINTER DOWN");
        PaintingManager.Instance.EnablePaintDetail(paintingTriggerId);
        StartFade();
    }
    
    
    public Volume volume;
    public float fadeDuration = 2f;

    [Header("Bokeh DoF Settings")]
    public float startFocusDistance = 0.1f;
    public float endFocusDistance = 10f;
    public float startAperture = 8f;
    public float endAperture = 1.2f;
    public float startFocalLength = 50f;
    public float endFocalLength = 150f;

    private DepthOfField dof;
    private float timer = 0f;
    private bool isFading = false;
    private bool isResetting = false;

    void Start()
    {
        if (volume.profile.TryGet(out dof))
        {
            dof.active = true;
            dof.mode.value = DepthOfFieldMode.Bokeh;

            // Gán giá trị bắt đầu
            dof.focusDistance.value = startFocusDistance;
            dof.aperture.value = startAperture;
            dof.focalLength.value = startFocalLength;
        }
        else
        {
            Debug.LogWarning("Không tìm thấy DepthOfField trong Volume Profile!");
        }
    }

    void Update()
    {
        if (isFading || isResetting)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / fadeDuration);

            if (isFading)
            {
                dof.focusDistance.value = Mathf.Lerp(startFocusDistance, endFocusDistance, t);
                dof.aperture.value = Mathf.Lerp(startAperture, endAperture, t);
                dof.focalLength.value = Mathf.Lerp(startFocalLength, endFocalLength, t);

                if (t >= 1f) isFading = false;
            }
            else if (isResetting)
            {
                dof.focusDistance.value = Mathf.Lerp(endFocusDistance, startFocusDistance, t);
                dof.aperture.value = Mathf.Lerp(endAperture, startAperture, t);
                dof.focalLength.value = Mathf.Lerp(endFocalLength, startFocalLength, t);

                if (t >= 1f) isResetting = false;
            }
        }
    }
    
    public static void SetLayerRecursively(GameObject obj, string layerName, bool includeSelf = true)
    {
        int newLayer = LayerMask.NameToLayer(layerName);
        if (newLayer == -1)
        {
            Debug.LogError("Layer không tồn tại: " + layerName);
            return;
        }

        SetLayerRecursively(obj, newLayer, includeSelf);
    }
    
    private static void SetLayerRecursively(GameObject obj, int newLayer, bool includeSelf)
    {
        if (obj == null) return;

        if (includeSelf)
            obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer, true);
        }
    }

    public void StartFade()
    {
        SetLayerRecursively(paintObject,"Default", true);
        SetLayerRecursively(playerObject,"Default", true);
        SetLayerRecursively(wsObject,"Default", true);
        SetLayerRecursively(this.gameObject,"Default", true);
        
        timer = 0f;
        isFading = true;
        isResetting = false;
    }

    public void ResetFade()
    {
        SetLayerRecursively(paintObject,"IgnoreBlur", true);
        SetLayerRecursively(playerObject,"IgnoreBlur", true);
        SetLayerRecursively(wsObject,"IgnoreBlur", true);
        SetLayerRecursively(this.gameObject,"Highlighter", true);
        
        timer = 0f;
        isFading = false;
        isResetting = true;
    }
}
