using System;
using System.Collections;
using System.Collections.Generic;
using AmazingAssets.DynamicRadialMasks;
using Camera;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.VFX;

public class TriggerPainting : MonoBehaviour
{
    public DRMGameObject DrmGameObject;
    public float scanSpeed = 1f;
    public float maxScanRadius = 100f;
    public float reduceSpeedMultiplier = 1.2f;
    
    public GameObject paintingObject;
    public GameObject otherObject;
    public GameObject SubtitleObject;
    public GameObject ButtonGroupCanvas_pc;
    public GameObject ButtonGroupCanvas_vr;
    public GameObject ButtonGroupCanvas_mobile;
    public GameObject ButtonGroupCanvas_tomko;
    public GameObject ScreenOutlineEffect;
    public GameObject Player;
    
    public GameObject VRPlayer;
    public Collider detailCollider;
    public TriggerPaintDetail triggerPaintDetail;
    public Renderer renderer;
    
    public UIPainting UIPainting;
    
    public PaintRotateAndZoom paintRotateAndZoom;
    
    private bool isEnter = false;
    private bool currentTrigger = false;

    public Material skyboxMaterial;
    
    public float atmosphereChangeRate = 0.01f; // Tốc độ thay đổi atmosphereThickness mỗi giây
    public float exposureChangeRate = 0.02f; // Tốc độ thay đổi Exposure mỗi giây
    public float minAtmosphereThickness = 0.5f; // Giá trị nhỏ nhất khi trong vùng trigger
    public float maxAtmosphereThickness = 1.5f; // Giá trị tối đa ban đầu
    public float minExposure = 0.5f;
    public float maxExposure = 1.5f;
    
    // Fog Settings
    public float fogDensityChangeRate = 0.05f; // Tốc độ thay đổi Density của Fog
    public float minFogDensity = 0.02f;
    public float maxFogDensity = 0.1f;

    public VisualEffect fogVFX;

    [field: SerializeField] private float distanceCamera;
    [field: SerializeField] private float heightCamera;
    [field: SerializeField] private Vector3 cameraPositionOffset;
    [field: SerializeField] private Vector3 cameraRotationOffset;
    
    private BoxCollider _objectCollider;

    

    private void Start()
    {
        // Thiết lập Fog Mode
        // RenderSettings.fog = true;
        // RenderSettings.fogMode = FogMode.ExponentialSquared;
        fogVFX.Stop();
        //paintRotateAndZoom = paintingObject.GetComponent<PaintRotateAndZoom>();
        //ButtonGroupCanvas.gameObject.SetActive(false);
        
        _objectCollider = paintingObject.GetComponent<BoxCollider>();
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
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!PlatformManager.Instance.IsVR)
            {
                //CameraManager.Instance.cameraFollowPlayer.RotateCamera(otherObject.transform);
                var rotation = Quaternion.Euler(cameraRotationOffset); 
                CameraManager.Instance.cameraFollowPlayer.SetCameraData(cameraPositionOffset, rotation);
            }
            //DrmGameObject.gameObject.SetActive(true);
            renderer.enabled = true;
            DrmGameObject.transform.position = transform.position;
            fogVFX.transform.position = new Vector3(transform.position.x, 2, transform.position.z);

            SetLayerRecursively(detailCollider.gameObject, "Default", true);
            SetLayerRecursively(paintingObject, "IgnoreBlur", true);
            SetLayerRecursively(otherObject, "IgnoreBlur", true);
            SetLayerRecursively(Player, "IgnoreBlur", true);
            SetLayerRecursively(VRPlayer, "IgnoreBlur", true);

            ScreenOutlineEffect.SetActive(false);
            SetLayerRecursively(ScreenOutlineEffect, "IgnoreBlur", true);
            paintRotateAndZoom.enabled = true;

            // 🔽 Set scale theo trung bình cộng min/max
            paintRotateAndZoom.SetAverageScale();

            detailCollider.enabled = false;

            if (PlatformManager.Instance.IsStandalone || PlatformManager.Instance.IsWebGL)
            {
                ButtonGroupCanvas_pc.gameObject.SetActive(true);
            }
            
            if (PlatformManager.Instance.IsMobile || PlatformManager.Instance.IsCloud)
            {
                ButtonGroupCanvas_mobile.gameObject.SetActive(true);
            }
            if (PlatformManager.Instance.IsVR)
            {
                ButtonGroupCanvas_vr.gameObject.SetActive(true);
            }
            if (PlatformManager.Instance.IsTomko)
            {
                ButtonGroupCanvas_tomko.gameObject.SetActive(true);
            }
            
            SubtitleObject.SetActive(true);
            isEnter = true;
            currentTrigger = true;
            fogVFX.Play();
            
