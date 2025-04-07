using System;
using DesignPatterns;
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

        public void DisableInput()
        {
            mouseInput.DisableMouseOrTouchInput();
        }
        
        public void EnableInput()
        {
            mouseInput.EnableMouseOrTouchInput();
        }
        
        public void EnableJoystick()
        {
            if(!PlatformManager.Instance.IsMobile && !PlatformManager.Instance.IsCloud) return;
            joystickInput.gameObject.SetActive(true);
        }
        
        public void DisableJoystick()
        {
            joystickInput.gameObject.SetActive(false);
        }
        
        public void EnableJoystickRotation()
        {
            if(!PlatformManager.Instance.IsCloud && !PlatformManager.Instance.IsMobile) return;
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
