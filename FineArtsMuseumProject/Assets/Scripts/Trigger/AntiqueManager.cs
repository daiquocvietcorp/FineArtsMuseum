using System;
using System.Collections.Generic;
using DesignPatterns;
using InputController;
using UI;
using UnityEngine;

namespace Trigger
{
    public class AntiqueManager : MonoSingleton<AntiqueManager>
    {
        [field: SerializeField] private List<AntiqueObject> antiqueObjects;
        private Dictionary<string, AntiqueObject> _antiqueDetailDict;
        private PaintRotateAndZoom _currentAntiqueObject;
        
        [field: SerializeField] private ArcSlider arcSlider;
        [field: SerializeField] private Transform sliderTransform;
        
        private bool _isShowAntiqueDetail;
        
        public bool IsShowAntiqueDetail => _isShowAntiqueDetail;

        private void Awake()
        {
            _antiqueDetailDict = new Dictionary<string, AntiqueObject>();

            foreach (var pair in antiqueObjects)
            {
                pair.gameObject.SetActive(false);
                _antiqueDetailDict.TryAdd(pair.GetAntiqueID(), pair);
            }
        }

        public void EnableAntiqueDetail(string antiqueID)
        {
            if (!_antiqueDetailDict.ContainsKey(antiqueID))
            {
                if (PlatformManager.Instance.IsStandalone || PlatformManager.Instance.IsWebGL)
                {
                    antiqueID += "_pc";
                }

                if (PlatformManager.Instance.IsMobile || PlatformManager.Instance.IsCloud)
                {
                    antiqueID += "_mobile";
                }
                
                if (PlatformManager.Instance.IsVR)
                {
                    Debug.Log("VR Detected");
                    antiqueID += "_vr";
                }

                if (PlatformManager.Instance.IsTomko)
                {
                    antiqueID += "_tomko";
                }
            }
            Debug.Log("AntiqueId:"+ antiqueID);
            _antiqueDetailDict[antiqueID].gameObject.SetActive(true);
            TriggerManager.Instance.DisableTriggerColliders();
            _isShowAntiqueDetail = true;
            if (_antiqueDetailDict[antiqueID].interactiveObject)
            {
                _currentAntiqueObject = _antiqueDetailDict[antiqueID].interactiveObject;

                if (PlatformManager.Instance.IsTomko || PlatformManager.Instance.IsVR)
                {
                    sliderTransform.gameObject.SetActive(true);
                    ResetSlider();
                }
            }
            UIManager.Instance.DisableUI("UI_NAVIGATION");
            
            if (!PlatformManager.Instance.IsMobile && !PlatformManager.Instance.IsCloud && !PlatformManager.Instance.IsTomko) return;
            InputManager.Instance.DisableJoystick();
            
            if(PlatformManager.Instance.IsTomko) return;
            InputManager.Instance.DisableJoystickRotation();
        }

        public void ResetSlider()
        {
            var valuePercent = _currentAntiqueObject.GetOriginalScalePercent();
            arcSlider.SetValue(valuePercent);
        }

        public void DisableAntiqueDetail(string antiqueID)
        {
            if (_antiqueDetailDict.ContainsKey(antiqueID))
            {
                TriggerManager.Instance.EnableTriggerColliders();
                _isShowAntiqueDetail = false;
                _currentAntiqueObject = null;
                _antiqueDetailDict[antiqueID].gameObject.SetActive(false);
                
                if (PlatformManager.Instance.IsTomko || PlatformManager.Instance.IsVR)
                    sliderTransform.gameObject.SetActive(false);
            }
            
            if (!PlatformManager.Instance.IsMobile && !PlatformManager.Instance.IsCloud && !PlatformManager.Instance.IsTomko) return;
            InputManager.Instance.EnableJoystick();
            
            if(PlatformManager.Instance.IsTomko) return;
            InputManager.Instance.EnableJoystickRotation();
        }

        public void EnableSoundAntique(string antiqueID)
        {
            if (!_antiqueDetailDict.ContainsKey(antiqueID))
            {
                if (PlatformManager.Instance.IsStandalone || PlatformManager.Instance.IsWebGL)
                {
                    antiqueID += "_pc";
                }
                if (PlatformManager.Instance.IsVR)
                {
                    antiqueID += "_vr";
                }
                if (PlatformManager.Instance.IsMobile || PlatformManager.Instance.IsCloud)
                {
                    antiqueID += "_mobile";
                }
            }
        }

        public void ResetView()
        {
            if(_currentAntiqueObject == null) return;
            _currentAntiqueObject.SmoothAverageResetTransform();
        }

        public void ZoomAntique(float f)
        {
            if(_currentAntiqueObject == null) return;
            _currentAntiqueObject.ZoomByPercentage(f);
        }

        public bool IsChangeArcSlider()
        {
            return arcSlider.IsChangeValue();
        }
    }
}
