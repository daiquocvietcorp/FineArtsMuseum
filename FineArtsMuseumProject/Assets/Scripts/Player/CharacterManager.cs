using DesignPatterns;
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
    }
}
