using System;
using System.Collections;
using System.Collections.Generic;
using AmazingAssets.DynamicRadialMasks;
using Camera;
using UnityEngine;
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

    public Renderer renderer;
    
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

    private void Start()
    {
        // Thiết lập Fog Mode
        // RenderSettings.fog = true;
        // RenderSettings.fogMode = FogMode.ExponentialSquared;
        
        fogVFX.Stop();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CameraManager.Instance.cameraFollowPlayer.RotateCamera(paintingObject.transform);
            //DrmGameObject.gameObject.SetActive(true);
            renderer.enabled = true;
            DrmGameObject.transform.position = transform.position;
            fogVFX.transform.position = new Vector3(transform.position.x, 2, transform.position.z);
            paintingObject.layer = LayerMask.NameToLayer("IgnoreBlur");
            otherObject.layer = LayerMask.NameToLayer("IgnoreBlur");
            SubtitleObject.SetActive(true);
            isEnter = true;
            currentTrigger = true;
            //UnityEngine.Camera.main.transform.LookAt(paintingObject.transform);
            fogVFX.Play();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isEnter = false;    
            fogVFX.Stop();
            
            SubtitleObject.SetActive(false);
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
            paintingObject.layer = LayerMask.NameToLayer("Default");
            otherObject.layer = LayerMask.NameToLayer("Default");
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
