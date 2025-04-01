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
            if(!PlatformManager.Instance.IsCloud) return;
            joystickRotationInput.gameObject.SetActive(true);
        }
        
        public void DisableJoystickRotation()
        {
            joystickRotationInput.gameObject.SetActive(false);
        }
    }
}
