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
        public Vector3 cameraPosition;
        public Vector3 cameraRotation;
    }
}
