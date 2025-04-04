using System;
using Camera;
using InputController;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UISetting : UIBasic
    {
        
        [Header("View Buttons")]
        [field: SerializeField] private Button thirdPersonButton;
        [field: SerializeField] private Button firstPersonButton;
        
        [Header("View Components")]
        [field: SerializeField] private Image thirdPersonImage;
        [field: SerializeField] private Image firstPersonImage;
        [field: SerializeField] private TMP_Text thirdPersonText;
        [field: SerializeField] private TMP_Text firstPersonText;
        
        [Header("Language Buttons")]
        [field: SerializeField] private Button englishButton;
        [field: SerializeField] private Button vietnameseButton;
        [field: SerializeField] private Toggle englishToggle;
        [field: SerializeField] private Toggle vietnameseToggle;
        
        [Header("Language Components")]
        [field: SerializeField] private Image englishImage;
        [field: SerializeField] private Image vietnameseImage;
        [field: SerializeField] private TMP_Text englishText;
        [field: SerializeField] private TMP_Text vietnameseText;
        
        [Header("Color Settings")]
        [field: SerializeField] private Color textOnColor;
        [field: SerializeField] private Color textOffColor;
        [field: SerializeField] private Color imageOnColor;
        [field: SerializeField] private Color imageOffColor;
        
        [Header("System Settings")]
        [field: SerializeField] private Button backButton;
        
        private Action _onBackButtonClicked;
        private bool _isFirstPerson;
        private bool _isVietnamese;

        #region Setup Methods

        public override void DisableUI()
        {
            base.DisableUI();
            _onBackButtonClicked?.Invoke();
            InputManager.Instance.EnableJoystick();
            InputManager.Instance.EnableJoystickRotation();
        }
        
        public override void EnableUI()
        {
            base.EnableUI();
            InputManager.Instance.DisableJoystick();
            InputManager.Instance.DisableJoystickRotation();
        }

        private void Awake()
        {
            if (thirdPersonButton != null)
            {
                thirdPersonButton.onClick.AddListener(OnThirdPersonButtonClicked);
            }
            if (firstPersonButton != null)
            {
                firstPersonButton.onClick.AddListener(OnFirstPersonButtonClicked);
            }
            englishButton.onClick.AddListener(OnEnglishButtonClicked);
            vietnameseButton.onClick.AddListener(OnVietnameseButtonClicked);
            backButton.onClick.AddListener(OnBackButtonClicked);
            
            _isFirstPerson = false;
            _isVietnamese = true;
            
            SwitchView(false);
            SwitchLanguage(true);
        }

        private void OnBackButtonClicked()
        {
            DisableUI();
            _onBackButtonClicked?.Invoke();
        }

        private void OnVietnameseButtonClicked()
        {
            if(_isVietnamese) return;
            ClickChangeVietnamese();
            SwitchLanguage(true);
            vietnameseToggle.isOn = true;
            englishToggle.isOn = false;
            _isVietnamese = true;
        }

        private void OnEnglishButtonClicked()
        {
            if(!_isVietnamese) return;
            ClickChangeEnglish();
            SwitchLanguage(false);
            vietnameseToggle.isOn = false;
            englishToggle.isOn = true;
            _isVietnamese = false;
        }

        private void OnFirstPersonButtonClicked()
        {
            if(CameraManager.Instance.IsLockFollowView) return;
            if(_isFirstPerson) return;
            ClickChangeFirstPerson();
            SwitchView(true);
            _isFirstPerson = true;
        }

        private void OnThirdPersonButtonClicked()
        {
            if(CameraManager.Instance.IsLockFollowView) return;
            if(!_isFirstPerson) return;
            ClickChangeThirdPerson();
            SwitchView(false);
            _isFirstPerson = false;
        }
        
        private void SwitchView(bool isFirstPerson)
        {
            if (PlatformManager.Instance.IsVR) return;
            if (isFirstPerson)
            {
                firstPersonImage.color = imageOnColor;
                firstPersonText.color = textOnColor;
                thirdPersonImage.color = imageOffColor;
                thirdPersonText.color = textOffColor;
            }
            else
            {
                firstPersonImage.color = imageOffColor;
                firstPersonText.color = textOffColor;
                thirdPersonImage.color = imageOnColor;
                thirdPersonText.color = textOnColor;
            }
        }
        
        private void SwitchLanguage(bool isVietnamese)
        {
            if (isVietnamese)
            {
                vietnameseImage.color = imageOnColor;
                vietnameseText.color = textOnColor;
                englishImage.color = imageOffColor;
                englishText.color = textOffColor;
            }
            else
            {
                vietnameseImage.color = imageOffColor;
                vietnameseText.color = textOffColor;
                englishImage.color = imageOnColor;
                englishText.color = textOnColor;
            }
        }

        public override void SetData(IUIData data)
        {
            base.SetData(data);
            if(data is not UISettingData settingData) return;
            _onBackButtonClicked = settingData.OnBackButtonClicked;
        }

        #endregion

        #region Main Methods

        private void ClickChangeVietnamese()
        {
            
        }
        
        private void ClickChangeEnglish()
        {
            
        }
        
        private void ClickChangeFirstPerson()
        {
            CameraManager.Instance.cameraFollowPlayer.SetFirstPersonView();
        }
        
        private void ClickChangeThirdPerson()
        {
            CameraManager.Instance.cameraFollowPlayer.SetThirdPersonView();
        }

        #endregion
    }
    
    public class UISettingData : IUIData
    {
        public Action OnBackButtonClicked;
    }
}
