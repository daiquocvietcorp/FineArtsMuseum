using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;

public class HandTrackingToggleObject : MonoBehaviour
{
    public GameObject leftHandTracking;

    public GameObject HandtrackingController;

    

    void Update()
    {
        if (leftHandTracking.activeSelf == false)
        {
            HandtrackingController.SetActive(false);
        }
        else
        {
            HandtrackingController.SetActive(true);
        }
    }
}