using System;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    [CreateAssetMenu(fileName = "CharacterData", menuName = "Player/CharacterData")]
    public class CharacterData : ScriptableObject
    {
        [field:SerializeField] public float JumpForce {get; private set;} = 5f;
        [field:SerializeField] public float MovementSpeed {get; private set;} = 5f;
        [field:SerializeField] public float RotationSpeed {get; private set;} = 5f;
        [field:SerializeField] public float MinJumpTime {get; private set;} = 0.2f;
        [field:SerializeField] public float WaitingJumpTime {get; private set;} = 0.2f;
        [field: SerializeField] public Vector3 DefaultPosition {get; private set;}
        [field: SerializeField] public Vector3 DefaultRotation {get; private set;}
    }
}
