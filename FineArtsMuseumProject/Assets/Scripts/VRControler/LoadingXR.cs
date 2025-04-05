using System;
using UnityEngine;

public class LoadingXR : MonoBehaviour
{
    [SerializeField] private Transform VRLoadingArea;

    private void Start()
    {
        if (PlatformManager.Instance.IsVR)
        {
            VRLoadingArea.gameObject.SetActive(true);
            StartCoroutine(LoadVRScene());
        }
        else
        {
            VRLoadingArea.gameObject.SetActive(false);
        }
    }

    private string LoadVRScene()
    {
        throw new NotImplementedException();
    }
}
