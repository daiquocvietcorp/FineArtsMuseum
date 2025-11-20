using System;
using DesignPatterns;
using InputController;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class CharacterManager : MonoSingleton<CharacterManager>
    {
        [field: SerializeField] private CharacterStateMachine defaultCharacterStateMachine;
        
        public void DisableCharacter()
        {
            if (PlatformManager.Instance.IsVR) return;
            defaultCharacterStateMachine.DisableCharacter();
        }
        
        public void EnableCharacter()
        {
            if (PlatformManager.Instance.IsVR) return;
            defaultCharacterStateMachine.EnableCharacter();
        }
        
        public void StartControlCharacter()
        {
            defaultCharacterStateMachine.StartCharacter();
            
            if(SceneLog.IsFirstView) HideCharacterSkin();
            else ShowCharacterSkin();
        }
        
        public void StopControlCharacter()
        {
            defaultCharacterStateMachine.StopCharacter();
        }

        public void RegisterActionDefault()
        {
            defaultCharacterStateMachine.RegisterJoystickAction();
        }
        
        public void ShowCharacterSkin()
        {
            if(SceneLog.IsBlockThirdView) return;
            defaultCharacterStateMachine.ShowCharacter();
        }
        
        public void HideCharacterSkin()
        {
            defaultCharacterStateMachine.HideCharacter();
        }

        public void SetCharacterInfo(Vector3 valuePlayerPosition, Vector3 valuePlayerRotation)
        {
            defaultCharacterStateMachine.SetCharacter(valuePlayerPosition, valuePlayerRotation);
        }

        public System.Action<InputAction.CallbackContext> GetActionMove()
        {
            return defaultCharacterStateMachine.MoveByNewInput;
        }
    }
}
