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
        private float _previousEnterTime;

        public bool IsDisableForOptimize = false;
        public List<GameObject> disableObjects;
        public List<GameObject> enableObjects;
        private void Start()
        {
            _targetRotation = Quaternion.Euler(cameraRotation);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            if (!PlatformManager.Instance.IsVR)
            {
                if(Time.time - _previousEnterTime <= .5f) return;
                CameraManager.Instance.cameraFollowPlayer.EnterArea(distanceView, heightView);
                CameraManager.Instance.cameraFollowPlayer.SetCameraData(cameraPosition, _targetRotation);
            }
            
            
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
                
                if(enableObjects != null && enableObjects.Count > 0)
                {
                    foreach (var obj in enableObjects)
                    {
                        if (obj != null)
                        {
                            obj.SetActive(true);
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
            if (!PlatformManager.Instance.IsVR)
            {
            	CameraManager.Instance.cameraFollowPlayer.ExitArea();
            	_previousEnterTime = Time.time;
            }       
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
                
                if(enableObjects != null && enableObjects.Count > 0)
                {
                    foreach (var obj in enableObjects)
                    {
                        if (obj != null)
                        {
                            obj.SetActive(false);
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
