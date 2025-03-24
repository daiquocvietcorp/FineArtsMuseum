using DesignPatterns;
using UnityEngine;

namespace InputController
{
    public class InputManager : MonoSingleton<InputManager>
    {
        [field: SerializeField] private MouseInput mouseInput;
        [field: SerializeField] private JoystickInput joystickInput;

        public void DisableInput()
        {
            mouseInput.DisableMouseOrTouchInput();
        }
        
        public void EnableInput()
        {
            mouseInput.EnableMouseOrTouchInput();
        }
    }
}
