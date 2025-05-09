using System;
using DesignPatterns;
using Player;
using UnityEngine;

namespace InputController
{
    public class InputManager : MonoSingleton<InputManager>
    {
        [field: SerializeField] private MouseInput mouseInput;
        
        [field: SerializeField] private Transform joystickInput;
        [field: SerializeField] private Transform joystickRotationInput;
        
        [field: SerializeField] private Transform joystickInputTomko;
        [field: SerializeField] private Transform joystickRotationInputTomko;

        private void Start()
        {
            if (PlatformManager.Instance.IsVR) return;
        }

        public void DisableInput()
        {
            if (PlatformManager.Instance.IsVR) return;
            mouseInput.DisableMouseOrTouchInput();
        }
        
        public void EnableInput()
        {
            mouseInput.EnableMouseOrTouchInput();
        }
        
        public void EnableJoystick()
        {
            
            if(!PlatformManager.Instance.IsMobile && !PlatformManager.Instance.IsCloud && !PlatformManager.Instance.IsTomko) return;
            joystickInput.gameObject.SetActive(true);
        }
        
        public void DisableJoystick()
        {
            if (PlatformManager.Instance.IsVR) return;
            joystickInput.gameObject.SetActive(false);
        }
        
        public void EnableJoystickRotation()
        {
            if(!PlatformManager.Instance.IsCloud && !PlatformManager.Instance.IsMobile && !PlatformManager.Instance.IsWebGL) return;
            joystickRotationInput.gameObject.SetActive(true);
        }
        
        public void DisableJoystickRotation()
        {
            if(!PlatformManager.Instance.IsCloud && !PlatformManager.Instance.IsMobile) return;
            joystickRotationInput.gameObject.SetActive(false);
        }
        
        public void SetDragImage(bool isDrag)
        {
            if (PlatformManager.Instance.IsStandalone || PlatformManager.Instance.IsWebGL)
            {
                mouseInput.SetIsDragImage(isDrag);
            }
        }
    }
}
