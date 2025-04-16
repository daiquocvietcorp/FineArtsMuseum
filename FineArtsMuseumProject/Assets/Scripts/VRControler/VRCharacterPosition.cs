using System;
using System.Collections.Generic;
using UnityEngine;

namespace VRControler
{
    public class VRCharacterPosition : MonoBehaviour
    {
        private CharacterController _characterController;
        private Dictionary<int, VRPosition> _vrPositionDictionary;
        
        
        [field: SerializeField] private List<VRPosition> vrPositions;
        private void Start()
        {
            if(SceneLog.IsFirstScene) return;
            _characterController = GetComponent<CharacterController>();
            _characterController.enabled = false;
            
            _vrPositionDictionary = new Dictionary<int, VRPosition>();
            foreach (var vrPosition in vrPositions)
            {
                _vrPositionDictionary.Add(vrPosition.sceneId, vrPosition);
            }

            if (_vrPositionDictionary.TryGetValue(SceneLog.PreviousSceneId, out var position))
            {
                if (position != null)
                {
                    transform.localPosition = position.position;
                    transform.localEulerAngles =  position.rotation;
                }
            }
            
            
            _characterController.enabled = true;
        }
    }

    [Serializable]
    public class VRPosition
    {
        public int sceneId;
        public Vector3 position;
        public Vector3 rotation;
    }
}
