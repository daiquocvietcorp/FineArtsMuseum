using System;
using System.Collections.Generic;
using DesignPatterns;
using InputController;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Utils;

namespace Player
{
    public class CharacterManager : MonoSingleton<CharacterManager>
    {
        [field: SerializeField] private CharacterStateMachine defaultCharacterStateMachine;
        [field: SerializeField] private BuildingCollectSO buildingCollectSo;
        
        private Dictionary<int, CollectPlayer> _collectPlayerDictionary;
        
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
            Initialize();
            
            defaultCharacterStateMachine.StartCharacter();
            
            if(SceneLog.IsFirstView) HideCharacterSkin();
            else ShowCharacterSkin();
        }

        private void Initialize()
        {
            if(buildingCollectSo == null || buildingCollectSo.floors == null || buildingCollectSo.floors.Count == 0) return;
            _collectPlayerDictionary = new Dictionary<int, CollectPlayer>();
            foreach (var collectFloor in buildingCollectSo.floors)
            {
                if(_collectPlayerDictionary.ContainsKey(collectFloor.floor)) continue;
                var collectPlayer = collectFloor.player;
                if(collectPlayer == null) continue;
                _collectPlayerDictionary.Add(collectFloor.floor, collectPlayer);
            }
        }

        public void SetFloorForCharacter(int floor)
        {
            if(_collectPlayerDictionary == null || _collectPlayerDictionary.Count == 0) return;
            if(!_collectPlayerDictionary.TryGetValue(floor, out var collectPlayer)) return;
            defaultCharacterStateMachine.SetCharacter(collectPlayer.playerPosition, collectPlayer.playerRotation);
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
