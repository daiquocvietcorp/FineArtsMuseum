using System;
using System.Collections.Generic;
using Camera;
using UnityEngine;
using UnityEngine.Serialization;

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

        public bool IsDisableForOptimize = false;
        public List<GameObject> disableObjects;
        private void Start()
        {
            _targetRotation = Quaternion.Euler(cameraRotation);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            CameraManager.Instance.cameraFollowPlayer.EnterArea(distanceView, heightView);
            CameraManager.Instance.cameraFollowPlayer.SetCameraData(cameraPosition, _targetRotation);
            
            if (IsDisableForOptimize)
            {
                if(disableObjects != null && disableObjects.Count > 0)
                {
                    foreach (var obj in disableObjects)
                    {
                        if (obj != null)
                        {
                            obj.SetActive(false);
                        }
                    }
                }
            }
            
            if(topMirror != null)
                topMirror.gameObject.SetActive(false);
            
            if(bottomMirror != null)
                bottomMirror.gameObject.SetActive(false);
            
            if(topMirror == null || bottomMirror == null)
                return;
            SlideManager.Instance.EnterSlideArea();
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            CameraManager.Instance.cameraFollowPlayer.ExitArea();
            
            if (IsDisableForOptimize)
            {
                if(disableObjects != null && disableObjects.Count > 0)
                {
                    foreach (var obj in disableObjects)
                    {
                        if (obj != null)
                        {
                            obj.SetActive(true);
                        }
                    }
                }
            }
            
            if(topMirror != null)
                topMirror.gameObject.SetActive(true);
            
            if(bottomMirror != null)
                bottomMirror.gameObject.SetActive(true);
            
            if(topMirror == null || bottomMirror == null)
                return;
            SlideManager.Instance.ExitSlideArea();
        }
    }
}
