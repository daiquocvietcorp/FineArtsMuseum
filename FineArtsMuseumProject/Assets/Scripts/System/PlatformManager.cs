using DesignPatterns;
using UnityEngine;

namespace System
{
    public class PlatformManager : MonoSingleton<PlatformManager>
    {
        [field: SerializeField] private PlatformType platformType;
        [field: SerializeField] private DeviceType deviceType;
        private void Awake()
        {
            
        }
        
        public bool IsMobile => platformType == PlatformType.Mobile;
        public bool IsStandalone => platformType == PlatformType.Standalone;
        public bool IsVR => platformType == PlatformType.VR;
        public bool IsWebGL => platformType == PlatformType.WebGL;
        public bool IsCloud => platformType == PlatformType.Cloud;
        public bool IsTomko => platformType == PlatformType.Tomko;
        public bool IsTomkoDevice => deviceType == DeviceType.Tomko;
    }
    
    public enum PlatformType
    {
        Mobile,
        Standalone,
        VR,
        WebGL,
        Cloud,
        Tomko
    }

    public enum DeviceType
    {
        Desktop,
        Mobile,
        VR,
        Tomko
    }
}
