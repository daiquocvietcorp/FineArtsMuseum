using System;
using DesignPatterns;
using UnityEngine;

namespace Camera
{
    public class CameraManager : MonoSingleton<CameraManager>
    {
        [field: SerializeField] public UnityEngine.Camera mainCamera;
        [field: SerializeField] public UnityEngine.Camera xrCamera;
        [field: SerializeField] public CameraFollowPlayer cameraFollowPlayer;

        private bool _isLockFollowView;
        
        public void RegisterRotationDefault()
        {
            if (PlatformManager.Instance.IsVR) return;
            cameraFollowPlayer.RegisterRotationAction();
        }

        public void SetCameraWhenEnterPainting(float distance, float height)
        {
            if (PlatformManager.Instance.IsVR) return;
            cameraFollowPlayer.EnterArea(distance, height);
            _isLockFollowView = true;
        }

        public void SetCameraWhenExitPainting()
        {
            if (PlatformManager.Instance.IsVR) return;
            cameraFollowPlayer.ExitArea();
            _isLockFollowView = false;
        }
        
        public bool IsLockFollowView => _isLockFollowView;
    }
}