            CameraManager.Instance.SetCameraWhenEnterPainting(distanceCamera, heightCamera);
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isEnter = false;    
            fogVFX.Stop();
            paintRotateAndZoom.SmoothOriginResetTransform();
            paintRotateAndZoom.enabled = false;
            SetLayerRecursively(ScreenOutlineEffect, "Default", true);
            ScreenOutlineEffect.SetActive(true);
            //Debug.Log(other.gameObject.name);
            
            SetLayerRecursively(VRPlayer, "IgnoreBlur", true);

            SetLayerRecursively(Player, "Default", true);
            SubtitleObject.SetActive(false);
            UIPainting.magnifierHover.enabled = false;
            detailCollider.enabled = true;
            detailCollider.GetComponent<TriggerPaintDetail>().ResetFade();
            //detailCollider.GetComponent<TriggerPaintDetail>().triggerPainting = null;
            detailCollider.GetComponent<TriggerPaintDetail>().paintDescription.SetActive(false);
            UIPainting.SetDefaultAll();
            
            CameraManager.Instance.SetCameraWhenExitPainting();
        }
    }

    private void Update()
    {
        if (fogVFX && fogVFX.isActiveAndEnabled)
        {
            fogVFX.SetVector3("ColliderPosition", transform.position);
        }
        
        if (DrmGameObject.radius == maxScanRadius && isEnter && DrmGameObject.gameObject.activeSelf && currentTrigger)
        {
            renderer.enabled = false;
        }
        
        if(DrmGameObject.radius <= 0f && !isEnter && DrmGameObject.gameObject.activeSelf && currentTrigger) 
        {
            DrmGameObject.radius = 0f;
            //paintingObject.layer = LayerMask.NameToLayer("Default");
            
            if(PlatformManager.Instance.IsStandalone || PlatformManager.Instance.IsWebGL)
            {
                ButtonGroupCanvas_pc.gameObject.SetActive(false);
            }
            
            if(PlatformManager.Instance.IsMobile || PlatformManager.Instance.IsCloud)
            {
                ButtonGroupCanvas_mobile.gameObject.SetActive(false);
            }
            if(PlatformManager.Instance.IsVR)
            {
                ButtonGroupCanvas_vr.gameObject.SetActive(false);
            }
            if(PlatformManager.Instance.IsTomko)
            {
                ButtonGroupCanvas_tomko.gameObject.SetActive(false);
            }
            
            //otherObject.layer = LayerMask.NameToLayer("Default");
            SetLayerRecursively(paintingObject, "Default", true);
            SetLayerRecursively(detailCollider.gameObject, "Highlighter", true);
            SetLayerRecursively(otherObject, "Default", true);
            SetLayerRecursively(Player, "Default", true);
            SetLayerRecursively(VRPlayer, "IgnoreBlur", true);
            
            currentTrigger = false;
            renderer.enabled = false;
        }
        
        if (isEnter && DrmGameObject.radius < maxScanRadius && currentTrigger)
        {
            DrmGameObject.radius += Time.deltaTime * scanSpeed;
            
        }
        else if(!isEnter && DrmGameObject.radius > 0 && currentTrigger)
        {
            DrmGameObject.radius -= Time.deltaTime * scanSpeed * reduceSpeedMultiplier;
        }
        
        float currentAtmosphere = skyboxMaterial.GetFloat("_AtmosphereThickness");
        float currentExposure = skyboxMaterial.GetFloat("_Exposure");
        
        if (isEnter && currentTrigger)
        {
            // Giảm giá trị khi vào vùng trigger
            currentAtmosphere = Mathf.Max(currentAtmosphere - atmosphereChangeRate * Time.deltaTime, minAtmosphereThickness);
            currentExposure = Mathf.Max(currentExposure - exposureChangeRate * Time.deltaTime, minExposure);
        }
        else if(!isEnter && currentTrigger)
        {
            // Tăng lại giá trị khi rời khỏi vùng trigger
            currentAtmosphere = Mathf.Min(currentAtmosphere + atmosphereChangeRate * Time.deltaTime, maxAtmosphereThickness);
            currentExposure = Mathf.Min(currentExposure + exposureChangeRate * Time.deltaTime, maxExposure);
        }

        skyboxMaterial.SetFloat("_AtmosphereThickness", currentAtmosphere);
        skyboxMaterial.SetFloat("_Exposure", currentExposure);
        
        // Điều chỉnh Fog Density
        // float currentFogDensity = RenderSettings.fogDensity;
        //
        // if (isEnter && currentTrigger)
        // {
        //     // Tăng Fog Density khi vào vùng trigger
        //     currentFogDensity = Mathf.Min(currentFogDensity + fogDensityChangeRate * Time.deltaTime, maxFogDensity);
        // }
        // else if(!isEnter && currentTrigger)
        // {
        //     // Giảm Fog Density khi rời khỏi vùng trigger
        //     currentFogDensity = Mathf.Max(currentFogDensity - fogDensityChangeRate * Time.deltaTime, minFogDensity);
        // }
        //
        // RenderSettings.fogDensity = currentFogDensity;
    }
}
