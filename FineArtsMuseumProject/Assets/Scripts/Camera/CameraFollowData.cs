using System;
using UnityEngine;

namespace Camera
{
    [CreateAssetMenu(fileName = "CameraFollowData", menuName = "Camera/CameraFollowData")]
    public class CameraFollowData : ScriptableObject
    {
        [field: SerializeField] public float Distance { get; set; } = 5f;
        [field: SerializeField] public float Sensitivity { get; private set; } = 2f;
        [field: SerializeField] public float MinPitch { get; private set; } = -20f;
        [field: SerializeField] public float MaxPitch { get; private set; } = 80f;
        [field: SerializeField] public float Height { get; set; } = 2f;
        [field: SerializeField] public CameraFollowDistance View1StPerson { get; private set; }
        [field: SerializeField] public CameraFollowDistance View3RdPerson { get; private set; }
    }

    [Serializable]
    public class CameraFollowDistance
    {
        [field: SerializeField]
        public float Distance { get; private set; }
        
        [field: SerializeField]
        public float Height { get; private set; }
    }
}
