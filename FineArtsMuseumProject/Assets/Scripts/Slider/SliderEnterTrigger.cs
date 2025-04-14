using System;
using Camera;
using UnityEngine;

namespace Slider
{
    public class SliderEnterTrigger : MonoBehaviour
    {
        [SerializeField] private Vector3 cameraPosition;
        [SerializeField] private Vector3 cameraRotation;
        [SerializeField] private float distanceView;
        [SerializeField] private float heightView;
        
        [SerializeField] private Transform topMirror;
        [SerializeField] private Transform bottomMirror;
        
        private Quaternion _targetRotation;

        private void Start()
        {
            _targetRotation = Quaternion.Euler(cameraRotation);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            CameraManager.Instance.cameraFollowPlayer.EnterArea(distanceView, heightView);
            CameraManager.Instance.cameraFollowPlayer.SetCameraData(cameraPosition, _targetRotation);
            
            if(topMirror != null)
                topMirror.gameObject.SetActive(false);
            
            if(bottomMirror != null)
                bottomMirror.gameObject.SetActive(false);
            SlideManager.Instance.EnterSlideArea();
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            CameraManager.Instance.cameraFollowPlayer.ExitArea();
            
            if(topMirror != null)
                topMirror.gameObject.SetActive(true);
            
            if(bottomMirror != null)
                bottomMirror.gameObject.SetActive(true);
            SlideManager.Instance.ExitSlideArea();
        }
    }
}
