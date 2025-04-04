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
            cameraFollowPlayer.RegisterRotationAction();
        }

        public void SetCameraWhenEnterPainting(float distance, float height)
        {
            cameraFollowPlayer.EnterPainting(distance, height);
            _isLockFollowView = true;
        }

        public void SetCameraWhenExitPainting()
        {
            cameraFollowPlayer.ExitPainting();
            _isLockFollowView = false;
        }
        
        public bool IsLockFollowView => _isLockFollowView;
    }
}
