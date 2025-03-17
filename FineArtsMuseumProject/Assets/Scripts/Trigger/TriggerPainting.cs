using System;
using System.Collections;
using System.Collections.Generic;
using AmazingAssets.DynamicRadialMasks;
using UnityEngine;

public class TriggerPainting : MonoBehaviour
{
    public DRMGameObject DrmGameObject;
    public float scanSpeed = 1f;
    public float maxScanRadius = 100f;
    public float reduceSpeedMultiplier = 1.2f;
    
    public GameObject paintingObject;
    public GameObject otherObject;
    private bool isEnter = false;
    private bool currentTrigger = false;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DrmGameObject.gameObject.SetActive(true);
            DrmGameObject.transform.position = transform.position;
            paintingObject.layer = LayerMask.NameToLayer("IgnoreBlur");
            otherObject.layer = LayerMask.NameToLayer("IgnoreBlur");
            isEnter = true;
            currentTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            
            isEnter = false;    
            
        }
    }

    private void Update()
    {
        if (DrmGameObject.radius == maxScanRadius && isEnter && DrmGameObject.gameObject.activeSelf && currentTrigger)
        {
            DrmGameObject.gameObject.SetActive(false);
        }
        
        if(DrmGameObject.radius <= 0f && !isEnter && DrmGameObject.gameObject.activeSelf && currentTrigger)
        {
            DrmGameObject.radius = 0f;
            paintingObject.layer = LayerMask.NameToLayer("Default");
            otherObject.layer = LayerMask.NameToLayer("Default");
            currentTrigger = false;
        }
        
        if (isEnter && DrmGameObject.radius < maxScanRadius && currentTrigger)
        {
            DrmGameObject.radius += Time.deltaTime * scanSpeed;
        }
        else if(!isEnter && DrmGameObject.radius > 0 && currentTrigger)
        {
            DrmGameObject.radius -= Time.deltaTime * scanSpeed * reduceSpeedMultiplier;
        }
    }
}
