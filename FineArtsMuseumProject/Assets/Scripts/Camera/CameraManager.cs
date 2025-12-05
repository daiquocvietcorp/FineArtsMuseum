using System;
using DesignPatterns;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;

namespace Camera
{
    public class CameraManager : MonoSingleton<CameraManager>
    {
        [field: SerializeField] public UnityEngine.Camera mainCamera;
        [field: SerializeField] public UnityEngine.Camera xrCamera;
        [field: SerializeField] public CameraFollowPlayer cameraFollowPlayer;

        private bool _isLockFollowView;
        private bool _isLockRotateCamera;
        
        public void RegisterRotationDefault()
        {
            if (PlatformManager.Instance.IsVR) return;
            cameraFollowPlayer.RegisterRotationAction();
        }
        
        public void SetLockRotateCamera(bool isLock)
        {
            _isLockRotateCamera = isLock;
        }
        
        public System.Action<InputAction.CallbackContext> GetActionRotate()
        {
            return cameraFollowPlayer.RotateByNewInput;
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
        
        public void SetCameraRotationByFloor(int floor)
        {
            var collectCamera = ScriptableObjectManager.Instance.GetCollectCamera(floor);
            if (collectCamera == null) return;
            cameraFollowPlayer.SetCameraRotation(collectCamera.cameraRotation, collectCamera.cameraPosition);
        }
        
        public bool IsLockFollowView => _isLockFollowView || cameraFollowPlayer.IsLocked;
        public bool IsLockRotateCamera => _isLockRotateCamera;
    }
}
