using System.Collections.Generic;
using DesignPatterns;
using UnityEngine;

namespace System
{
    public class WallManager : MonoSingleton<WallManager>
    {
        [SerializeField] private List<BoxCollider> wallColliders;
        
        public void DisableAllWalls()
        {
            foreach (var wall in wallColliders)
            {
                wall.enabled = false;
            }
        }
        
        public void EnableAllWalls()
        {
            foreach (var wall in wallColliders)
            {
                wall.enabled = true;
            }
        }
    }
}
