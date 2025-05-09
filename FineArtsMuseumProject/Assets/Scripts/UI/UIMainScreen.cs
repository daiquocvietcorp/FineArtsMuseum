using System;
using InputController;
using Trigger;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIMainScreen : UIBasic
    {
        [Header("Toggles")]
        [field: SerializeField] private Toggle guideToggle;
        [field: SerializeField] private Toggle vrToggle;
        [field: SerializeField] private Toggle settingsToggle;
        
        [Header("Guide Sprites")]
        [field: SerializeField] private Sprite guideOnSprite;
        [field: SerializeField] private Sprite guideOffSprite;
        
        [Header("VR Sprites")]
        [field: SerializeField] private Sprite vrOnSprite;
        [field: SerializeField] private Sprite vrOffSprite;
        
        [Header("Settings Sprites")]
        [field: SerializeField] private Sprite settingsOnSprite;
        [field: SerializeField] private Sprite settingsOffSprite;
        
        private bool _isGuideOn;
        private bool _isVROn;
        private bool _isSettingsOn;
        
        #region Override Methods

        public override void EnableUI()
        {
            gameObject.SetActive(true);
        }
        
        public override void DisableUI()
        {
            gameObject.SetActive(false);
        }

        #endregion
        
        #region Setup Methods
        
        private void Awake()
        {
            if (!PlatformManager.Instance.IsVR)
            {
                vrToggle.onValueChanged.AddListener(OnVRToggleValueChanged);
                vrToggle.isOn = false;
                vrToggle.image.sprite = vrOffSprite;
                guideToggle.onValueChanged.AddListener(OnGuideToggleValueChanged);
            }
            settingsToggle.onValueChanged.AddListener(OnSettingsToggleValueChanged);
            
            _isVROn = false;
            _isSettingsOn = false;
            settingsToggle.isOn = false;

            if (SceneLog.IsFirstScene && !PlatformManager.Instance.IsVR)
            {
                _isGuideOn = true;
                guideToggle.isOn = true;
                guideToggle.image.sprite = guideOnSprite;
            }
            else
            {
                if (!PlatformManager.Instance.IsVR)
                {
                    _isGuideOn = false;
                    guideToggle.isOn = false;
                    guideToggle.image.sprite = guideOffSprite;
                }
                
            }
            
            settingsToggle.image.sprite = settingsOffSprite;
        }

        private void Start()
        {
            var settingCallback = new UISettingData()
            {
                OnBackButtonClicked = () =>
                {
                    settingsToggle.isOn = false;
                    settingsToggle.image.sprite = settingsOffSprite;
                    _isSettingsOn = false;
                }
            };
            UIManager.Instance.SetDataUI("UI_SETTING", settingCallback);
            
            var guideCallback = new UIGuideData()
            {
                OnBackButtonClicked = () =>
                {
                    guideToggle.isOn = false;
                    guideToggle.image.sprite = guideOffSprite;
                    _isGuideOn = false;
                    
                    InputManager.Instance.EnableJoystick();
                    InputManager.Instance.EnableJoystickRotation();
                }
            };
            UIManager.Instance.SetDataUI("UI_GUIDE", guideCallback);
            
            var vrCallback = new UivrData()
            {
                CloseAction = () =>
                {
                    vrToggle.isOn = false;
                    vrToggle.image.sprite = vrOffSprite;
                    _isVROn = false;
                }
            };
            
            UIManager.Instance.SetDataUI("UI_VR", vrCallback);
        }

        private void OnSettingsToggleValueChanged(bool arg0)
        {
            if(_isSettingsOn == arg0) return;

            if (arg0)
            {
                CheckToggleOn();
                settingsToggle.image.sprite = settingsOnSprite;
                SettingOn();
            }
            else
            {
                settingsToggle.image.sprite = settingsOffSprite;
                SettingOff();
            }
            
            _isSettingsOn = arg0;
            
        }

        private void OnVRToggleValueChanged(bool arg0)
        {
            
            if(_isVROn == arg0) return;

            if (arg0)
            {
                CheckToggleOn();
                vrToggle.image.sprite = vrOnSprite;
                VROn();
            }
            else
            {
                vrToggle.image.sprite = vrOffSprite;
                VROff();
            }
            
            _isVROn = arg0;
        }

        private void OnGuideToggleValueChanged(bool arg0)
        {
            
            if(_isGuideOn == arg0) return;
            
            if (arg0)
            {
                CheckToggleOn();
                guideToggle.image.sprite = guideOnSprite;
                GuideOn();
            }
            else
            {
                guideToggle.image.sprite = guideOffSprite;
                GuideOff();
            }
            
            _isGuideOn = arg0;
        }
        
        private void CheckToggleOn()
        {
            if (_isSettingsOn)
            {
                _isSettingsOn = false;
                settingsToggle.isOn = false;
                settingsToggle.image.sprite = settingsOffSprite;
                SettingOff();
            }
            
            if (_isVROn)
            {
                _isVROn = false;
                vrToggle.isOn = false;
                vrToggle.image.sprite = vrOffSprite;
                VROff();
            }

            if (!_isGuideOn) return;
            _isGuideOn = false;
            guideToggle.isOn = false;
            guideToggle.image.sprite = guideOffSprite;
            GuideOff();
        }
        
        private void DisablePainting()
        {
            PaintingDetailManager.Instance.ForceDisablePaintDetail();
        }

        #endregion

        #region Main Methods

        private void SettingOff()
        {
            UIManager.Instance.DisableUI("UI_SETTING");
        }

        private void SettingOn()
        {
            UIPaintingManager.Instance.ForceDisableUIPaintingZoom();
            UIManager.Instance.EnableUI("UI_SETTING");
            DisablePainting();
        }
        
        private void VROff()
        {
            UIPaintingManager.Instance.ForceDisableUIPaintingZoom();
            DisablePainting();
            UIManager.Instance.DisableUI("UI_VR");
        }

        private void VROn()
        {
            UIPaintingManager.Instance.ForceDisableUIPaintingZoom();
            DisablePainting();
            UIManager.Instance.EnableUI("UI_VR");
        }
        
        private void GuideOff()
        {
            UIManager.Instance.DisableUI("UI_GUIDE");
        }

        private void GuideOn()
        {
            UIPaintingManager.Instance.ForceDisableUIPaintingZoom();
            UIManager.Instance.EnableUI("UI_GUIDE");
            UIManager.Instance.ActionUI("UI_GUIDE");
            DisablePainting();
        }

        #endregion
    }
}
