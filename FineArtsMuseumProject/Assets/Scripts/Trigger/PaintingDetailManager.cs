using System;
using System.Collections.Generic;
using Camera;
using DesignPatterns;
using InputController;
using UI;
using UnityEngine;

namespace Trigger
{
    public class PaintingDetailManager : MonoSingleton<PaintingDetailManager>
    {
        [field: SerializeField] private List<PaintDetail> paintDetails;
        [field: SerializeField] private ArcSlider arcSlider;
        [field: SerializeField] private Transform sliderTransform;
        
        private Dictionary<string, PaintDetail> _paintDetailDict;
        private PaintDetail _currentPaintDetail;
        private PaintRotateAndZoom _currentPainting;
        private GameObject _buttonGroup;
        private float _avarageScale = -1f;
        
        private void Awake()
        {
            _paintDetailDict = new Dictionary<string, PaintDetail>();
            _currentPaintDetail = null;

            foreach (var pair in paintDetails)
            {
                pair.gameObject.SetActive(false);
                _paintDetailDict.Add(pair.GetPaintID(), pair);
            }
        }

        public void EnablePaintDetail(string paintID)
        {
            if (!_paintDetailDict.ContainsKey(paintID))
            {
                if (PlatformManager.Instance.IsStandalone || PlatformManager.Instance.IsWebGL)
                {
                    paintID += "_pc";
                }

                if (PlatformManager.Instance.IsMobile || PlatformManager.Instance.IsCloud)
                {
                    paintID += "_mobile";
                }
                
                if (PlatformManager.Instance.IsVR)
                {
                    paintID += "_vr";
                }

                if (PlatformManager.Instance.IsTomko)
                {
                    paintID += "_tomko";
                }

            }
            Debug.Log("paintID:"+ paintID);
            UIManager.Instance.DisableUI("UI_SETTING");
            UIManager.Instance.DisableUI("UI_GUIDE");
            UIManager.Instance.DisableUI("UI_VR");
            _paintDetailDict[paintID].gameObject.SetActive(true);
            _currentPaintDetail = _paintDetailDict[paintID];

            if (!PlatformManager.Instance.IsMobile && !PlatformManager.Instance.IsCloud) return;
            InputManager.Instance.DisableJoystickRotation();
            InputManager.Instance.DisableJoystick();
        }
        
        public void DisablePaintDetail(string paintID)
        {
            if (_paintDetailDict.ContainsKey(paintID))
            {
                _paintDetailDict[paintID].gameObject.SetActive(false);
                if (_currentPaintDetail == _paintDetailDict[paintID])
                {
                    _currentPaintDetail = null;
                }
            }
        }

        public void  ForceDisablePaintDetail()
        {
            if (_currentPaintDetail != null)
            {
                _currentPaintDetail.ClosePanel();
                _currentPaintDetail = null;
            }
        }
        
        public void SetCurrentPainting(PaintRotateAndZoom currentPainting)
        {
            _currentPainting = currentPainting;
            
            if(!PlatformManager.Instance.IsTomko) return;
            sliderTransform.gameObject.SetActive(true);
            var originalScale = _currentPainting.GetOriginalScalePercent();
            arcSlider.SetValue(originalScale);
            _avarageScale = originalScale;
        }

        public void RemoveCurrentPainting()
        {
            _currentPainting = null;
            
            if(!PlatformManager.Instance.IsTomko) return;
            _buttonGroup = null;
            _avarageScale = -1;
            sliderTransform.gameObject.SetActive(false);
        }

        public void SetColliderPainting(bool isActive)
        {
            if(!_currentPainting) return;
            _currentPainting.SetCollider(isActive);
        }

        public void ResetView()
        {
            if(_currentPainting == null) return;
            arcSlider.ResetSlider();
            _currentPainting.SmoothAverageResetTransform();
            if (!PlatformManager.Instance.IsVR)
                CameraManager.Instance.cameraFollowPlayer.ResetCameraInArea();
            
        }
        
        public void ZoomPainting(float percent)
        {
            if(_currentPainting == null) return;
            _currentPainting.ZoomByPercentage(percent);
            if(Mathf.Approximately(_avarageScale, -1)) return;
            if(!_buttonGroup) return;
            _buttonGroup.SetActive(percent <= _avarageScale);
        }

        public void SetButtonGroup(GameObject buttonGroupCanvas)
        {
            _buttonGroup = buttonGroupCanvas;
        }
    }
}
