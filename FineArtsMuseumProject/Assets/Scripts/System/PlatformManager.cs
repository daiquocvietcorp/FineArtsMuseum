using DesignPatterns;
using UnityEngine;

namespace System
{
    public class PlatformManager : MonoSingleton<PlatformManager>
    {
        [field: SerializeField] private PlatformType platformType;

        private void Awake()
        {
            
        }
        
        public bool IsMobile => platformType == PlatformType.Mobile;
        public bool IsStandalone => platformType == PlatformType.Standalone;
        public bool IsVR => platformType == PlatformType.VR;
        public bool IsWebGL => platformType == PlatformType.WebGL;
        public bool IsCloud => platformType == PlatformType.Cloud;
    }
    
    public enum PlatformType
    {
        Mobile,
        Standalone,
        VR,
        WebGL,
        Cloud
    }
}
