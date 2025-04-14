using DesignPatterns;
using InputController;
using UnityEngine;

namespace Player
{
    public class CharacterManager : MonoSingleton<CharacterManager>
    {
        [field: SerializeField] private CharacterStateMachine defaultCharacterStateMachine;
        
        public void DisableCharacter()
        {
            defaultCharacterStateMachine.DisableCharacter();
        }
        
        public void EnableCharacter()
        {
            defaultCharacterStateMachine.EnableCharacter();
        }
        
        public void StartControlCharacter()
        {
            defaultCharacterStateMachine.StartCharacter();
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
    }
}
