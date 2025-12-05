using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    [CreateAssetMenu(fileName = "BuildingCollectSO", menuName = "Building/CollectFloor")]
    public class BuildingCollectSO : ScriptableObject
    {
        [field: SerializeField] public List<CollectFloor> floors;
    }
    
    [Serializable]
    public class CollectFloor
    {
        public int floor;
        public CollectCamera camera;
        public CollectPlayer player;
    }

    [Serializable]
    public class CollectCamera
    {
        public Vector3 cameraPosition;
        public Vector3 cameraRotation;
    }
    
    [Serializable]
    public class CollectPlayer
    {
        public Vector3 playerPosition;
        public Vector3 playerRotation;
    }
}
